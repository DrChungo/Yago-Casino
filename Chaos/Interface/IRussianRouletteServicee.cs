using Chaos.Api.ResponseEntity.RussianRoulette;
using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{
    public interface IRussianRouletteServicee
    {
        Task<RussianRouletteStatusResponse> StartGame(Guid lobbyId);
        Task<RoundResultResponse> PlayRound(Guid lobbyId);
        Task<RussianRouletteStatusResponse> GetStatus(Guid lobbyId);

        //nuevo añadido el 01/06/2026
        Task<List<RussianRouletteRound>> GetRoundHistory(Guid lobbyId);

    }
}
