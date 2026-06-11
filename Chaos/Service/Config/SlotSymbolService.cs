using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class SlotSymbolService : ISlotSymbolService
    {
        private readonly CasinoDBContext _context;

        public SlotSymbolService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<SlotSymbolResponse>> GetAllAsync()
        {
            return await _context.SlotSymbols
                .Include(x => x.SlotGameConfig)
                .Select(x => new SlotSymbolResponse
                {
                    Id = x.Id,
                    SymbolName = x.SymbolName,
                    SymbolCode = x.SymbolCode,
                    Rarity = x.Rarity,
                    BaseValue = x.BaseValue,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    SlotGameConfigId = x.SlotGameConfigId,
                    SlotGameConfigName = x.SlotGameConfig.MachineName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<SlotSymbolResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _context.SlotSymbols
                .Include(x => x.SlotGameConfig)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new SlotSymbolResponse
            {
                Id = entity.Id,
                SymbolName = entity.SymbolName,
                SymbolCode = entity.SymbolCode,
                Rarity = entity.Rarity,
                BaseValue = entity.BaseValue,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = entity.SlotGameConfig?.MachineName ?? "Unknown"
            };
        }

        // CREATE
        public async Task<SlotSymbolResponse> CreateAsync(CreateSlotSymbolRequest request)
        {
         
            var slotGameConfig = await _context.SlotGameConfigs.FindAsync(request.SlotGameConfigId);
            if (slotGameConfig == null)
                throw new ArgumentException($"SlotGameConfig with Id '{request.SlotGameConfigId}' not found.");

            var entity = new SlotSymbol
            {
                Id = Guid.NewGuid(),
                SymbolName = request.SymbolName,
                SymbolCode = request.SymbolCode,
                Rarity = request.Rarity,
                BaseValue = request.BaseValue,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow.ToString(),
                SlotGameConfigId = request.SlotGameConfigId
            };

            _context.SlotSymbols.Add(entity);
            await _context.SaveChangesAsync();

            return new SlotSymbolResponse
            {
                Id = entity.Id,
                SymbolName = entity.SymbolName,
                SymbolCode = entity.SymbolCode,
                Rarity = entity.Rarity,
                BaseValue = entity.BaseValue,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = slotGameConfig.MachineName
            };
        }

        // UPDATE
        public async Task<SlotSymbolResponse?> UpdateAsync(Guid id, UpdateSlotSymbolRequest request)
        {
            var entity = await _context.SlotSymbols
                .Include(x => x.SlotGameConfig)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

     
            var slotGameConfig = await _context.SlotGameConfigs.FindAsync(request.SlotGameConfigId);
            if (slotGameConfig == null)
                throw new ArgumentException($"SlotGameConfig with Id '{request.SlotGameConfigId}' not found.");

            entity.SymbolName = request.SymbolName;
            entity.SymbolCode = request.SymbolCode;
            entity.Rarity = request.Rarity;
            entity.BaseValue = request.BaseValue;
            entity.IsActive = request.IsActive;
            entity.SlotGameConfigId = request.SlotGameConfigId;

            await _context.SaveChangesAsync();

            return new SlotSymbolResponse
            {
                Id = entity.Id,
                SymbolName = entity.SymbolName,
                SymbolCode = entity.SymbolCode,
                Rarity = entity.Rarity,
                BaseValue = entity.BaseValue,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = slotGameConfig.MachineName
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.SlotSymbols
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            _context.SlotSymbols.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // UPDATE STATUS
        public async Task<bool> UpdateStatusAsync(Guid id, bool isActive)
        {
            var entity = await _context.SlotSymbols
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            entity.IsActive = isActive;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}