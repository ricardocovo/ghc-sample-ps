namespace GhcSamplePs.Core.Models.PlayerManagement;

/// <summary>
/// Represents a player's membership in a team for a specific championship.
/// This is a domain entity that tracks when a player joins and leaves a team.
/// </summary>
public sealed class TeamPlayer
{
    /// <summary>
    /// Gets or sets the unique identifier for the team player record.
    /// </summary>
    public int TeamPlayerId { get; set; }

    /// <summary>
    /// Gets the foreign key to the associated Player entity.
    /// </summary>
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the name of the team. Maximum 200 characters.
    /// </summary>
    public required string TeamName { get; init; }

    /// <summary>
    /// Gets the name of the championship. Maximum 200 characters.
    /// </summary>
    public required string ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date when the player joined the team.
    /// </summary>
    public required DateTime JoinedDate { get; init; }

    /// <summary>
    /// Gets or sets the date when the player left the team.
    /// Null indicates the player is still active on the team.
    /// </summary>
    public DateTime? LeftDate { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the player is currently active on the team.
    /// Returns true when LeftDate is null.
    /// </summary>
    public bool IsActive => LeftDate is null;

    /// <summary>
    /// Gets the UTC timestamp when the team player record was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the identifier of the user who created this team player record.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the team player record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who last updated this team player record.
    /// </summary>
    public string? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated Player entity.
    /// </summary>
    public Player? Player { get; set; }

    /// <summary>
    /// Validates the team player entity against all business rules.
    /// </summary>
    /// <returns>True if the entity is valid; otherwise, false.</returns>
    /// <remarks>
    /// Validation rules:
    /// - TeamName is required and cannot exceed 200 characters
    /// - ChampionshipName is required and cannot exceed 200 characters
    /// - JoinedDate is required
    /// - LeftDate, if provided, must be after JoinedDate
    /// - LeftDate cannot be in the future
    /// - CreatedBy is required
    /// </remarks>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// bool isValid = teamPlayer.Validate(); // Returns true
    /// </code>
    /// </example>
    public bool Validate()
    {
        // TeamName validation
        if (string.IsNullOrWhiteSpace(TeamName) || TeamName.Trim().Length > 200)
        {
            return false;
        }

        // ChampionshipName validation
        if (string.IsNullOrWhiteSpace(ChampionshipName) || ChampionshipName.Trim().Length > 200)
        {
            return false;
        }

        // JoinedDate must be valid (not default)
        if (JoinedDate == default)
        {
            return false;
        }

        // LeftDate validation
        if (LeftDate.HasValue)
        {
            // LeftDate must be after JoinedDate
            if (LeftDate.Value <= JoinedDate)
            {
                return false;
            }

            // LeftDate cannot be in the future
            if (LeftDate.Value > DateTime.UtcNow)
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
    /// Marks the player as having left the team by setting the LeftDate and updating audit fields.
    /// </summary>
    /// <param name="leftDate">The date when the player left the team.</param>
    /// <param name="userId">The identifier of the user making this update.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when userId is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when leftDate is not after JoinedDate, is in the future, or player is already marked as left.
    /// </exception>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// teamPlayer.MarkAsLeft(new DateTime(2024, 6, 30), "admin-user");
    /// // teamPlayer.LeftDate is now set, IsActive returns false
    /// </code>
    /// </example>
    public void MarkAsLeft(DateTime leftDate, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null, empty, or whitespace.", nameof(userId));
        }

        if (LeftDate.HasValue)
        {
            throw new InvalidOperationException("Player has already left the team.");
        }

        if (leftDate <= JoinedDate)
        {
            throw new InvalidOperationException("Left date must be after the joined date.");
        }

        if (leftDate > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Left date cannot be in the future.");
        }

        LeftDate = leftDate;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }

    /// <summary>
    /// Returns whether the player is currently active on the team.
    /// </summary>
    /// <returns>True if the player is currently active (LeftDate is null); otherwise, false.</returns>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// bool isActive = teamPlayer.IsCurrentlyActive(); // Returns true
    /// </code>
    /// </example>
    public bool IsCurrentlyActive() => LeftDate is null;

    /// <summary>
    /// Updates the last modified audit fields with the specified user identifier.
    /// </summary>
    /// <param name="userId">The identifier of the user making the update.</param>
    /// <exception cref="ArgumentException">Thrown when userId is null, empty, or whitespace.</exception>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// teamPlayer.UpdateLastModified("admin-user");
    /// // teamPlayer.UpdatedAt and teamPlayer.UpdatedBy are now set
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
