using Chaos.Api.RequestEntity.Config.HigherLower;
using Chaos.Api.ResponseEntity.Config.HigherLower;

namespace Chaos.Api.Interface.Config
{
    public interface IHigherLowerGameService
    {

             Task<List<HigherLowerGameResponse>> GetAllAsync();
        Task<HigherLowerGameResponse?> GetByIdAsync(Guid id);
        Task<HigherLowerGameResponse> CreateAsync(HigherLowerCreateRequest dto);
        Task<HigherLowerGameResponse?> UpdateAsync(Guid id, HigherLowerUpdateRequest dto);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
