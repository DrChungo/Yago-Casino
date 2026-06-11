namespace Chaos.Api.ResponseEntity.Config.Slots
{
    public class SlotSymbolResponse
    {
        public Guid Id { get; set; }
        public string SymbolName { get; set; }
        public string SymbolCode { get; set; }
        public string Rarity { get; set; }
        public decimal BaseValue { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid SlotGameConfigId { get; set; }
        public string SlotGameConfigName { get; set; }
    }
}