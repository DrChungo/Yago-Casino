namespace Chaos.Api.ResponseEntity
{
    public class RouletteGameConfigResponse
    {
        public Guid Id { get; set; }
        public string TableName { get; set; }
        public string RouletteType { get; set; }
        public bool HasZero { get; set; }
        public bool HasDoubleZero { get; set; }
        public int TotalNumbers { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; } // viene del join con Games
    }
}
