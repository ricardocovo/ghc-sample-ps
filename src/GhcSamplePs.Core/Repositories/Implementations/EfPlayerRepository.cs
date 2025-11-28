using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Repositories.Implementations;

/// <summary>
/// Entity Framework Core implementation of <see cref="IPlayerRepository"/>.
/// Provides database persistence for player entities with proper error handling and logging.
/// </summary>
/// <remarks>
/// <para>
/// This repository uses <see cref="ApplicationDbContext"/> for database operations and implements:
/// - AsNoTracking queries for read operations (GetAll, GetById) to improve performance
/// - Optimistic concurrency handling for updates
/// - Comprehensive error handling with domain-specific exceptions
/// - Detailed logging for all operations
/// </para>
/// <example>
/// <code>
/// // Register in DI container
/// builder.Services.AddScoped&lt;IPlayerRepository, EfPlayerRepository&gt;();
/// 
/// // Use in a service
/// public class PlayerService
/// {
///     private readonly IPlayerRepository _repository;
///     
///     public async Task&lt;Player?&gt; GetPlayerAsync(int id)
///     {
///         return await _repository.GetByIdAsync(id);
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public sealed class EfPlayerRepository : IPlayerRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfPlayerRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfPlayerRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for data operations.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfPlayerRepository(ApplicationDbContext context, ILogger<EfPlayerRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all players from database");

        try
        {
            var players = await _context.Players
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} players from database", players.Count);
            return players;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllAsync operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all players from database");
            throw new RepositoryException(
                "Failed to retrieve players from database.",
                nameof(GetAllAsync),
                nameof(Player),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<Player?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving player with ID {PlayerId} from database", id);

        try
        {
            var player = await _context.Players
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (player is not null)
            {
                _logger.LogInformation("Retrieved player with ID {PlayerId}", id);
            }
            else
            {
                _logger.LogDebug("Player with ID {PlayerId} not found", id);
            }

            return player;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetByIdAsync operation was cancelled for player ID {PlayerId}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player with ID {PlayerId} from database", id);
            throw new RepositoryException(
                $"Failed to retrieve player with ID {id} from database.",
                nameof(GetByIdAsync),
                nameof(Player),
                id,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="player"/> is null.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<Player> AddAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);

        _logger.LogDebug("Adding new player '{PlayerName}' for user {UserId}", player.Name, player.UserId);

        try
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully added player with ID {PlayerId}, Name '{PlayerName}' for user {UserId}",
                player.Id,
                player.Name,
                player.UserId);

            return player;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AddAsync operation was cancelled for player '{PlayerName}'", player.Name);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error adding player '{PlayerName}'", player.Name);
            throw new RepositoryException(
                $"Failed to add player '{player.Name}' to database.",
                nameof(AddAsync),
                nameof(Player),
                innerException: ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding player '{PlayerName}'", player.Name);
            throw new RepositoryException(
                $"Failed to add player '{player.Name}' to database.",
                nameof(AddAsync),
                nameof(Player),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="player"/> is null.</exception>
    /// <exception cref="PlayerNotFoundException">Thrown when the player to update does not exist.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs or concurrency conflict detected.</exception>
    public async Task<Player> UpdateAsync(Player player, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(player);

        _logger.LogDebug("Updating player with ID {PlayerId}", player.Id);

        try
        {
            var existingPlayer = await _context.Players.FindAsync([player.Id], cancellationToken);

            if (existingPlayer is null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for update", player.Id);
                throw new PlayerNotFoundException(player.Id);
            }

            // Update properties
            existingPlayer.GetType().GetProperty(nameof(Player.Name))?.SetValue(existingPlayer, player.Name);
            _context.Entry(existingPlayer).CurrentValues.SetValues(new
            {
                player.Name,
                player.DateOfBirth,
                player.Gender,
                player.PhotoUrl,
                player.UserId
            });

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated player with ID {PlayerId}", player.Id);

            return existingPlayer;
        }
        catch (PlayerNotFoundException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("UpdateAsync operation was cancelled for player ID {PlayerId}", player.Id);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating player with ID {PlayerId}", player.Id);
            throw new RepositoryException(
                $"The player with ID {player.Id} was modified by another user. Please refresh and try again.",
                nameof(UpdateAsync),
                nameof(Player),
                player.Id,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating player with ID {PlayerId}", player.Id);
            throw new RepositoryException(
                $"Failed to update player with ID {player.Id} in database.",
                nameof(UpdateAsync),
                nameof(Player),
                player.Id,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating player with ID {PlayerId}", player.Id);
            throw new RepositoryException(
                $"Failed to update player with ID {player.Id} in database.",
                nameof(UpdateAsync),
                nameof(Player),
                player.Id,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting player with ID {PlayerId}", id);

        try
        {
            var player = await _context.Players.FindAsync([id], cancellationToken);

            if (player is null)
            {
                _logger.LogDebug("Player with ID {PlayerId} not found for deletion", id);
                return false;
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted player with ID {PlayerId}", id);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DeleteAsync operation was cancelled for player ID {PlayerId}", id);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict deleting player with ID {PlayerId}", id);
            throw new RepositoryException(
                $"The player with ID {id} was modified by another user. Please refresh and try again.",
                nameof(DeleteAsync),
                nameof(Player),
                id,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting player with ID {PlayerId}", id);
            throw new RepositoryException(
                $"Failed to delete player with ID {id} from database.",
                nameof(DeleteAsync),
                nameof(Player),
                id,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting player with ID {PlayerId}", id);
            throw new RepositoryException(
                $"Failed to delete player with ID {id} from database.",
                nameof(DeleteAsync),
                nameof(Player),
                id,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking existence of player with ID {PlayerId}", id);

        try
        {
            var exists = await _context.Players.AnyAsync(p => p.Id == id, cancellationToken);

            _logger.LogDebug("Player with ID {PlayerId} exists: {Exists}", id, exists);
            return exists;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("ExistsAsync operation was cancelled for player ID {PlayerId}", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of player with ID {PlayerId}", id);
            throw new RepositoryException(
                $"Failed to check existence of player with ID {id}.",
                nameof(ExistsAsync),
                nameof(Player),
                id,
                ex);
        }
    }
}
