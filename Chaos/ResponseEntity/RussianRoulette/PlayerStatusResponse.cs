namespace Chaos.Api.ResponseEntity.RussianRoulette
{
    public class PlayerStatusResponse
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public bool IsBot { get; set; }
        public int TurnOrder { get; set; }
        public bool IsWinner { get; set; }
    }
}
