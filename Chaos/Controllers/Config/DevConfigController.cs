using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Chaos.Infraestructure.Models;
using Chaos.Api.Interface.Config;
using Chaos.Api.ResponseEntity.Config.AnimalValue;

namespace Chaos.Api.Controllers
{
    [ApiController]
    [Route("api/dev/animalconfigs")]
    public class DevConfigController : ControllerBase
    {
        private readonly CasinoDBContext _context;
        private readonly IAnimalValueConfigService _service;

        public DevConfigController(CasinoDBContext context, IAnimalValueConfigService service)
        {
            _context = context;
            _service = service;
        }

        private string GetJsonPath()
        {
            var paths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "../ChaosFrontend/src/AnimalMovement/animalConfigs.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "ChaosFrontend/src/AnimalMovement/animalConfigs.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../ChaosFrontend/src/AnimalMovement/animalConfigs.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AnimalMovement/animalConfigs.json")
            };

            foreach (var path in paths)
            {
                var fullPath = Path.GetFullPath(path);
                if (System.IO.File.Exists(fullPath)) return fullPath;
                
                var dir = Path.GetDirectoryName(fullPath);
                if (dir != null && Directory.Exists(dir)) return fullPath;
            }

            return Path.GetFullPath(paths[0]);
        }

        [HttpGet]
        public async Task<IActionResult> GetConfigs()
        {
            var jsonPath = GetJsonPath();
            var configs = new List<AnimalConfigItem>();

            if (System.IO.File.Exists(jsonPath))
            {
                try
                {
                    var content = await System.IO.File.ReadAllTextAsync(jsonPath);
                    configs = JsonSerializer.Deserialize<List<AnimalConfigItem>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<AnimalConfigItem>();
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("Error reading JSON: " + ex.Message);
                }
            }

            // Sync with DB: Get active animal types from DB
            var dbAnimals = await _service.GetAnimalImage(); // returns List of configs / images
            var dbTypes = dbAnimals.Select(a => a.AnimalType).Where(t => !string.IsNullOrEmpty(t)).ToList();

            bool changed = false;
            foreach (var type in dbTypes)
            {
                var normType = type.Trim().ToLower();
                if (!configs.Any(c => c.Tipo.Trim().ToLower() == normType))
                {
                    // Find habitat in db configs
                    var dbConf = dbAnimals.FirstOrDefault(a => a.AnimalType?.Trim().ToLower() == normType);
                    var habitat = dbConf?.Habitat ?? "Farm";

                    configs.Add(new AnimalConfigItem
                    {
                        Tipo = normType,
                        Habitat = habitat,
                        AnchoHitbox = 40,
                        AltoHitbox = 40,
                        Escala = 1
                    });
                    changed = true;
                }
            }

            if (changed)
            {
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
                var json = JsonSerializer.Serialize(configs, options);
                var dir = Path.GetDirectoryName(jsonPath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                await System.IO.File.WriteAllTextAsync(jsonPath, json);
            }

            return Ok(configs);
        }

        [HttpPost]
        public async Task<IActionResult> SaveConfigs([FromBody] List<AnimalConfigItem> configs)
        {
            var jsonPath = GetJsonPath();
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
            var json = JsonSerializer.Serialize(configs, options);

            var dir = Path.GetDirectoryName(jsonPath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await System.IO.File.WriteAllTextAsync(jsonPath, json);
            return Ok(new { success = true, message = "Configuraciones guardadas correctamente." });
        }
    }

    public class AnimalConfigItem
    {
        public string Tipo { get; set; } = string.Empty;
        public string Habitat { get; set; } = string.Empty;
        public double AnchoHitbox { get; set; }
        public double AltoHitbox { get; set; }
        public double Escala { get; set; }
    }
}
