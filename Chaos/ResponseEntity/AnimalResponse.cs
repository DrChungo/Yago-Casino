using Chaos.Api.Enums;
using Chaos.Api.Models;
using System;

public class AnimalResponse
{

    /// <summary>
    /// Gets or sets the unique identifier of the animal.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the animal.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the animal.
    /// </summary>
    public String TypeAnimal { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age of the animal.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the weight of the animal.
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Gets or sets the height of the animal.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the health status of the animal.
    /// </summary>
    public HealthEnum Health { get; set; }

    /// <summary>
    /// Gets or sets the calculated value of the animal.
    /// </summary>
    public int Value { get; set; }
    public Guid? OwnerId { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? Rarity { get; set; }
}
