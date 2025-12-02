namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Represents the aggregate statistics result for a player.
/// Contains totals and averages calculated from multiple game statistics.
/// </summary>
/// <remarks>
/// <example>
/// <code>
/// var aggregates = await repository.GetAggregatesAsync(playerId);
/// Console.WriteLine($"Total Goals: {aggregates.TotalGoals}, Average: {aggregates.AverageGoals:F2}");
/// </code>
/// </example>
/// </remarks>
public sealed record PlayerStatisticAggregateResult
{
    /// <summary>
    /// Gets the total number of games represented in the aggregation.
    /// </summary>
    public int GameCount { get; init; }

    /// <summary>
    /// Gets the total goals scored across all games.
    /// </summary>
    public int TotalGoals { get; init; }

    /// <summary>
    /// Gets the total assists made across all games.
    /// </summary>
    public int TotalAssists { get; init; }

    /// <summary>
    /// Gets the total minutes played across all games.
    /// </summary>
    public int TotalMinutesPlayed { get; init; }

    /// <summary>
    /// Gets the average goals scored per game.
    /// </summary>
    public double AverageGoals { get; init; }

    /// <summary>
    /// Gets the average assists made per game.
    /// </summary>
    public double AverageAssists { get; init; }

    /// <summary>
    /// Gets the average minutes played per game.
    /// </summary>
    public double AverageMinutesPlayed { get; init; }

    /// <summary>
    /// Creates an empty aggregate result with zero values.
    /// </summary>
    /// <returns>An empty <see cref="PlayerStatisticAggregateResult"/> instance.</returns>
    public static PlayerStatisticAggregateResult Empty()
    {
        return new PlayerStatisticAggregateResult
        {
            GameCount = 0,
            TotalGoals = 0,
            TotalAssists = 0,
            TotalMinutesPlayed = 0,
            AverageGoals = 0,
            AverageAssists = 0,
            AverageMinutesPlayed = 0
        };
    }
}
