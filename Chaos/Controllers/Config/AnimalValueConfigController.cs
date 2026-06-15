using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.AnimalValue;
using Microsoft.AspNetCore.Mvc;


namespace Chaos.Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AnimalValueConfigController : ControllerBase
        {
            private readonly IAnimalValueConfigService _service;
            private readonly IWebHostEnvironment _env;

            public AnimalValueConfigController(IAnimalValueConfigService service, IWebHostEnvironment env)
            {
                _service = service;
                _env = env;
            }

        // GET SVG FILES — returns available SVG file paths from wwwroot/images/animals/
        [HttpGet("svg-files")]
        public IActionResult GetSvgFiles()
        {
            var animalsPath = Path.Combine(_env.WebRootPath, "images", "animals");

            if (!Directory.Exists(animalsPath))
                return Ok(new List<string>());

            var files = Directory.GetFiles(animalsPath, "*.svg")
                .Select(f => "/images/animals/" + Path.GetFileName(f))
                .OrderBy(f => f)
                .ToList();

            return Ok(files);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET BY ID — este sí devuelve SVGs (se llama al hacer clic)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound($"No se encontró configuración con Id: {id}");
            return Ok(result);
        }

        [HttpGet("images")]
        public async Task<IActionResult> GetAnimalImage()
        {
            var result = await _service.GetAnimalImage();
            return Ok(result);
        }

        // CREATE
        [HttpPost]
            public async Task<IActionResult> Create([FromBody] CreateAnimalValueConfigRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }

            // UPDATE
            [HttpPut("{id:guid}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnimalValueConfigRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.UpdateAsync(id, request);

                if (result == null)
                    return NotFound($"No se encontró configuración con Id: {id}");

                return Ok(result);
            }

            // DELETE
            [HttpDelete("{id:guid}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound($"No se encontró configuración con Id: {id}");

                return NoContent();
            }



        }
    }


