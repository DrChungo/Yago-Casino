namespace Chaos.Shared.RequestEntity
{
    public class HigherLowerGameRequest
    {
        public string ConfigName { get; set; } = string.Empty;
        public decimal BaseMultiplier { get; set; }
        public decimal RoundIncrement { get; set; }
        public bool IsActive { get; set; } = true;
    }
}