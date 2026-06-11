using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;

namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface IRouletteGameConfigService
    {
        Task<List<RouletteGameConfigResponse>> GetAllAsync();
        Task<RouletteGameConfigResponse?> GetByIdAsync(Guid id);
        Task CreateAsync(RouletteGameConfigRequest request);
        Task UpdateAsync(Guid id, RouletteGameConfigRequest request);
        Task ToggleActiveAsync(Guid id);
        Task DeleteAsync(Guid id);


    }
}
