namespace Chaos.Shared.ResponseEntity
{
    public class BlackjackResponse
    {
        public Guid Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int NumberOfDecks { get; set; }
        public int MaxPlayers { get; set; }
        public int DealerStandsOn { get; set; }
        public decimal BlackjackPayout { get; set; }
        public bool AllowDoubleDown { get; set; }
        public bool AllowInsurance { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
    }
}