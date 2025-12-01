namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying team player information.
/// Contains all team player properties including computed fields for read operations.
/// </summary>
public sealed record TeamPlayerDto
{
    /// <summary>
    /// Gets the unique identifier for the team player record.
    /// </summary>
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the identifier of the player.
    /// </summary>
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the name of the team.
    /// </summary>
    public required string TeamName { get; init; }

    /// <summary>
    /// Gets the name of the championship.
    /// </summary>
    public required string ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date when the player joined the team.
    /// </summary>
    public required DateTime JoinedDate { get; init; }

    /// <summary>
    /// Gets the date when the player left the team.
    /// Null indicates the player is still active on the team.
    /// </summary>
    public DateTime? LeftDate { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player is currently active on the team.
    /// Returns true when LeftDate is null.
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// Gets the duration in days the player has been or was on the team.
    /// Calculated from JoinedDate to LeftDate (if provided) or current date.
    /// </summary>
    public required int DurationDays { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the team player record was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the team player record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Creates a TeamPlayerDto from a TeamPlayer entity.
    /// </summary>
    /// <param name="teamPlayer">The team player entity to map from.</param>
    /// <returns>A new TeamPlayerDto instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when teamPlayer is null.</exception>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     TeamPlayerId = 1,
    ///     PlayerId = 123,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// var dto = TeamPlayerDto.FromEntity(teamPlayer);
    /// </code>
    /// </example>
    public static TeamPlayerDto FromEntity(TeamPlayer teamPlayer)
    {
        ArgumentNullException.ThrowIfNull(teamPlayer);

        var endDate = teamPlayer.LeftDate ?? DateTime.UtcNow;
        var durationDays = (int)(endDate.Date - teamPlayer.JoinedDate.Date).TotalDays;

        return new TeamPlayerDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            PlayerId = teamPlayer.PlayerId,
            TeamName = teamPlayer.TeamName.Trim(),
            ChampionshipName = teamPlayer.ChampionshipName.Trim(),
            JoinedDate = teamPlayer.JoinedDate,
            LeftDate = teamPlayer.LeftDate,
            IsActive = teamPlayer.IsActive,
            DurationDays = durationDays,
            CreatedAt = teamPlayer.CreatedAt,
            UpdatedAt = teamPlayer.UpdatedAt
        };
    }
}
