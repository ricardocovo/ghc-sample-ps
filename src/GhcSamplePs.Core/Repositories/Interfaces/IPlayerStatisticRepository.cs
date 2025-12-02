using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for PlayerStatistic data access operations.
/// </summary>
public interface IPlayerStatisticRepository
{
    /// <summary>
    /// Retrieves all player statistics for a specific player through their team player assignments.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of player statistics ordered by GameDate DESC.</returns>
    Task<IReadOnlyList<PlayerStatistic>> GetAllByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all player statistics for a specific team player assignment.
    /// </summary>
    /// <param name="teamPlayerId">The team player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of player statistics ordered by GameDate DESC.</returns>
    Task<IReadOnlyList<PlayerStatistic>> GetAllByTeamPlayerIdAsync(int teamPlayerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single player statistic by its unique identifier.
    /// </summary>
    /// <param name="statisticId">The unique identifier of the player statistic.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The player statistic if found; otherwise, null.</returns>
    Task<PlayerStatistic?> GetByIdAsync(int statisticId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves player statistics for a specific player within a date range.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="startDate">The start date of the range (inclusive).</param>
    /// <param name="endDate">The end date of the range (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of player statistics ordered by GameDate DESC.</returns>
    Task<IReadOnlyList<PlayerStatistic>> GetByDateRangeAsync(
        int playerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new player statistic to the data store.
    /// </summary>
    /// <param name="statistic">The player statistic to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added player statistic with generated identifier.</returns>
    Task<PlayerStatistic> AddAsync(PlayerStatistic statistic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player statistic in the data store.
    /// </summary>
    /// <param name="statistic">The player statistic with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated player statistic.</returns>
    Task<PlayerStatistic> UpdateAsync(PlayerStatistic statistic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player statistic from the data store.
    /// </summary>
    /// <param name="statisticId">The unique identifier of the player statistic to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the player statistic was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(int statisticId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a player statistic with the specified identifier exists.
    /// </summary>
    /// <param name="statisticId">The unique identifier to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the player statistic exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int statisticId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregate statistics (totals and averages) for a player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="teamPlayerId">Optional team player identifier to filter by specific team.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate statistics result.</returns>
    Task<PlayerStatisticAggregateResult> GetAggregatesAsync(
        int playerId,
        int? teamPlayerId = null,
        CancellationToken cancellationToken = default);
}
