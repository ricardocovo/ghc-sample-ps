namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying player information.
/// Contains all player properties including computed age for read operations.
/// </summary>
public sealed record PlayerDto
{
    /// <summary>
    /// Gets the unique identifier for the player.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets the identifier of the user who owns this player.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the player's full name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the player's date of birth.
    /// </summary>
    public required DateTime DateOfBirth { get; init; }

    /// <summary>
    /// Gets the player's calculated age in years.
    /// </summary>
    public required int Age { get; init; }

    /// <summary>
    /// Gets the player's gender.
    /// </summary>
    public string? Gender { get; init; }

    /// <summary>
    /// Gets the URL to the player's photo.
    /// </summary>
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the identifier of the user who created this player.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the identifier of the user who last updated this player.
    /// </summary>
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// Creates a PlayerDto from a Player entity.
    /// </summary>
    /// <param name="player">The player entity to map from.</param>
    /// <returns>A new PlayerDto instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     Id = 1,
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     CreatedBy = "system"
    /// };
    /// var dto = PlayerDto.FromEntity(player);
    /// </code>
    /// </example>
    public static PlayerDto FromEntity(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        return new PlayerDto
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = player.Name.Trim(),
            DateOfBirth = player.DateOfBirth,
            Age = player.Age,
            Gender = player.Gender?.Trim(),
            PhotoUrl = player.PhotoUrl?.Trim(),
            CreatedAt = player.CreatedAt,
            CreatedBy = player.CreatedBy,
            UpdatedAt = player.UpdatedAt,
            UpdatedBy = player.UpdatedBy
        };
    }
}
