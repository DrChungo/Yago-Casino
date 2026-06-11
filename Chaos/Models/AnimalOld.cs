using Chaos.Api.Enums;

namespace Chaos.Api.Models
{
    public class AnimalOld
    {

        /// <summary>
        /// Unique identifier of the animal.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the animal.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of animal.
        /// </summary>
        public AnimalEnum TypeAnimal { get; set; }

        /// <summary>
        /// Health of animal.
        /// </summary>
        public HealthEnum HealthStatus { get; set; }

        /// <summary>
        /// Age of animal.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Weight of animal.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Height of animal.
        /// </summary>
        public int Height { get; set; }

        public required Guid OwnerId { get; set; }

        public int Value { get; set; }

    }

}