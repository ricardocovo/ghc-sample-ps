namespace GhcSamplePs.Core.Models.PlayerManagement;

/// <summary>
/// Represents a player's performance statistics for a single game.
/// This is a domain entity that tracks game-level performance data for a team player.
/// </summary>
/// <remarks>
/// <para>
/// This entity stores individual game performance metrics including:
/// - Playing time (minutes, starter status)
/// - Scoring data (goals, assists)
/// - Jersey number for the game
/// </para>
/// <example>
/// <code>
/// var statistic = new PlayerStatistic
/// {
///     TeamPlayerId = 1,
///     GameDate = new DateTime(2024, 3, 15),
///     MinutesPlayed = 90,
///     IsStarter = true,
///     JerseyNumber = 10,
///     Goals = 2,
///     Assists = 1,
///     CreatedBy = "system"
/// };
/// </code>
/// </example>
/// </remarks>
public sealed class PlayerStatistic
{
    /// <summary>
    /// Gets or sets the unique identifier for the player statistic record.
    /// </summary>
    public int PlayerStatisticId { get; set; }

    /// <summary>
    /// Gets the foreign key to the associated TeamPlayer entity.
    /// </summary>
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the date of the game for which statistics are recorded.
    /// </summary>
    public required DateTime GameDate { get; init; }

    /// <summary>
    /// Gets the number of minutes the player played in the game. Must be ≥ 0.
    /// </summary>
    public required int MinutesPlayed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player started the game.
    /// </summary>
    public required bool IsStarter { get; init; }

    /// <summary>
    /// Gets the player's jersey number for this game. Must be &gt; 0.
    /// </summary>
    public required int JerseyNumber { get; init; }

    /// <summary>
    /// Gets the number of goals scored by the player in the game. Must be ≥ 0.
    /// </summary>
    public required int Goals { get; init; }

    /// <summary>
    /// Gets the number of assists made by the player in the game. Must be ≥ 0.
    /// </summary>
    public required int Assists { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player statistic record was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the identifier of the user who created this player statistic record.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player statistic record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the identifier of the user who last updated this player statistic record.
    /// </summary>
    public string? UpdatedBy { get; private set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated TeamPlayer entity.
    /// </summary>
    public TeamPlayer? TeamPlayer { get; set; }

    /// <summary>
    /// Validates the player statistic entity against all business rules.
    /// </summary>
    /// <returns>True if the entity is valid; otherwise, false.</returns>
    /// <remarks>
    /// Validation rules:
    /// - TeamPlayerId must be greater than 0
    /// - GameDate must be valid (not default)
    /// - MinutesPlayed must be ≥ 0
    /// - JerseyNumber must be &gt; 0
    /// - Goals must be ≥ 0
    /// - Assists must be ≥ 0
    /// - CreatedBy is required
    /// </remarks>
    /// <example>
    /// <code>
    /// var statistic = new PlayerStatistic
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 2,
    ///     Assists = 1,
    ///     CreatedBy = "system"
    /// };
    /// bool isValid = statistic.Validate(); // Returns true
    /// </code>
    /// </example>
    public bool Validate()
    {
        // TeamPlayerId must reference a valid TeamPlayer
        if (TeamPlayerId <= 0)
        {
            return false;
        }

        // GameDate must be valid (not default)
        if (GameDate == default)
        {
            return false;
        }

        // MinutesPlayed must be non-negative
        if (MinutesPlayed < 0)
        {
            return false;
        }

        // JerseyNumber must be positive
        if (JerseyNumber <= 0)
        {
            return false;
        }

        // Goals must be non-negative
        if (Goals < 0)
        {
            return false;
        }

        // Assists must be non-negative
        if (Assists < 0)
        {
            return false;
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
    /// var statistic = new PlayerStatistic
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 2,
    ///     Assists = 1,
    ///     CreatedBy = "system"
    /// };
    /// statistic.UpdateLastModified("admin-user");
    /// // statistic.UpdatedAt and statistic.UpdatedBy are now set
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

    /// <summary>
    /// Calculates the total minutes played from a collection of player statistics.
    /// </summary>
    /// <param name="statistics">The collection of player statistics to aggregate.</param>
    /// <returns>The total minutes played across all statistics.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistics is null.</exception>
    /// <example>
    /// <code>
    /// var statistics = new List&lt;PlayerStatistic&gt;
    /// {
    ///     new PlayerStatistic { MinutesPlayed = 90, ... },
    ///     new PlayerStatistic { MinutesPlayed = 45, ... }
    /// };
    /// int totalMinutes = PlayerStatistic.CalculateTotalMinutes(statistics); // Returns 135
    /// </code>
    /// </example>
    public static int CalculateTotalMinutes(IEnumerable<PlayerStatistic> statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);
        return statistics.Sum(s => s.MinutesPlayed);
    }

    /// <summary>
    /// Calculates the total goals from a collection of player statistics.
    /// </summary>
    /// <param name="statistics">The collection of player statistics to aggregate.</param>
    /// <returns>The total goals scored across all statistics.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistics is null.</exception>
    /// <example>
    /// <code>
    /// var statistics = new List&lt;PlayerStatistic&gt;
    /// {
    ///     new PlayerStatistic { Goals = 2, ... },
    ///     new PlayerStatistic { Goals = 1, ... }
    /// };
    /// int totalGoals = PlayerStatistic.CalculateTotalGoals(statistics); // Returns 3
    /// </code>
    /// </example>
    public static int CalculateTotalGoals(IEnumerable<PlayerStatistic> statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);
        return statistics.Sum(s => s.Goals);
    }

    /// <summary>
    /// Calculates the total assists from a collection of player statistics.
    /// </summary>
    /// <param name="statistics">The collection of player statistics to aggregate.</param>
    /// <returns>The total assists across all statistics.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistics is null.</exception>
    /// <example>
    /// <code>
    /// var statistics = new List&lt;PlayerStatistic&gt;
    /// {
    ///     new PlayerStatistic { Assists = 1, ... },
    ///     new PlayerStatistic { Assists = 2, ... }
    /// };
    /// int totalAssists = PlayerStatistic.CalculateTotalAssists(statistics); // Returns 3
    /// </code>
    /// </example>
    public static int CalculateTotalAssists(IEnumerable<PlayerStatistic> statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);
        return statistics.Sum(s => s.Assists);
    }
}
