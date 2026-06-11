
using Chaos.Shared.ResponseEntity;
using Chaos.Shared.RequestEntity;


namespace Chaos.BlazorCasinoApp.IApiService
{
    public interface ICoinGameService
    {
        public Task<List<CoinGameResponse>> GetAllAsync();
        Task<CoinGameResponse> GetByIdAsync(Guid id);
        Task CreateAsync(CoinGameRequest request);
        Task UpdateAsync(Guid id, CoinGameRequest request);
        Task DeleteAsync(Guid id);

        //Específicos del juego
        Task ToggleActiveAsync(Guid id);  // Activar/Desactivar config
        Task<CoinGameResponse> GetActiveAsync();
    }
}
