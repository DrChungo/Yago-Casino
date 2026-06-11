namespace Chaos.Api.RequestEntity.Config.SlotGame
{
    public class UpdateSlotSymbolRequest
    {
        public string SymbolName { get; set; } = string.Empty;
        public string SymbolCode { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public decimal BaseValue { get; set; }
        public bool IsActive { get; set; }
        public Guid SlotGameConfigId { get; set; }
    }
}