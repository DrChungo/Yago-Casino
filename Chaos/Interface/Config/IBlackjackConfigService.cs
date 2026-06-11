using Chaos.Api.ResponseEntity.Config.Blackjack;

namespace Chaos.Api.Interface.Config
{
    public interface IBlackjackConfigService
    {
        Task<List<BlackjackConfigResponse>> GetAllAsync();
        Task<BlackjackConfigResponse?> GetByIdAsync(Guid id);
        Task<BlackjackConfigResponse> CreateAsync(BlackjackCreateResponse dto);
        Task<BlackjackConfigResponse?> UpdateAsync(Guid id, BlackjackUpdateResponse dto);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
