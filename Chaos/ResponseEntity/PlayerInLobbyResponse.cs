namespace Chaos.Api.ResponseEntity
{
	public class PlayerInLobbyResponse
	{
		public Guid PlayerId { get; set; }
		public Guid? UserId { get; set; }
		public bool IsBot { get; set; }
		public string? BotName { get; set; }
		public bool IsAlive { get; set; }
		public bool IsWinner { get; set; }
		public int TurnOrder { get; set; }
		public string? JoinedAt { get; set; }
		public bool IsMaster { get; set; }
	}
}