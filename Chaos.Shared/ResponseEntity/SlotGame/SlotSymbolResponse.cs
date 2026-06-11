namespace Chaos.Shared.ResponseEntity.SlotGame
{
    public class SlotSymbolResponse
    {
        public Guid Id { get; set; }
        public string SymbolName { get; set; } = string.Empty;
        public string SymbolCode { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public decimal BaseValue { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid SlotGameConfigId { get; set; }
        public string SlotGameConfigName { get; set; } = string.Empty;
    }
}