using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.Roulette;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class RouletteGameConfigService : IRouletteGameConfigService
    {
        private readonly CasinoDBContext _context;

        public RouletteGameConfigService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<RouletteGameConfigResponse>> GetAllAsync()
        {
            return await _context.RouletteGameConfigs
                .Include(c => c.Game)
                .Select(c => new RouletteGameConfigResponse
                {
                    Id = c.Id,
                    TableName = c.TableName,
                    RouletteType = c.RouletteType,
                    HasZero = c.HasZero,
                    HasDoubleZero = c.HasDoubleZero,
                    TotalNumbers = c.TotalNumbers,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    GameId = c.GameId,
                    GameName = c.Game.GameName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<RouletteGameConfigResponse?> GetByIdAsync(Guid id)
        {
            var config = await _context.RouletteGameConfigs
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return null;

            return new RouletteGameConfigResponse
            {
                Id = config.Id,
                TableName = config.TableName,
                RouletteType = config.RouletteType,
                HasZero = config.HasZero,
                HasDoubleZero = config.HasDoubleZero,
                TotalNumbers = config.TotalNumbers,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt,
                GameId = config.GameId,
                GameName = config.Game.GameName
            };
        }

        // CREATE
        public async Task<RouletteGameConfigResponse> CreateAsync(CreateRouletteGameConfigRequest dto)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "roulette");

            if (game == null)
            {
                game = new Game
                {
                    Id = Guid.NewGuid(),
                    GameName = "Roulette-Common",
                    GameType = "Roulette",
                    Description = "Juego de ruleta clásica",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }

            var config = new RouletteGameConfig
            {
                Id = Guid.NewGuid(),
                TableName = dto.TableName,
                RouletteType = dto.RouletteType,
                HasZero = dto.HasZero,
                HasDoubleZero = dto.HasDoubleZero,
                TotalNumbers = dto.TotalNumbers,
                IsActive = dto.IsActive,
                GameId = game.Id,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.RouletteGameConfigs.Add(config);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(config.Id)
                ?? throw new Exception("Error al recuperar la configuración creada");
        }

        // UPDATE
        public async Task<RouletteGameConfigResponse?> UpdateAsync(Guid id, UpdateRouletteGameConfigRequest dto)
        {
            var config = await _context.RouletteGameConfigs.FindAsync(id);
            if (config == null) return null;

            config.TableName = dto.TableName;
            config.RouletteType = dto.RouletteType;
            config.HasZero = dto.HasZero;
            config.HasDoubleZero = dto.HasDoubleZero;
            config.TotalNumbers = dto.TotalNumbers;
            config.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // TOGGLE ACTIVE
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var config = await _context.RouletteGameConfigs.FindAsync(id);
            if (config == null) return false;

            config.IsActive = !config.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var config = await _context.RouletteGameConfigs.FindAsync(id);
            if (config == null) return false;

            if (config.IsActive)
                throw new InvalidOperationException(
                    "No puedes eliminar una configuración activa. Desactívala primero.");

            // Borrar sesiones asociadas primero
            var sessions = await _context.RouletteSessions
                .Where(s => s.RouletteGameConfigId == id)
                .ToListAsync();

            if (sessions.Any())
                _context.RouletteSessions.RemoveRange(sessions);

            // Borrar BetTypes asociados
            var betTypes = await _context.RouletteBetTypes
                .Where(b => b.RouletteGameConfigId == id)
                .ToListAsync();

            if (betTypes.Any())
                _context.RouletteBetTypes.RemoveRange(betTypes);

            _context.RouletteGameConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
