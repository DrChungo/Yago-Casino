using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;

namespace Chaos.Api.Interface.Config
{
    public interface ISlotPayoutRuleService
    {
        Task<IEnumerable<SlotPayoutRuleResponse>> GetAllAsync();
        Task<SlotPayoutRuleResponse?> GetByIdAsync(Guid id);
        Task<SlotPayoutRuleResponse> CreateAsync(CreateSlotPayoutRuleRequest request);
        Task<SlotPayoutRuleResponse?> UpdateAsync(Guid id, UpdateSlotPayoutRuleRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, bool isActive);
    }
}