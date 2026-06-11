using Chaos.Api.RequestEntity.Config.Roulette;
using Chaos.Api.ResponseEntity;

namespace Chaos.Api.Interface.Config
{
    public interface IRouletteGameConfigService
    {
        Task<List<RouletteGameConfigResponse>> GetAllAsync();
        Task<RouletteGameConfigResponse?> GetByIdAsync(Guid id);
        Task<RouletteGameConfigResponse> CreateAsync(CreateRouletteGameConfigRequest dto);
        Task<RouletteGameConfigResponse?> UpdateAsync(Guid id, UpdateRouletteGameConfigRequest dto);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
