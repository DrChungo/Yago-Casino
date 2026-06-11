using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;

namespace Chaos.BlazorCasinoApp.IApiService
{


        public interface IBlackjackConfigService
        {
            Task<List<BlackjackResponse>> GetAllAsync();
            Task<BlackjackResponse?> GetByIdAsync(Guid id);
            Task CreateAsync(BlackjackRequest request);
            Task UpdateAsync(Guid id, BlackjackRequest request);
            Task ToggleActiveAsync(Guid id);
            Task DeleteAsync(Guid id);
        }
    }