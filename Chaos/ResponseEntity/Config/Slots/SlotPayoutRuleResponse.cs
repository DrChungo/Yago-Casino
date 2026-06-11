namespace Chaos.Api.ResponseEntity.Config.Slots
{
    public class SlotPayoutRuleResponse
    {
        public Guid Id { get; set; }
        public string Combination { get; set; }
        public decimal PayoutMultiplier { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid SlotGameConfigId { get; set; }
        public string SlotGameConfigName { get; set; }
        public Guid SlotSymbolId { get; set; }
        public string SlotSymbolName { get; set; }
    }
}