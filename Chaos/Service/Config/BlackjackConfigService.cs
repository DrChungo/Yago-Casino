using Chaos.Api.Interface.Config;
using Chaos.Api.ResponseEntity.Config.Blackjack;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class BlackjackConfigService : IBlackjackConfigService
    {
        private readonly CasinoDBContext _context;

        public BlackjackConfigService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<BlackjackConfigResponse>> GetAllAsync()
        {
            return await _context.BlackjackGameConfigs
                .Include(c => c.Game)
                .Select(c => new BlackjackConfigResponse
                {
                    Id = c.Id,
                    TableName = c.TableName,
                    NumberOfDecks = c.NumberOfDecks,
                    MaxPlayers = c.MaxPlayers,
                    DealerStandsOn = c.DealerStandsOn,
                    BlackjackPayout = c.BlackjackPayout,
                    AllowDoubleDown = c.AllowDoubleDown,
                    AllowInsurance = c.AllowInsurance,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    GameId = c.GameId,
                    GameName = c.Game.GameName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<BlackjackConfigResponse?> GetByIdAsync(Guid id)
        {
            var config = await _context.BlackjackGameConfigs
                .Include(c => c.Game)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (config == null) return null;

            return new BlackjackConfigResponse
            {
                Id = config.Id,
                TableName = config.TableName,
                NumberOfDecks = config.NumberOfDecks,
                MaxPlayers = config.MaxPlayers,
                DealerStandsOn = config.DealerStandsOn,
                BlackjackPayout = config.BlackjackPayout,
                AllowDoubleDown = config.AllowDoubleDown,
                AllowInsurance = config.AllowInsurance,
                IsActive = config.IsActive,
                CreatedAt = config.CreatedAt,
                GameId = config.GameId,
                GameName = config.Game.GameName
            };
        }

        // CREATE
        public async Task<BlackjackConfigResponse> CreateAsync(BlackjackCreateResponse dto)
        {
            // Asegurar que exista el juego "Blackjack" — si no existe, lo creamos automáticamente
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "blackjack");

            if (game == null)
            {
                game = new Game
                {
                    Id = Guid.NewGuid(),
                    GameName = "Blackjack-Common",
                    GameType = "Blackjack",
                    Description = "Llega a 21 sin pasarte",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Games.Add(game);
                await _context.SaveChangesAsync();
            }

       

            var config = new BlackjackGameConfig
            {
                Id = Guid.NewGuid(),
                TableName = dto.TableName,
                NumberOfDecks = dto.NumberOfDecks,
                MaxPlayers = dto.MaxPlayers,
                DealerStandsOn = dto.DealerStandsOn,
                BlackjackPayout = dto.BlackjackPayout,
                AllowDoubleDown = dto.AllowDoubleDown,
                AllowInsurance = dto.AllowInsurance,
                GameId = game.Id,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
            };

            _context.BlackjackGameConfigs.Add(config);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(config.Id)
                ?? throw new Exception("Error al recuperar la configuración creada");
        }

        // UPDATE
        public async Task<BlackjackConfigResponse?> UpdateAsync(Guid id, BlackjackUpdateResponse dto)
        {
            var config = await _context.BlackjackGameConfigs.FindAsync(id);
            if (config == null) return null;


            config.TableName = dto.TableName;
            config.NumberOfDecks = dto.NumberOfDecks;
            config.MaxPlayers = dto.MaxPlayers;
            config.DealerStandsOn = dto.DealerStandsOn;
            config.BlackjackPayout = dto.BlackjackPayout;
            config.AllowDoubleDown = dto.AllowDoubleDown;
            config.AllowInsurance = dto.AllowInsurance;
            config.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        // TOGGLE ACTIVE
        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var config = await _context.BlackjackGameConfigs.FindAsync(id);
            if (config == null) return false;

  

            config.IsActive = !config.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var config = await _context.BlackjackGameConfigs.FindAsync(id);
            if (config == null) return false;

            if (config.IsActive)
                throw new InvalidOperationException("No puedes eliminar una configuración activa. Desactívala primero.");

            // Borrar sesiones asociadas primero
            var sessions = await _context.BlackjackSessions
                .Where(s => s.BlackjackGameConfigId == id)
                .ToListAsync();

            if (sessions.Any())
                _context.BlackjackSessions.RemoveRange(sessions);

            _context.BlackjackGameConfigs.Remove(config);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}