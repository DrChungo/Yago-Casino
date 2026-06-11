

namespace Chaos.Shared.ResponseEntity
{
    public class CoinGameResponse
    {
        public Guid Id { get; set; } 
        public string ConfigName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string CreatedAt { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public string GameName { get; set; } = string.Empty;// viene del join con Games
    }
}
