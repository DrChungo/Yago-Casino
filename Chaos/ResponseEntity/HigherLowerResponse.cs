using Chaos.Api.Enums;
using Chaos.Api.Models;

namespace Chaos.Api.ResponseEntity
{
    public class HigherLowerResponse
    {
        public Guid GameId { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int AnimalValue { get; set; }
        public int Reward { get; set; }
        public bool GameEnded { get; set; }
        public Card? CurrentCard { get; set; }
        public string Message { get; set; } = string.Empty;
        public HigherOrLowerChoiceEnum? Choice { get; set; }
    }
}
