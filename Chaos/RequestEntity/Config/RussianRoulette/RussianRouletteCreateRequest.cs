namespace Chaos.Api.RequestEntity.Config.RussianRoulette
{
    public class RussianRouletteCreateRequest
    {
        public string ConfigName { get; set; }
        public int TotalChambers { get; set; } = 6;
        public int BulletCount { get; set; } = 2;
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public decimal FixedPrizePool { get; set; }
        public bool AllowBots { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
