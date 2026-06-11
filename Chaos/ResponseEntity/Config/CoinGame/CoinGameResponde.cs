namespace Chaos.Api.ResponseEntity.Config.CoinGame
{
    public class CoinGameResponde
    {
        public Guid Id { get; set; }
        public string ConfigName { get; set; }
        public bool IsActive { get; set; }

        public string CreatedAt { get; set; }
        public Guid GameId { get; set; }
        public string GameName { get; set; } // viene del join con Games
    }
}
