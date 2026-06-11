namespace Chaos.Api.ResponseEntity.Config.Slots
{
    public class SlotGameConfigResponse
    {

        public Guid Id { get; set; }
        public string MachineName { get; set; }
        public decimal Multiplier { get; set; }
        public int NumberOfReels { get; set; }
        public int NumberOfRows { get; set; }
        public int PayLines { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; }
    }
}
