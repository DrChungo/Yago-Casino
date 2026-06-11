using Chaos.Api.Interface.Config;
using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service.Config
{
    public class SlotPayoutRuleService : ISlotPayoutRuleService
    {
        private readonly CasinoDBContext _context;

        public SlotPayoutRuleService(CasinoDBContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<SlotPayoutRuleResponse>> GetAllAsync()
        {
            return await _context.SlotPayoutRules
                .Include(x => x.SlotGameConfig)
                .Include(x => x.SlotSymbol)
                .Select(x => new SlotPayoutRuleResponse
                {
                    Id = x.Id,
                    Combination = x.Combination,
                    PayoutMultiplier = x.PayoutMultiplier,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    SlotGameConfigId = x.SlotGameConfigId,
                    SlotGameConfigName = x.SlotGameConfig.MachineName,
                    SlotSymbolId = x.SlotSymbolId,
                    SlotSymbolName = x.SlotSymbol.SymbolName
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<SlotPayoutRuleResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _context.SlotPayoutRules
                .Include(x => x.SlotGameConfig)
                .Include(x => x.SlotSymbol)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            return new SlotPayoutRuleResponse
            {
                Id = entity.Id,
                Combination = entity.Combination,
                PayoutMultiplier = entity.PayoutMultiplier,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = entity.SlotGameConfig?.MachineName ?? "Unknown",
                SlotSymbolId = entity.SlotSymbolId,
                SlotSymbolName = entity.SlotSymbol?.SymbolName ?? "Unknown"
            };
        }

        // CREATE
        public async Task<SlotPayoutRuleResponse> CreateAsync(CreateSlotPayoutRuleRequest request)
        {
            var entity = new SlotPayoutRule
            {
                Id = Guid.NewGuid(),
                Combination = request.Combination,
                PayoutMultiplier = request.PayoutMultiplier,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow.ToString(),
                SlotGameConfigId = request.SlotGameConfigId,
                SlotSymbolId = request.SlotSymbolId
            };

            _context.SlotPayoutRules.Add(entity);
            await _context.SaveChangesAsync();

            var slotGameConfig = await _context.SlotGameConfigs.FindAsync(entity.SlotGameConfigId);
            var slotSymbol = await _context.SlotSymbols.FindAsync(entity.SlotSymbolId);

            return new SlotPayoutRuleResponse
            {
                Id = entity.Id,
                Combination = entity.Combination,
                PayoutMultiplier = entity.PayoutMultiplier,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = slotGameConfig?.MachineName ?? "Unknown",
                SlotSymbolId = entity.SlotSymbolId,
                SlotSymbolName = slotSymbol?.SymbolName ?? "Unknown"
            };
        }

        // UPDATE
        public async Task<SlotPayoutRuleResponse?> UpdateAsync(Guid id, UpdateSlotPayoutRuleRequest request)
        {
            var entity = await _context.SlotPayoutRules
                .Include(x => x.SlotGameConfig)
                .Include(x => x.SlotSymbol)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return null;

            entity.Combination = request.Combination;
            entity.PayoutMultiplier = request.PayoutMultiplier;
            entity.IsActive = request.IsActive;
            entity.SlotGameConfigId = request.SlotGameConfigId;
            entity.SlotSymbolId = request.SlotSymbolId;

            await _context.SaveChangesAsync();

            return new SlotPayoutRuleResponse
            {
                Id = entity.Id,
                Combination = entity.Combination,
                PayoutMultiplier = entity.PayoutMultiplier,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                SlotGameConfigId = entity.SlotGameConfigId,
                SlotGameConfigName = entity.SlotGameConfig?.MachineName ?? "Unknown",
                SlotSymbolId = entity.SlotSymbolId,
                SlotSymbolName = entity.SlotSymbol?.SymbolName ?? "Unknown"
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.SlotPayoutRules
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            _context.SlotPayoutRules.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // UPDATE STATUS
        public async Task<bool> UpdateStatusAsync(Guid id, bool isActive)
        {
            var entity = await _context.SlotPayoutRules
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return false;

            entity.IsActive = isActive;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}