using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Repositories.Implementations;

/// <summary>
/// Entity Framework Core implementation of <see cref="ITeamPlayerRepository"/>.
/// Provides database persistence for team player entities with proper error handling and logging.
/// </summary>
/// <remarks>
/// <para>
/// This repository uses <see cref="ApplicationDbContext"/> for database operations and implements:
/// - AsNoTracking queries for read operations to improve performance
/// - Optimistic concurrency handling for updates
/// - Comprehensive error handling with domain-specific exceptions
/// - Detailed logging for all operations
/// - Results ordered by JoinedDate DESC
/// </para>
/// <example>
/// <code>
/// // Register in DI container
/// builder.Services.AddScoped&lt;ITeamPlayerRepository, EfTeamPlayerRepository&gt;();
/// 
/// // Use in a service
/// public class TeamPlayerService
/// {
///     private readonly ITeamPlayerRepository _repository;
///     
///     public async Task&lt;TeamPlayer?&gt; GetTeamPlayerAsync(int id)
///     {
///         return await _repository.GetByIdAsync(id);
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public sealed class EfTeamPlayerRepository : ITeamPlayerRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfTeamPlayerRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfTeamPlayerRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for data operations.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfTeamPlayerRepository(ApplicationDbContext context, ILogger<EfTeamPlayerRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<TeamPlayer>> GetAllByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all team player assignments for player ID {PlayerId}", playerId);

        try
        {
            var teamPlayers = await _context.TeamPlayers
                .AsNoTracking()
                .Where(tp => tp.PlayerId == playerId)
                .OrderByDescending(tp => tp.JoinedDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} team player assignments for player ID {PlayerId}", teamPlayers.Count, playerId);
            return teamPlayers;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllByPlayerIdAsync operation was cancelled for player ID {PlayerId}", playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team player assignments for player ID {PlayerId}", playerId);
            throw new RepositoryException(
                $"Failed to retrieve team player assignments for player ID {playerId}.",
                nameof(GetAllByPlayerIdAsync),
                nameof(TeamPlayer),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<TeamPlayer>> GetActiveByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving active team player assignments for player ID {PlayerId}", playerId);

        try
        {
            var teamPlayers = await _context.TeamPlayers
                .AsNoTracking()
                .Where(tp => tp.PlayerId == playerId && tp.LeftDate == null)
                .OrderByDescending(tp => tp.JoinedDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} active team player assignments for player ID {PlayerId}", teamPlayers.Count, playerId);
            return teamPlayers;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetActiveByPlayerIdAsync operation was cancelled for player ID {PlayerId}", playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active team player assignments for player ID {PlayerId}", playerId);
            throw new RepositoryException(
                $"Failed to retrieve active team player assignments for player ID {playerId}.",
                nameof(GetActiveByPlayerIdAsync),
                nameof(TeamPlayer),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<TeamPlayer?> GetByIdAsync(int teamPlayerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving team player assignment with ID {TeamPlayerId}", teamPlayerId);

        try
        {
            var teamPlayer = await _context.TeamPlayers
                .AsNoTracking()
                .FirstOrDefaultAsync(tp => tp.TeamPlayerId == teamPlayerId, cancellationToken);

            if (teamPlayer is not null)
            {
                _logger.LogInformation("Retrieved team player assignment with ID {TeamPlayerId}", teamPlayerId);
            }
            else
            {
                _logger.LogDebug("Team player assignment with ID {TeamPlayerId} not found", teamPlayerId);
            }

            return teamPlayer;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetByIdAsync operation was cancelled for team player ID {TeamPlayerId}", teamPlayerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team player assignment with ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"Failed to retrieve team player assignment with ID {teamPlayerId}.",
                nameof(GetByIdAsync),
                nameof(TeamPlayer),
                teamPlayerId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="teamPlayer"/> is null.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<TeamPlayer> AddAsync(TeamPlayer teamPlayer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(teamPlayer);

        _logger.LogDebug("Adding new team player assignment for player ID {PlayerId}, team '{TeamName}'", teamPlayer.PlayerId, teamPlayer.TeamName);

        try
        {
            _context.TeamPlayers.Add(teamPlayer);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully added team player assignment with ID {TeamPlayerId} for player ID {PlayerId}, team '{TeamName}'",
                teamPlayer.TeamPlayerId,
                teamPlayer.PlayerId,
                teamPlayer.TeamName);

            return teamPlayer;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AddAsync operation was cancelled for player ID {PlayerId}, team '{TeamName}'", teamPlayer.PlayerId, teamPlayer.TeamName);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error adding team player assignment for player ID {PlayerId}, team '{TeamName}'", teamPlayer.PlayerId, teamPlayer.TeamName);
            throw new RepositoryException(
                $"Failed to add team player assignment for player ID {teamPlayer.PlayerId}, team '{teamPlayer.TeamName}'.",
                nameof(AddAsync),
                nameof(TeamPlayer),
                innerException: ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding team player assignment for player ID {PlayerId}, team '{TeamName}'", teamPlayer.PlayerId, teamPlayer.TeamName);
            throw new RepositoryException(
                $"Failed to add team player assignment for player ID {teamPlayer.PlayerId}, team '{teamPlayer.TeamName}'.",
                nameof(AddAsync),
                nameof(TeamPlayer),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="teamPlayer"/> is null.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs or entity not found.</exception>
    public async Task<TeamPlayer> UpdateAsync(TeamPlayer teamPlayer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(teamPlayer);

        _logger.LogDebug("Updating team player assignment with ID {TeamPlayerId}", teamPlayer.TeamPlayerId);

        try
        {
            var existingTeamPlayer = await _context.TeamPlayers.FindAsync([teamPlayer.TeamPlayerId], cancellationToken);

            if (existingTeamPlayer is null)
            {
                _logger.LogWarning("Team player assignment with ID {TeamPlayerId} not found for update", teamPlayer.TeamPlayerId);
                throw new RepositoryException(
                    $"Team player assignment with ID {teamPlayer.TeamPlayerId} not found.",
                    nameof(UpdateAsync),
                    nameof(TeamPlayer),
                    teamPlayer.TeamPlayerId);
            }

            // Update all properties from the domain entity, including audit fields
            _context.Entry(existingTeamPlayer).CurrentValues.SetValues(teamPlayer);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated team player assignment with ID {TeamPlayerId}", teamPlayer.TeamPlayerId);

            return existingTeamPlayer;
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("UpdateAsync operation was cancelled for team player ID {TeamPlayerId}", teamPlayer.TeamPlayerId);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating team player assignment with ID {TeamPlayerId}", teamPlayer.TeamPlayerId);
            throw new RepositoryException(
                $"The team player assignment with ID {teamPlayer.TeamPlayerId} was modified by another user. Please refresh and try again.",
                nameof(UpdateAsync),
                nameof(TeamPlayer),
                teamPlayer.TeamPlayerId,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating team player assignment with ID {TeamPlayerId}", teamPlayer.TeamPlayerId);
            throw new RepositoryException(
                $"Failed to update team player assignment with ID {teamPlayer.TeamPlayerId}.",
                nameof(UpdateAsync),
                nameof(TeamPlayer),
                teamPlayer.TeamPlayerId,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating team player assignment with ID {TeamPlayerId}", teamPlayer.TeamPlayerId);
            throw new RepositoryException(
                $"Failed to update team player assignment with ID {teamPlayer.TeamPlayerId}.",
                nameof(UpdateAsync),
                nameof(TeamPlayer),
                teamPlayer.TeamPlayerId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> DeleteAsync(int teamPlayerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting team player assignment with ID {TeamPlayerId}", teamPlayerId);

        try
        {
            var teamPlayer = await _context.TeamPlayers.FindAsync([teamPlayerId], cancellationToken);

            if (teamPlayer is null)
            {
                _logger.LogDebug("Team player assignment with ID {TeamPlayerId} not found for deletion", teamPlayerId);
                return false;
            }

            _context.TeamPlayers.Remove(teamPlayer);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted team player assignment with ID {TeamPlayerId}", teamPlayerId);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DeleteAsync operation was cancelled for team player ID {TeamPlayerId}", teamPlayerId);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict deleting team player assignment with ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"The team player assignment with ID {teamPlayerId} was modified by another user. Please refresh and try again.",
                nameof(DeleteAsync),
                nameof(TeamPlayer),
                teamPlayerId,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting team player assignment with ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"Failed to delete team player assignment with ID {teamPlayerId}.",
                nameof(DeleteAsync),
                nameof(TeamPlayer),
                teamPlayerId,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting team player assignment with ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"Failed to delete team player assignment with ID {teamPlayerId}.",
                nameof(DeleteAsync),
                nameof(TeamPlayer),
                teamPlayerId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> ExistsAsync(int teamPlayerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking existence of team player assignment with ID {TeamPlayerId}", teamPlayerId);

        try
        {
            var exists = await _context.TeamPlayers.AnyAsync(tp => tp.TeamPlayerId == teamPlayerId, cancellationToken);

            _logger.LogDebug("Team player assignment with ID {TeamPlayerId} exists: {Exists}", teamPlayerId, exists);
            return exists;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("ExistsAsync operation was cancelled for team player ID {TeamPlayerId}", teamPlayerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of team player assignment with ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"Failed to check existence of team player assignment with ID {teamPlayerId}.",
                nameof(ExistsAsync),
                nameof(TeamPlayer),
                teamPlayerId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Thrown when <paramref name="teamName"/> or <paramref name="championshipName"/> is null or whitespace.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> HasActiveDuplicateAsync(
        int playerId,
        string teamName,
        string championshipName,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(teamName))
        {
            throw new ArgumentException("Team name cannot be null or whitespace.", nameof(teamName));
        }

        if (string.IsNullOrWhiteSpace(championshipName))
        {
            throw new ArgumentException("Championship name cannot be null or whitespace.", nameof(championshipName));
        }

        _logger.LogDebug(
            "Checking for active duplicate for player ID {PlayerId}, team '{TeamName}', championship '{ChampionshipName}', excluding ID {ExcludeId}",
            playerId, teamName, championshipName, excludeId);

        try
        {
            var query = _context.TeamPlayers
                .Where(tp => tp.PlayerId == playerId
                    && tp.TeamName == teamName
                    && tp.ChampionshipName == championshipName
                    && tp.LeftDate == null);

            if (excludeId.HasValue)
            {
                query = query.Where(tp => tp.TeamPlayerId != excludeId.Value);
            }

            var exists = await query.AnyAsync(cancellationToken);

            _logger.LogDebug(
                "Active duplicate for player ID {PlayerId}, team '{TeamName}', championship '{ChampionshipName}': {Exists}",
                playerId, teamName, championshipName, exists);

            return exists;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "HasActiveDuplicateAsync operation was cancelled for player ID {PlayerId}, team '{TeamName}'",
                playerId, teamName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking for active duplicate for player ID {PlayerId}, team '{TeamName}', championship '{ChampionshipName}'",
                playerId, teamName, championshipName);
            throw new RepositoryException(
                $"Failed to check for active duplicate for player ID {playerId}, team '{teamName}', championship '{championshipName}'.",
                nameof(HasActiveDuplicateAsync),
                nameof(TeamPlayer),
                innerException: ex);
        }
    }
}
