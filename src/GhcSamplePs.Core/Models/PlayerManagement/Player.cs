namespace GhcSamplePs.Core.Models.PlayerManagement;

/// <summary>
/// Represents a player in the system with personal information and audit tracking.
/// This is a domain entity that encapsulates player data and behavior.
/// </summary>
public sealed class Player
{
    /// <summary>
    /// Gets or sets the unique identifier for the player.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets the identifier of the user who owns this player.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets the player's full name. Maximum 200 characters.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the player's date of birth. Must be a past date.
    /// </summary>
    public required DateTime DateOfBirth { get; init; }

    /// <summary>
    /// Gets the player's gender. Maximum 50 characters.
    /// Valid values: Male, Female, Non-binary, Prefer not to say.
    /// </summary>
    public string? Gender { get; init; }

    /// <summary>
    /// Gets the URL to the player's photo. Maximum 500 characters.
    /// Must be a valid URL format when provided.
    /// </summary>
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the identifier of the user who created this player.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who last updated this player.
    /// </summary>
    public string? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets the player's current age calculated from their date of birth.
    /// </summary>
    public int Age => CalculateAge();

    /// <summary>
    /// Calculates the player's current age based on their date of birth and today's date.
    /// Correctly handles birthdays that have not yet occurred this year and leap year birthdays.
    /// </summary>
    /// <returns>The player's age in years.</returns>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     CreatedBy = "system"
    /// };
    /// int age = player.CalculateAge(); // Returns current age
    /// </code>
    /// </example>
    public int CalculateAge()
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - DateOfBirth.Year;

        // If the birthday has not occurred yet this year, subtract one year
        if (DateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    /// <summary>
    /// Validates the player entity against all business rules.
    /// </summary>
    /// <returns>True if the entity is valid; otherwise, false.</returns>
    /// <remarks>
    /// Validation rules:
    /// - UserId is required
    /// - Name is required and cannot exceed 200 characters
    /// - DateOfBirth must be a past date
    /// - Gender, if provided, cannot exceed 50 characters
    /// - PhotoUrl, if provided, cannot exceed 500 characters and must be a valid URL
    /// - CreatedBy is required
    /// </remarks>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     CreatedBy = "system"
    /// };
    /// bool isValid = player.Validate(); // Returns true
    /// </code>
    /// </example>
    public bool Validate()
    {
        // UserId validation
        if (string.IsNullOrWhiteSpace(UserId))
        {
            return false;
        }

        // Name validation
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 200)
        {
            return false;
        }

        // DateOfBirth must be in the past
        if (DateOfBirth >= DateTime.UtcNow.Date)
        {
            return false;
        }

        // Gender validation (optional, but max 50 chars if provided)
        if (Gender is not null && Gender.Trim().Length > 50)
        {
            return false;
        }

        // PhotoUrl validation (optional, but max 500 chars and valid URL if provided)
        if (PhotoUrl is not null)
        {
            var trimmedUrl = PhotoUrl.Trim();
            if (trimmedUrl.Length > 500)
            {
                return false;
            }

            if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return false;
            }
        }

        // CreatedBy validation
        if (string.IsNullOrWhiteSpace(CreatedBy))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the last modified audit fields with the specified user identifier.
    /// </summary>
    /// <param name="userId">The identifier of the user making the update.</param>
    /// <exception cref="ArgumentException">Thrown when userId is null, empty, or whitespace.</exception>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     CreatedBy = "system"
    /// };
    /// player.UpdateLastModified("admin-user");
    /// // player.UpdatedAt and player.UpdatedBy are now set
    /// </code>
    /// </example>
    public void UpdateLastModified(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null, empty, or whitespace.", nameof(userId));
        }

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}
