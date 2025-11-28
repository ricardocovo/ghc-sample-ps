using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for player data access operations.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Retrieves all players from the data store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all players.</returns>
    Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single player by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the player.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The player if found; otherwise, null.</returns>
    Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new player to the data store.
    /// </summary>
    /// <param name="player">The player to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added player with generated identifier.</returns>
    Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player in the data store.
    /// </summary>
    /// <param name="player">The player with updated values.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated player.</returns>
    Task<Player> UpdateAsync(Player player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player from the data store.
    /// </summary>
    /// <param name="id">The unique identifier of the player to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the player was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a player with the specified identifier exists.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the player exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
