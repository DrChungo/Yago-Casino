namespace Chaos.Api.Models
{
    public class Wallet
    {
        /// <summary>
        /// Unique identifier of the wallet and user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Amount total for the user.
        /// </summary>
        public int Amount { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
