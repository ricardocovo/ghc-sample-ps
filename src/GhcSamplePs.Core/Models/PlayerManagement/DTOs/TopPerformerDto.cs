namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying top performer statistics.
/// Contains aggregated player performance data for leaderboard display.
/// </summary>
public sealed record TopPerformerDto
{
    /// <summary>
    /// Gets the player identifier.
    /// </summary>
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public required string PlayerName { get; init; }

    /// <summary>
    /// Gets the total number of goals scored across all games.
    /// </summary>
    public required int TotalGoals { get; init; }

    /// <summary>
    /// Gets the total number of games played.
    /// </summary>
    public required int GamesPlayed { get; init; }

    /// <summary>
    /// Gets the average goals per game.
    /// </summary>
    public required decimal GoalsPerGame { get; init; }

    /// <summary>
    /// Gets the total number of assists across all games.
    /// </summary>
    public required int TotalAssists { get; init; }
}
