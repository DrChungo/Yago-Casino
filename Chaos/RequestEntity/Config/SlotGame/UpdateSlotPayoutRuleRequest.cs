namespace Chaos.Api.RequestEntity.Config.SlotGame
{
    public class UpdateSlotPayoutRuleRequest
    {
        public string Combination { get; set; } = string.Empty;
        public decimal PayoutMultiplier { get; set; }
        public bool IsActive { get; set; }
        public Guid SlotGameConfigId { get; set; }
        public Guid SlotSymbolId { get; set; }
    }
}