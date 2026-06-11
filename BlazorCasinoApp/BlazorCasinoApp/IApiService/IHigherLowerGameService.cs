using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;

namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface IHigherLowerGameService
    {
        Task<List<HigherLowerGameResponse>> GetAllAsync();
        Task<HigherLowerGameResponse?> GetByIdAsync(Guid id);
        Task CreateAsync(HigherLowerGameRequest request);
        Task UpdateAsync(Guid id, HigherLowerGameRequest request);
        Task ToggleActiveAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}