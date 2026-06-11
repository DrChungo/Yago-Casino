using Chaos.Api.ResponseEntity;

namespace Chaos.Api.Interface
{
    public interface IRankingService
    {
        Task<(bool success, string message, List<AnimalRankingResponse> data)> BestThreeAnimals();
        Task<(bool success, string message, List<UserWalletRankingResponse> data)> BestThreeWallets();
        Task<(bool success, string message, List<UserTotalAnimalValueResponse> data)> BestThreeTotalAnimalValues();
    }
}
