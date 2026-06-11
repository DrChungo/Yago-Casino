namespace Chaos.Shared.ResponseEntity
{
    public class HigherLowerGameResponse
    {
        public Guid Id { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public decimal BaseMultiplier { get; set; }
        public decimal RoundIncrement { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
    }
}