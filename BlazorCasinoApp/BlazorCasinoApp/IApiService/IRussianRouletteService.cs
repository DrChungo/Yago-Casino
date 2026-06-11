
using Chaos.Shared.RequestEntity.Config.RussianRoulette;
using Chaos.Shared.ResponseEntity;

namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface IRussianRouletteService
    {
        Task<List<RussianRouletteResponse>> GetAllAsync();
        Task<RussianRouletteResponse?> GetByIdAsync(Guid id);
        Task CreateAsync(RussianRouletteRequest request);
        Task UpdateAsync(Guid id, RussianRouletteRequest request);
        Task ToggleActiveAsync(Guid id);
        Task DeleteAsync(Guid id);

    }
}
