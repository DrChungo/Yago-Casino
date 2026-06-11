namespace Chaos.Api.ResponseEntity.Config.Blackjack
{
    public class BlackjackConfigResponse
    {
        public Guid Id { get; set; }
        public string TableName { get; set; }
        public int NumberOfDecks { get; set; }
        public int MaxPlayers { get; set; }
        public int DealerStandsOn { get; set; }
        public decimal BlackjackPayout { get; set; }
        public bool AllowDoubleDown { get; set; }
        public bool AllowInsurance { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; } 

    }
}
