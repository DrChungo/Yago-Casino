namespace Chaos.Api.RequestEntity.Config.HigherLower
{
    public class HigherLowerCreateRequest
    {
        public string ConfigName { get; set; }
        public decimal BaseMultiplier { get; set; }
        public decimal RoundIncrement { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
