namespace Chaos.Api.ResponseEntity.RussianRoulette
{
    public class RussianRouletteStatusResponse
    {
        public Guid LobbyId { get; set; }
        public string LobbyCode { get; set; }
        public string Status { get; set; }
        public decimal CurrentPrizePool { get; set; }
        public int TotalPlayers { get; set; }
        public int AlivePlayers { get; set; }
        public List<PlayerStatusResponse> Players { get; set; }
        public Guid? WinnerId { get; set; }
        public string? WinnerName { get; set; }
        public bool SessionClosed { get; set; }

    }
}
