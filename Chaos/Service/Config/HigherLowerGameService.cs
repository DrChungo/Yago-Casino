using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.HigherLower;
using Chaos.Api.ResponseEntity.Config.HigherLower;

using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class HigherLowerGameService : IHigherLowerGameService
    {
        private readonly CasinoDBContext _context;

        public HigherLowerGameService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<HigherLowerGameResponse>> GetAllAsync()
        {
            return await _context.HigherLowerGameConfigs
                .Include(c => c.Game)
                .Select(c => new HigherLowerGameResponse
                {
                    Id = c.Id,
                    ConfigName = c.ConfigName,
                    BaseMultiplier = c.BaseMultiplier,
                    RoundIncrement = c.RoundIncrement,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    GameId = c.GameId,
                    GameName = c.Game.GameName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<HigherLowerGameResponse?> GetByIdAsync(Guid id)
        {
            var config = await _context.HigherLowerGameConfigs
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return null;

            return new HigherLowerGameResponse
            {
                Id = config.Id,
                ConfigName = config.ConfigName,
                BaseMultiplier = config.BaseMultiplier,
                RoundIncrement = config.RoundIncrement,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt,
                GameId = config.GameId,
                GameName = config.Game.GameName
            };
        }

        // CREATE
        public async Task<HigherLowerGameResponse> CreateAsync(HigherLowerCreateRequest dto)
        {
            // Asegurar que exista el juego "HigherLower" antes de crear la config.
            // Si no existe, lo creamos automáticamente.
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "higherlower");

            if (game == null)
            {
                game = new Game
                {
                    Id = Guid.NewGuid(),
                    GameName = "Higher Lower-Common",
                    GameType = "HigherLower",
                    Description = "Adivina si el siguiente número es mayor o menor",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }

    

            var config = new HigherLowerGameConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = dto.ConfigName,
                BaseMultiplier = dto.BaseMultiplier,
                RoundIncrement = dto.RoundIncrement,
                GameId = game.Id,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.HigherLowerGameConfigs.Add(config);
            await _context.SaveChangesAsync();

            // Null safety al recuperar la config creada
            return await GetByIdAsync(config.Id)
                ?? throw new Exception("Error al recuperar la configuración creada");
        }

        // UPDATE
        public async Task<HigherLowerGameResponse?> UpdateAsync(Guid id, HigherLowerUpdateRequest dto)
        {
            var config = await _context.HigherLowerGameConfigs.FindAsync(id);
            if (config == null) return null;

      

            config.ConfigName = dto.ConfigName;
            config.BaseMultiplier = dto.BaseMultiplier;
            config.RoundIncrement = dto.RoundIncrement;
            config.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // TOGGLE ACTIVE
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var config = await _context.HigherLowerGameConfigs.FindAsync(id);
            if (config == null) return false;

            // Si se va a activar, desactivar las demás primero
         

            config.IsActive = !config.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var config = await _context.HigherLowerGameConfigs.FindAsync(id);

            if (config == null) return false;

            if (config.IsActive)
                throw new InvalidOperationException(
                    "No puedes eliminar una configuración activa. Desactívala primero.");

            // Borrar sesiones asociadas primero
            var sessions = await _context.HigherLowerSessions
                .Where(s => s.HigherLowerGameConfigId == id)
                .ToListAsync();

            if (sessions.Any())
                _context.HigherLowerSessions.RemoveRange(sessions);

            // Ahora sí borrar la config
            _context.HigherLowerGameConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}