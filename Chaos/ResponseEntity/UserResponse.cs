namespace Chaos.Api.ResponseEntity
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Wallet { get; set; }
        public bool IsAlive { get; set; } = true;
        public bool IsAdmin { get; set; }
    }
}
