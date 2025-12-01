using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for TeamPlayer data access operations.
/// </summary>
public interface ITeamPlayerRepository
{
    /// <summary>
    /// Retrieves all team player assignments for a specific player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of team player assignments ordered by JoinedDate DESC.</returns>
    Task<IReadOnlyList<TeamPlayer>> GetAllByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves only active team player assignments for a specific player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of active team player assignments ordered by JoinedDate DESC.</returns>
    Task<IReadOnlyList<TeamPlayer>> GetActiveByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single team player assignment by its unique identifier.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier of the team player assignment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team player assignment if found; otherwise, null.</returns>
    Task<TeamPlayer?> GetByIdAsync(int teamPlayerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new team player assignment to the data store.
    /// </summary>
    /// <param name="teamPlayer">The team player assignment to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added team player assignment with generated identifier.</returns>
    Task<TeamPlayer> AddAsync(TeamPlayer teamPlayer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing team player assignment in the data store.
    /// </summary>
    /// <param name="teamPlayer">The team player assignment with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated team player assignment.</returns>
    Task<TeamPlayer> UpdateAsync(TeamPlayer teamPlayer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a team player assignment from the data store.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier of the team player assignment to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the team player assignment was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(int teamPlayerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a team player assignment with the specified identifier exists.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the team player assignment exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int teamPlayerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an active duplicate team player assignment exists for the given player, team, and championship.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="teamName">The team name.</param>
    /// <param name="championshipName">The championship name.</param>
    /// <param name="excludeId">Optional team player ID to exclude from the check (useful for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if an active duplicate exists; otherwise, false.</returns>
    Task<bool> HasActiveDuplicateAsync(
        int playerId,
        string teamName,
        string championshipName,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}
