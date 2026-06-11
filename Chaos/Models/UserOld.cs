namespace Chaos.Api.Models
{
    public class UserOld
    {
        /// <summary>
        /// Unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the user.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password of the user (maybe in the future hashed)
        /// </summary>
        public string Password { get; set; } = string.Empty;

        public bool IsAlive { get; set; } = true;
    }
}
