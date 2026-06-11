namespace Chaos.Api.RequestEntity.Config.RussianRoulette
{
    public class RussianRouletteUpdateRequest
    {

        public string ConfigName { get; set; }
        public int TotalChambers { get; set; }
        public int BulletCount { get; set; }
        public int MaxPlayers { get; set; }
        public int MinPlayers { get; set; }
        public decimal FixedPrizePool { get; set; }
        public bool AllowBots { get; set; }
        public bool IsActive { get; set; }
    }
}
