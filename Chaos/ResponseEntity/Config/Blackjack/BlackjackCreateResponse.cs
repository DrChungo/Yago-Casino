namespace Chaos.Api.ResponseEntity.Config.Blackjack
{
    public class BlackjackCreateResponse
    {
        public string TableName { get; set; }
        public int NumberOfDecks { get; set; } = 6;
        public int MaxPlayers { get; set; } = 6;
        public int DealerStandsOn { get; set; } = 17;
        public decimal BlackjackPayout { get; set; } = 1.50m;
        public bool AllowDoubleDown { get; set; } = true;
        public bool AllowInsurance { get; set; } = true;
        public bool IsActive { get; set; } = true;

    }
}
