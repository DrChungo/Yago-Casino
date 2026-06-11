namespace Chaos.Shared.ResponseEntity
{
    public class RussianRouletteResponse
    {
        public Guid Id { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public int TotalChambers { get; set; }
        public int BulletCount { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public decimal FixedPrizePool { get; set; }
        public bool AllowBots { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = string.Empty;

    }
}
