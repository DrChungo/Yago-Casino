namespace Chaos.Api.ResponseEntity.RussianRoulette
{
    public class RoundResultResponse
    {
        public int RoundNumber { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool WasBullet { get; set; }
        public bool IsBot { get; set; }
        public string Message { get; set; }
        public bool GameFinished { get; set; }
        public Guid? WinnerId { get; set; }
        public string? WinnerName { get; set; }
        public decimal? PrizePool { get; set; }
    }
}
