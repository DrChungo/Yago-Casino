namespace Chaos.Api.ResponseEntity
{
    public class CoinGameResponse
    {
        public string AnimalName { get; set; } = string.Empty;
        public int Possibility { get; set; }
        public int Reward { get; set; }
        public bool Won { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
