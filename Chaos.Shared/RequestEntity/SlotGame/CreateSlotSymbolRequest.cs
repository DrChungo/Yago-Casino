namespace Chaos.Shared.RequestEntity.SlotGame
{
    public class CreateSlotSymbolRequest
    {
        public string SymbolName { get; set; }
        public string SymbolCode { get; set; }
        public string Rarity { get; set; }
        public decimal BaseValue { get; set; }
        public bool IsActive { get; set; }
        public Guid SlotGameConfigId { get; set; }
    }
}