namespace Chaos.Api.RequestEntity
{
    public class CoinGameRequest
    {
        public string GameName { get; set; } = String.Empty;
        public Guid AnimalId { get; set; }
        public string HeadOrTail { get; set; } = string.Empty;

    }
}
