namespace Chaos.Shared.RequestEntity.Config.RussianRoulette
{
    public class RussianRouletteRequest
    {
        public string ConfigName { get; set; } = string.Empty;
        public int TotalChambers { get; set; } = 6;
        public int BulletCount { get; set; } = 2;
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public decimal FixedPrizePool { get; set; }
        public bool AllowBots { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
