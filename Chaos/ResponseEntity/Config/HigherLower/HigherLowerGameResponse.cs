namespace Chaos.Api.ResponseEntity.Config.HigherLower
{
    public class HigherLowerGameResponse
    {
        public Guid Id { get; set; }
        public string ConfigName { get; set; }
        public decimal BaseMultiplier { get; set; }
        public decimal RoundIncrement { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; } // viene del join con Games


    }
}
