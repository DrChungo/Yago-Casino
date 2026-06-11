using Chaos.Api.RequestEntity.Config.RussianRoulette;
using Chaos.Api.ResponseEntity.Config.RussianRoulette;

namespace Chaos.Api.Interface.Config
{
    public interface IRussianRouletteService
    {
        Task<List<RussianRouletteGameResponse>> GetAllAsync();
        Task<RussianRouletteGameResponse?> GetByIdAsync(Guid id);
        Task<RussianRouletteGameResponse> CreateAsync(RussianRouletteCreateRequest dto);
        Task<RussianRouletteGameResponse?> UpdateAsync(Guid id, RussianRouletteUpdateRequest dto);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
