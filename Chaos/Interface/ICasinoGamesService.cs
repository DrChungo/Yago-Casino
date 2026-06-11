using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;

namespace Chaos.Api.Interface
{
    public interface ICasinoGamesService
    {

        //generales

        public Task<bool> CashTransactionAsync(Guid WalletId, int AddAmount);
        public bool ExistAnimal(Guid id);
        public decimal CalculateMultiplyerByRaririty(bool v);

        //Tragaperrilla

        public Task <Tragaperras> PlayTragaPerrillas(CoinGameRequest animal);
        public List<string[][]> ConvertToJagged(List<string[,]> matrices);
        //Coin game

        public Task<CoinGame> CoinGame(Guid id, Guid playerId, string userChoice);
        public int CalculateWinPossibility(int animalValue);

        //Roullette game

        //public List<RoundEliminated> PlayRussianRoullete(List<User> russianRoulletePlayers); CAMBIADA
     //   public List<RoundEliminated> PlayRussianRoullete(List<User> russianRoulletePlayers, Guid lobbyId);

       public Task<CashBack> PlayEuropeanRoullete(SelectionUserEuropeanRoulette Bet, Guid UserId);
        MessageWarning CheckNotDuplicateAnimalAndAvailable(SelectionUserEuropeanRoulette bet, Guid userId);

        //BlackJack game

        public BlackJackResponse MapToBlackJackResponse(BlackJackGame game);
        Task DetermineWinner(BlackJackGame game);
        Task<BlackJackGame> BlackJackGame(Guid idAnimal, Guid idUser);
        Task<BlackJackGame> BlackJackHit(Guid gameId);
        Task<BlackJackGame> BlackJackStand(Guid gameId);
        BlackJackGame BlackJackGetGame(Guid gameId);

        //Higher or lower game

        public Task<HigherLowerGame> StartHigherLowerGame(Guid AnimalId, Guid UserId);
        Task<HigherLowerGame> PlayHigherLower(HigherLowerPlayRequest request);
        public HigherLowerGame GetHigherLowerGame(Guid gameId);
        public HigherLowerResponse MapToHigherLowerResponse(HigherLowerGame game);
        Task<HigherLowerGame> CashOutHigherLower(Guid gameId);
        public Task FinishHigherLowerGame(HigherLowerGame game);
        List<SlotSymbol> GetSymbolsFromDB();
    }
}
