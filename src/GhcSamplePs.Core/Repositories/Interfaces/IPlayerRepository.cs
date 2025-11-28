using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Repositories.Interfaces;

/// <summary>
/// Defines the contract for player data access operations.
/// </summary>
public interface IPlayerRepository
{
    /// <summary>
    /// Gets all players from the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of all players.</returns>
    Task<IEnumerable<Player>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a player by their unique identifier.
    /// </summary>
    /// <param name="id">The player's unique identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The player if found; otherwise, null.</returns>
    Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all players associated with a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of players owned by the specified user.</returns>
    Task<IEnumerable<Player>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new player to the repository.
    /// </summary>
    /// <param name="player">The player to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The added player with the assigned identifier.</returns>
    Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player in the repository.
    /// </summary>
    /// <param name="player">The player with updated information.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated player if successful; otherwise, null if not found.</returns>
    Task<Player?> UpdateAsync(Player player, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player from the repository by their unique identifier.
    /// </summary>
    /// <param name="id">The player's unique identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the player was deleted; otherwise, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
