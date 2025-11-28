using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing player.
/// Contains all updatable properties for player modification with validation attributes.
/// </summary>
public sealed record UpdatePlayerDto
{
    /// <summary>
    /// Gets the unique identifier of the player to update.
    /// </summary>
    [Required(ErrorMessage = "Player ID is required.")]
    public required int Id { get; init; }

    /// <summary>
    /// Gets the player's full name. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the player's date of birth. Required, must be a past date.
    /// </summary>
    [Required(ErrorMessage = "Date of birth is required.")]
    public required DateTime DateOfBirth { get; init; }

    /// <summary>
    /// Gets the player's gender. Maximum 50 characters.
    /// Valid values: Male, Female, Non-binary, Prefer not to say.
    /// </summary>
    [StringLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
    public string? Gender { get; init; }

    /// <summary>
    /// Gets the URL to the player's photo. Maximum 500 characters, must be a valid URL format.
    /// </summary>
    [StringLength(500, ErrorMessage = "Photo URL cannot exceed 500 characters.")]
    [Url(ErrorMessage = "Photo URL must be a valid URL.")]
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// Applies the updates from this DTO to an existing Player entity.
    /// </summary>
    /// <param name="updatedBy">The identifier of the user making the update.</param>
    /// <returns>A new Player entity with updated values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when updatedBy is null.</exception>
    /// <exception cref="ArgumentException">Thrown when updatedBy is empty or whitespace.</exception>
    /// <remarks>
    /// This method creates a new Player entity with the updated values.
    /// The CreatedAt and CreatedBy fields should be preserved from the original entity.
    /// </remarks>
    /// <example>
    /// <code>
    /// var updateDto = new UpdatePlayerDto
    /// {
    ///     Id = 1,
    ///     Name = "John Doe Updated",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     Gender = "Male"
    /// };
    /// // Apply to existing entity preserving audit fields
    /// var updatedPlayer = updateDto.ApplyTo(existingPlayer, "admin-user");
    /// </code>
    /// </example>
    public Player ApplyTo(Player existingPlayer, string updatedBy)
    {
        ArgumentNullException.ThrowIfNull(existingPlayer);
        ArgumentNullException.ThrowIfNull(updatedBy);

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy cannot be empty or whitespace.", nameof(updatedBy));
        }

        var player = new Player
        {
            Id = existingPlayer.Id,
            Name = Name.Trim(),
            DateOfBirth = DateOfBirth,
            Gender = Gender?.Trim(),
            PhotoUrl = PhotoUrl?.Trim(),
            CreatedAt = existingPlayer.CreatedAt,
            CreatedBy = existingPlayer.CreatedBy
        };

        player.UpdateLastModified(updatedBy);

        return player;
    }
}
