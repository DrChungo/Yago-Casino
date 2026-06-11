namespace Chaos.Shared.ResponseEntity.SlotGame
{
    public class SlotPayoutRuleResponse
    {
        public Guid Id { get; set; }
        public string Combination { get; set; } = string.Empty;
        public decimal PayoutMultiplier { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid SlotGameConfigId { get; set; }
        public string SlotGameConfigName { get; set; } = string.Empty;
        public Guid SlotSymbolId { get; set; }
        public string SlotSymbolName { get; set; } = string.Empty;
    }   
}