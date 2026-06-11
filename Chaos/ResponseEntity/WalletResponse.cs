namespace Chaos.Api.ResponseEntity
{
    public class WalletResponse
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Amount total for the user.
        /// </summary>
        public int Amount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
