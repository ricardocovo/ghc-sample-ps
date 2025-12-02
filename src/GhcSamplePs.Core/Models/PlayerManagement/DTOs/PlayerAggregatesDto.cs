namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying aggregated player statistics.
/// Contains summary metrics calculated from a collection of player statistics.
/// </summary>
public sealed record PlayerAggregatesDto
{
    /// <summary>
    /// Gets the total number of games played.
    /// </summary>
    public required int GamesPlayed { get; init; }

    /// <summary>
    /// Gets the total number of minutes played across all games.
    /// </summary>
    public required int TotalMinutes { get; init; }

    /// <summary>
    /// Gets the total number of goals scored across all games.
    /// </summary>
    public required int TotalGoals { get; init; }

    /// <summary>
    /// Gets the total number of assists across all games.
    /// </summary>
    public required int TotalAssists { get; init; }

    /// <summary>
    /// Gets the average number of goals per game.
    /// Returns 0 if no games have been played.
    /// </summary>
    public required decimal AverageGoalsPerGame { get; init; }

    /// <summary>
    /// Gets the average number of assists per game.
    /// Returns 0 if no games have been played.
    /// </summary>
    public required decimal AverageAssistsPerGame { get; init; }

    /// <summary>
    /// Creates a PlayerAggregatesDto from a collection of player statistics.
    /// </summary>
    /// <param name="statistics">The collection of player statistics to aggregate.</param>
    /// <returns>A new PlayerAggregatesDto instance with calculated aggregates.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistics is null.</exception>
    /// <example>
    /// <code>
    /// var statistics = new List&lt;PlayerStatistic&gt;
    /// {
    ///     new PlayerStatistic { MinutesPlayed = 90, Goals = 2, Assists = 1, ... },
    ///     new PlayerStatistic { MinutesPlayed = 45, Goals = 1, Assists = 2, ... }
    /// };
    /// var aggregates = PlayerAggregatesDto.FromStatistics(statistics);
    /// // aggregates.GamesPlayed = 2
    /// // aggregates.TotalGoals = 3
    /// // aggregates.AverageGoalsPerGame = 1.5m
    /// </code>
    /// </example>
    public static PlayerAggregatesDto FromStatistics(IEnumerable<PlayerStatistic> statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        var statisticsList = statistics.ToList();
        var gamesPlayed = statisticsList.Count;
        var totalMinutes = PlayerStatistic.CalculateTotalMinutes(statisticsList);
        var totalGoals = PlayerStatistic.CalculateTotalGoals(statisticsList);
        var totalAssists = PlayerStatistic.CalculateTotalAssists(statisticsList);

        return new PlayerAggregatesDto
        {
            GamesPlayed = gamesPlayed,
            TotalMinutes = totalMinutes,
            TotalGoals = totalGoals,
            TotalAssists = totalAssists,
            AverageGoalsPerGame = gamesPlayed > 0 ? (decimal)totalGoals / gamesPlayed : 0m,
            AverageAssistsPerGame = gamesPlayed > 0 ? (decimal)totalAssists / gamesPlayed : 0m
        };
    }
}
