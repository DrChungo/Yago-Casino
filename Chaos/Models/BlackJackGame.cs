using Chaos.Api.Enums;
using Chaos.Api.ResponseEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chaos.Api.Models
{
    public class BlackJackGame
    {
        public Guid GameId { get; set; }
        public Guid AnimalId { get; set; }
        public Guid UserId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int AnimalValue { get; set; }
        public int Reward { get; set; }

        public BlackJackGameResultEnum Result { get; set; }
        public BlackJackGameStatusEnum Status { get; set; }
        public BlackJackHand PlayerHand { get; set; }
        public BlackJackHand DealerHand { get; set; }
        public string Message { get; set; } = string.Empty;

        public Guid GameSessionDbId { get; set; }

        //Cartas que recibe el front
        public List<CardDto> DealerCardsToShow { get; set; } = new();
        public List<CardDto> UserCardsToShow { get; set; } = new();

        public BlackJackGame()
        {
            GameId = Guid.NewGuid();
            PlayerHand = new BlackJackHand();
            DealerHand = new BlackJackHand();
            Status = BlackJackGameStatusEnum.PLAYERTURN;
            Result = BlackJackGameResultEnum.PENDING;
        }

        // GETTERS
        public int PlayerScore => PlayerHand?.Score ?? 0;
        public int DealerScore => DealerHand?.Score ?? 0;
        public bool PlayerBusted => PlayerHand?.IsBusted ?? false;
        public bool DealerBusted => DealerHand?.IsBusted ?? false;
        public bool IsFinished => Status == BlackJackGameStatusEnum.FINISHED;

        public bool CanHit => Status == BlackJackGameStatusEnum.PLAYERTURN &&
                              !PlayerBusted &&
                              PlayerScore < 21;

        public bool CanStand => Status == BlackJackGameStatusEnum.PLAYERTURN &&
                                !PlayerBusted;


    }
}
