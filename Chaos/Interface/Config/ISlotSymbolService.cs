using Chaos.Api.RequestEntity.Config.SlotGame;
using Chaos.Api.ResponseEntity.Config.Slots;

namespace Chaos.Api.Interface.Config
{
    public interface ISlotSymbolService
    {
        Task<IEnumerable<SlotSymbolResponse>> GetAllAsync();
        Task<SlotSymbolResponse?> GetByIdAsync(Guid id);
        Task<SlotSymbolResponse> CreateAsync(CreateSlotSymbolRequest request);
        Task<SlotSymbolResponse?> UpdateAsync(Guid id, UpdateSlotSymbolRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusAsync(Guid id, bool isActive);
    }
}