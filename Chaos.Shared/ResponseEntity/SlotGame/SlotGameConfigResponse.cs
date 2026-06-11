namespace Chaos.Shared.ResponseEntity.SlotGame
{
    public class SlotGameConfigResponse
    {

        public Guid Id { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public decimal Multiplier { get; set; }
        public int NumberOfReels { get; set; }
        public int NumberOfRows { get; set; }
        public int PayLines { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
            public string GameName { get; set; } = string.Empty;
        }
}
