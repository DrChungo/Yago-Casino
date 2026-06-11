using Chaos.Api.Interface.Config;
using Chaos.Api.ResponseEntity.Config.CoinGame;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class CoinGameService : ICoinGameService
    {
        private readonly CasinoDBContext _context;

        public CoinGameService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<CoinGameResponde>> GetAllAsync()
        {
            return await _context.CoinFlipGameConfigs
                .Include(c => c.Game)
                .Select(c => new CoinGameResponde
                {
                    Id = c.Id,
                    ConfigName = c.ConfigName,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    GameId = c.GameId,
                    GameName = c.Game.GameName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<CoinGameResponde?> GetByIdAsync(Guid id)
        {
            var config = await _context.CoinFlipGameConfigs
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return null;

            return new CoinGameResponde
            {
                Id = config.Id,
                ConfigName = config.ConfigName,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt,
                GameId = config.GameId,
                GameName = config.Game.GameName
            };
        }

        // CREATE
        public async Task<CoinGameResponde> CreateAsync(CoinGameCreate dto)
        {
            // Asegurar que exista el juego "CoinFlip" antes de crear la config. Si no existe, lo creamos automáticamente.
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "coinflip");

            if (game == null)
            {
                game = new Game
                {
                    Id = Guid.NewGuid(),
                    GameName = "Coin Flip-Common",
                    GameType = "CoinFlip",
                    Description = "Cara o cruz, tú decides",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }

            //Si la nueva config es activa, desactivar todas las anteriores
            if (dto.IsActive)
            {
                var activeConfigs = await _context.CoinFlipGameConfigs
                    .Where(c => c.IsActive)
                    .ToListAsync();

                foreach (var active in activeConfigs)
                    active.IsActive = false;
            }

            var config = new CoinFlipGameConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = dto.ConfigName,
                GameId = game.Id,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.CoinFlipGameConfigs.Add(config);
            await _context.SaveChangesAsync();

            //Null safety al recuperar la config creada
            return await GetByIdAsync(config.Id)
                ?? throw new Exception("Error al recuperar la configuración creada");
        }

        // UPDATE
        public async Task<CoinGameResponde?> UpdateAsync(Guid id, CoinGameUpdate dto)
        {
            var config = await _context.CoinFlipGameConfigs.FindAsync(id);
            if (config == null) return null;

            //Si se activa esta config, desactivar las demás
            if (dto.IsActive && !config.IsActive)
            {
                var activeConfigs = await _context.CoinFlipGameConfigs
                    .Where(c => c.IsActive && c.Id != id)
                    .ToListAsync();

                foreach (var active in activeConfigs)
                    active.IsActive = false;
            }

            config.ConfigName = dto.ConfigName;
            config.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // TOGGLE ACTIVE
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var config = await _context.CoinFlipGameConfigs.FindAsync(id);
            if (config == null) return false;

            //Si se va a activar, desactivar las demás primero
            if (!config.IsActive)
            {
                var activeConfigs = await _context.CoinFlipGameConfigs
                    .Where(c => c.IsActive && c.Id != id)
                    .ToListAsync();

                foreach (var active in activeConfigs)
                    active.IsActive = false;
            }

            config.IsActive = !config.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var config = await _context.CoinFlipGameConfigs.FindAsync(id);

            if (config == null) return false;

            if (config.IsActive)
                throw new InvalidOperationException("No puedes eliminar una configuración activa. Desactívala primero.");

            //Borrar sesiones asociadas primero
            var sessions = await _context.CoinFlipSessions
                .Where(s => s.CoinFlipGameConfigId == id)
                .ToListAsync();

            if (sessions.Any())
                _context.CoinFlipSessions.RemoveRange(sessions);

            //Ahora sí borrar la config
            _context.CoinFlipGameConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}