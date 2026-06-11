using Chaos.Shared.ResponseEntity.SlotGame;
using Chaos.Shared.RequestEntity.SlotGame;


namespace Chaos.BlazorCasinoApp.IApiService.Slots
{
    public interface ISlotGameService
    {
        Task<List<SlotGameConfigResponse>> GetAllAsync();
        Task<SlotGameConfigResponse?> GetByIdAsync(Guid id);
         Task CreateAsync(CreateSlotGameConfigRequest request);
        Task UpdateAsync(Guid id, UpdateSlotGameConfigRequest config);
        Task DeleteAsync(Guid id);
    }
}
