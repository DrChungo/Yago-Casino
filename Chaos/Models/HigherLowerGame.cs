using Chaos.Api.Enums;

namespace Chaos.Api.Models
{
    public class HigherLowerGame
    {
        public Guid GameId { get; set; }
        public Guid AnimalId { get; set; }
        public Guid UserId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int AnimalValue { get; set; }
        public int Reward { get; set; }
        public bool GameEnded { get; set; }
        public Card? CurrentCard { get; set; } 
        public Card? NextCard { get; set; }
        public bool HigherOrLowerChoice { get; set; }
        public string Message { get; set; } = string.Empty;
        public HigherOrLowerChoiceEnum? Choice { get; set; }
        public Guid GameSessionId {  get; set; }
        public int RoundsPLayed { get; set; }

        public HigherLowerGame()
        {
            GameId = Guid.NewGuid();
            GameEnded = false;
        }

    }
}
