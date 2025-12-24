namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying recent game activity.
/// Contains player, team, and game performance information for dashboard display.
/// </summary>
public sealed record RecentActivityDto
{
    /// <summary>
    /// Gets the unique identifier for the player statistic record.
    /// </summary>
    public required int PlayerStatisticId { get; init; }

    /// <summary>
    /// Gets the player identifier.
    /// </summary>
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public required string PlayerName { get; init; }

    /// <summary>
    /// Gets the name of the team.
    /// </summary>
    public required string TeamName { get; init; }

    /// <summary>
    /// Gets the name of the championship.
    /// </summary>
    public required string ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date of the game.
    /// </summary>
    public required DateTime GameDate { get; init; }

    /// <summary>
    /// Gets the number of goals scored in the game.
    /// </summary>
    public required int Goals { get; init; }

    /// <summary>
    /// Gets the number of assists made in the game.
    /// </summary>
    public required int Assists { get; init; }

    /// <summary>
    /// Gets the number of minutes played in the game.
    /// </summary>
    public required int MinutesPlayed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player started the game.
    /// </summary>
    public required bool IsStarter { get; init; }
}
