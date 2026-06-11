using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class SlotGameConfigService : ISlotGameConfigService
    {
        private readonly CasinoDBContext _context;

        public SlotGameConfigService(CasinoDBContext context)
        {
            _context = context;
        }
        // GET ALL
        public async Task<IEnumerable<SlotGameConfigResponse>> GetAllAsync()
        {
            return await _context.SlotGameConfigs
                .Select(x => new SlotGameConfigResponse
                {
                    Id = x.Id,
                    MachineName = x.MachineName,
                    Multiplier = x.Multiplier,
                    NumberOfReels = x.NumberOfReels,
                    NumberOfRows = x.NumberOfRows,
                    PayLines = x.PayLines,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    GameId = x.GameId,
                    GameName = x.Game.GameName



                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<SlotGameConfigResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _context.SlotGameConfigs.Include(x => x.Game)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new SlotGameConfigResponse
            {
                Id = entity.Id,
                MachineName = entity.MachineName,
                Multiplier = entity.Multiplier,
                NumberOfReels = entity.NumberOfReels,
                NumberOfRows = entity.NumberOfRows,
                PayLines = entity.PayLines,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                GameId = entity.GameId,
                GameName = entity.Game.GameName

            };
        }

        // CREATE
        public async Task<SlotGameConfigResponse> CreateAsync(CreateSlotGameConfigRequest request)
        {
            // Obtiene el GameId automáticamente por GameType
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.GameType == "Slots" && g.IsActive);

            if (game is null)
                throw new Exception("No se encontró un juego de tipo 'Slot' activo en la base de datos.");

            var entity = new SlotGameConfig
            {
                Id = Guid.NewGuid(),
                GameId = game.Id, 
                MachineName = request.MachineName,
                Multiplier = request.Multiplier,
                NumberOfReels = request.NumberOfReels,
                NumberOfRows = request.NumberOfRows,
                PayLines = request.PayLines,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow.ToString()
            };

            _context.SlotGameConfigs.Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(entity.Id)
                ?? throw new Exception("Error al crear la configuración del juego de tragamonedas.");
        }

        // UPDATE
        public async Task<SlotGameConfigResponse?> UpdateAsync(Guid id, UpdateSlotGameConfigRequest request)
        {
            var entity = await _context.SlotGameConfigs.Include(x => x.Game)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            entity.MachineName = request.MachineName;
            entity.Multiplier = request.Multiplier;
            entity.NumberOfReels = request.NumberOfReels;
            entity.NumberOfRows = request.NumberOfRows;
            entity.PayLines = request.PayLines;
            entity.IsActive = request.IsActive;


            await _context.SaveChangesAsync();

            return new SlotGameConfigResponse
            {
                Id = entity.Id,
                MachineName = entity.MachineName,
                Multiplier = entity.Multiplier,
                NumberOfReels = entity.NumberOfReels,
                NumberOfRows = entity.NumberOfRows,
                PayLines = entity.PayLines,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                GameId = entity.GameId,
                GameName = entity.Game?.GameName ?? "Unknown"
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.SlotGameConfigs
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            _context.SlotGameConfigs.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

       
    }
}
