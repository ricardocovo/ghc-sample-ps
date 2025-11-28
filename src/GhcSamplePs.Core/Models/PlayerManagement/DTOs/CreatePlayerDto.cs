using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for creating a new player.
/// Contains all required properties for player creation with validation attributes.
/// </summary>
public sealed record CreatePlayerDto
{
    /// <summary>
    /// Gets the identifier of the user who owns this player. Required.
    /// </summary>
    [Required(ErrorMessage = "User ID is required.")]
    public required string UserId { get; init; }

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
    /// Creates a Player entity from this DTO.
    /// </summary>
    /// <param name="createdBy">The identifier of the user creating the player.</param>
    /// <returns>A new Player entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when createdBy is null.</exception>
    /// <exception cref="ArgumentException">Thrown when createdBy is empty or whitespace.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerDto
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     Gender = "Male"
    /// };
    /// var player = createDto.ToEntity("admin-user");
    /// </code>
    /// </example>
    public Player ToEntity(string createdBy)
    {
        ArgumentNullException.ThrowIfNull(createdBy);

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy cannot be empty or whitespace.", nameof(createdBy));
        }

        return new Player
        {
            UserId = UserId.Trim(),
            Name = Name.Trim(),
            DateOfBirth = DateOfBirth,
            Gender = Gender?.Trim(),
            PhotoUrl = PhotoUrl?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
