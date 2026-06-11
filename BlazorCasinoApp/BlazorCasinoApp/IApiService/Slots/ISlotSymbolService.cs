using Chaos.Shared.ResponseEntity.SlotGame;
using Chaos.Shared.RequestEntity.SlotGame;


namespace Chaos.BlazorCasinoApp.IApiService.Slots
{
    public interface ISlotSymbolService
    {
        Task<List<SlotSymbolResponse>> GetAllAsync();
        Task<SlotSymbolResponse?> GetByIdAsync(Guid id);
        Task CreateAsync(CreateSlotSymbolRequest symbol);
         Task UpdateAsync(Guid id, UpdateSlotSymbolRequest symbol);
          Task DeleteAsync(Guid id);
    }
}
