using Chaos.Shared.ResponseEntity.SlotGame;
using Chaos.Shared.RequestEntity.SlotGame;


namespace Chaos.BlazorCasinoApp.IApiService.Slots
{
    public interface ISlotPayoutRuleService
    {
        Task<List<SlotPayoutRuleResponse>> GetAllAsync();
        Task<SlotPayoutRuleResponse?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateSlotPayoutRuleRequest rule);
        Task UpdateAsync(Guid id, UpdateSlotPayoutRuleRequest rule);
        Task DeleteAsync(Guid id);
    }
}
