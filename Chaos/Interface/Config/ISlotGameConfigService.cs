using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;

namespace Chaos.Api.Interface.Config
{
    public interface ISlotGameConfigService
    {
        public Task<IEnumerable<SlotGameConfigResponse>> GetAllAsync();
        public Task<SlotGameConfigResponse?> GetByIdAsync(Guid id);
        public Task<SlotGameConfigResponse> CreateAsync(CreateSlotGameConfigRequest request);
        public Task<SlotGameConfigResponse?> UpdateAsync(Guid id, UpdateSlotGameConfigRequest request);
        public Task<bool> DeleteAsync(Guid id);
      
    }
}
