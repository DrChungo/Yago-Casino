using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.RussianRoulette;
using Chaos.Api.ResponseEntity.Config.RussianRoulette;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class RussianRouletteService : IRussianRouletteService
    {
        private readonly CasinoDBContext _context;
        public RussianRouletteService(CasinoDBContext context)
        {
            _context = context;
        }

        public async Task<RussianRouletteGameResponse> CreateAsync(RussianRouletteCreateRequest dto)
        {
            var game = _context.Games.FirstOrDefault(g => g.GameType == "Russian Roulette");

            if (game == null)
            {
                game = new Game
                {
                    Id = Guid.NewGuid(),
                    GameName = "Russian Roulette",
                    GameType = "Russian Roulette",
                    Description = "A deadly game of chance where players take turns spinning a revolver loaded with a few bullets.",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.ToString("o")
                };
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }

            var config = new RussianRouletteGameConfig
            {
                Id = Guid.NewGuid(),
                ConfigName = dto.ConfigName,
                TotalChambers = dto.TotalChambers,
                BulletCount = dto.BulletCount,
                MaxPlayers = dto.MaxPlayers,
                MinPlayers = dto.MinPlayers,
                FixedPrizePool = dto.FixedPrizePool,
                AllowBots = dto.AllowBots,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                GameId = game.Id
            };

            _context.RussianRouletteGameConfigs.Add(config);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(config.Id) ?? throw new Exception("Failed to retrieve the created configuration.");

        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var config = await _context.RussianRouletteGameConfigs.FindAsync(id);
            if (config == null) return false;

            if (config.IsActive) throw new InvalidOperationException("No puedes eliminar una configuración activa. Desactívala primero.");


            var lobbies = await _context.RussianRouletteLobbies.Where(l => l.RussianRouletteGameConfigId == id)
                .ToListAsync();

            if (lobbies.Any())
              _context.RussianRouletteLobbies.RemoveRange(lobbies);
            _context.RussianRouletteGameConfigs.Remove(config);


            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<RussianRouletteGameResponse>> GetAllAsync()
        {
            return await _context.RussianRouletteGameConfigs.Include(c => c.Game).Select(c => new RussianRouletteGameResponse
            {
                Id = c.Id,
                ConfigName = c.ConfigName,
                TotalChambers = c.TotalChambers,
                BulletCount = c.BulletCount,
                MaxPlayers = c.MaxPlayers,
                MinPlayers = c.MinPlayers,
                FixedPrizePool = c.FixedPrizePool,
                AllowBots = c.AllowBots,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                GameId = c.GameId,
                GameName = c.Game.GameName


            }).ToListAsync();



        }

        public async Task<RussianRouletteGameResponse?> GetByIdAsync(Guid id)
        {
            var config = await _context.RussianRouletteGameConfigs.Include(c => c.Game).FirstOrDefaultAsync(c => c.Id == id);
            if (config == null) return null;
            return new RussianRouletteGameResponse
            {
                Id = config.Id,
                ConfigName = config.ConfigName,
                TotalChambers = config.TotalChambers,
                BulletCount = config.BulletCount,
                MaxPlayers = config.MaxPlayers,
                MinPlayers = config.MinPlayers,
                FixedPrizePool = config.FixedPrizePool,
                AllowBots = config.AllowBots,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt,
                GameId = config.GameId,
                GameName = config.Game.GameName
            };
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var config = _context.RussianRouletteGameConfigs.FindAsync(id);
            if (config == null) return false;
            config.Result.IsActive = !config.Result.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RussianRouletteGameResponse?> UpdateAsync(Guid id, RussianRouletteUpdateRequest dto)
        {
            var config = _context.RussianRouletteGameConfigs.FindAsync(id);
            if (config == null) return null;

            config.Result.ConfigName = dto.ConfigName;
            config.Result.TotalChambers = dto.TotalChambers;
            config.Result.BulletCount = dto.BulletCount;
            config.Result.MaxPlayers = dto.MaxPlayers;
            config.Result.MinPlayers = dto.MinPlayers;
            config.Result.FixedPrizePool = dto.FixedPrizePool;
            config.Result.AllowBots = dto.AllowBots;
            config.Result.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);

        }
    }
}
