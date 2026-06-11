namespace Chaos.Api.Models
{
    public class CoinGame
    {
        
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }

        public string AnimalName { get; set; } = string.Empty;
        public int AnimalValue { get; set; }
        public int Possibility { get; set; }
        public int Reward { get; set; }
        public bool Won { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Multiplyer { get; set; }
    }
}