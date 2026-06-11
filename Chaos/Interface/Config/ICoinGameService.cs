using Chaos.Api.ResponseEntity.Config.CoinGame;

namespace Chaos.Api.Interface.Config
{
    public interface ICoinGameService
    {
        Task<List<CoinGameResponde>> GetAllAsync();
        Task<CoinGameResponde?> GetByIdAsync(Guid id);
        Task<CoinGameResponde> CreateAsync(CoinGameCreate dto);
        Task<CoinGameResponde?> UpdateAsync(Guid id, CoinGameUpdate dto);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
