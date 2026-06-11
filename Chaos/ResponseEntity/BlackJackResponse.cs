using Chaos.Api.Enums;
using Chaos.Api.Models;

namespace Chaos.Api.ResponseEntity
{
    public class BlackJackResponse
    {
        public Guid GameId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int Reward { get; set; }
        public string Message { get; set; } = string.Empty;
        public BlackJackGameResultEnum? Result { get; set; }
        public int PlayerScore { get; set; }
        public int DealerScore { get; set; }
        public bool IsFinished { get; set; }
        public List<CardDto> DealerCards { get; set; } = new();
        public List<CardDto> UserCards { get; set; } = new();
    }
}
