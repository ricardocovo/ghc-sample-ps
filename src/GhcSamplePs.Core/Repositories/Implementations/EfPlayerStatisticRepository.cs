using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Repositories.Implementations;

/// <summary>
/// Entity Framework Core implementation of <see cref="IPlayerStatisticRepository"/>.
/// Provides database persistence for player statistic entities with proper error handling and logging.
/// </summary>
/// <remarks>
/// <para>
/// This repository uses <see cref="ApplicationDbContext"/> for database operations and implements:
/// - AsNoTracking queries for read operations to improve performance
/// - Optimistic concurrency handling for updates
/// - Comprehensive error handling with domain-specific exceptions
/// - Detailed logging for all operations
/// - Results ordered by GameDate DESC
/// - TeamPlayer navigation included when needed
/// </para>
/// <example>
/// <code>
/// // Register in DI container
/// builder.Services.AddScoped&lt;IPlayerStatisticRepository, EfPlayerStatisticRepository&gt;();
/// 
/// // Use in a service
/// public class PlayerStatisticService
/// {
///     private readonly IPlayerStatisticRepository _repository;
///     
///     public async Task&lt;PlayerStatistic?&gt; GetStatisticAsync(int id)
///     {
///         return await _repository.GetByIdAsync(id);
///     }
/// }
/// </code>
/// </example>
/// </remarks>
public sealed class EfPlayerStatisticRepository : IPlayerStatisticRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EfPlayerStatisticRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfPlayerStatisticRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for data operations.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> or <paramref name="logger"/> is null.
    /// </exception>
    public EfPlayerStatisticRepository(ApplicationDbContext context, ILogger<EfPlayerStatisticRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<PlayerStatistic>> GetAllByPlayerIdAsync(int playerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all player statistics for player ID {PlayerId}", playerId);

        try
        {
            var statistics = await _context.PlayerStatistics
                .AsNoTracking()
                .Include(ps => ps.TeamPlayer)
                .Where(ps => ps.TeamPlayer != null && ps.TeamPlayer.PlayerId == playerId)
                .OrderByDescending(ps => ps.GameDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} player statistics for player ID {PlayerId}", statistics.Count, playerId);
            return statistics;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllByPlayerIdAsync operation was cancelled for player ID {PlayerId}", playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player statistics for player ID {PlayerId}", playerId);
            throw new RepositoryException(
                $"Failed to retrieve player statistics for player ID {playerId}.",
                nameof(GetAllByPlayerIdAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<PlayerStatistic>> GetAllByTeamPlayerIdAsync(int teamPlayerId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving all player statistics for team player ID {TeamPlayerId}", teamPlayerId);

        try
        {
            var statistics = await _context.PlayerStatistics
                .AsNoTracking()
                .Include(ps => ps.TeamPlayer)
                .Where(ps => ps.TeamPlayerId == teamPlayerId)
                .OrderByDescending(ps => ps.GameDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} player statistics for team player ID {TeamPlayerId}", statistics.Count, teamPlayerId);
            return statistics;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAllByTeamPlayerIdAsync operation was cancelled for team player ID {TeamPlayerId}", teamPlayerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player statistics for team player ID {TeamPlayerId}", teamPlayerId);
            throw new RepositoryException(
                $"Failed to retrieve player statistics for team player ID {teamPlayerId}.",
                nameof(GetAllByTeamPlayerIdAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<PlayerStatistic?> GetByIdAsync(int statisticId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving player statistic with ID {StatisticId}", statisticId);

        try
        {
            var statistic = await _context.PlayerStatistics
                .AsNoTracking()
                .Include(ps => ps.TeamPlayer)
                .FirstOrDefaultAsync(ps => ps.PlayerStatisticId == statisticId, cancellationToken);

            if (statistic is not null)
            {
                _logger.LogInformation("Retrieved player statistic with ID {StatisticId}", statisticId);
            }
            else
            {
                _logger.LogDebug("Player statistic with ID {StatisticId} not found", statisticId);
            }

            return statistic;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetByIdAsync operation was cancelled for statistic ID {StatisticId}", statisticId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player statistic with ID {StatisticId}", statisticId);
            throw new RepositoryException(
                $"Failed to retrieve player statistic with ID {statisticId}.",
                nameof(GetByIdAsync),
                nameof(PlayerStatistic),
                statisticId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Thrown when startDate is greater than endDate.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<IReadOnlyList<PlayerStatistic>> GetByDateRangeAsync(
        int playerId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be less than or equal to end date.", nameof(startDate));
        }

        _logger.LogDebug(
            "Retrieving player statistics for player ID {PlayerId} between {StartDate} and {EndDate}",
            playerId, startDate, endDate);

        try
        {
            var statistics = await _context.PlayerStatistics
                .AsNoTracking()
                .Include(ps => ps.TeamPlayer)
                .Where(ps => ps.TeamPlayer != null && ps.TeamPlayer.PlayerId == playerId)
                .Where(ps => ps.GameDate >= startDate && ps.GameDate <= endDate)
                .OrderByDescending(ps => ps.GameDate)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} player statistics for player ID {PlayerId} between {StartDate} and {EndDate}",
                statistics.Count, playerId, startDate, endDate);

            return statistics;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "GetByDateRangeAsync operation was cancelled for player ID {PlayerId}",
                playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving player statistics for player ID {PlayerId} between {StartDate} and {EndDate}",
                playerId, startDate, endDate);
            throw new RepositoryException(
                $"Failed to retrieve player statistics for player ID {playerId} between {startDate} and {endDate}.",
                nameof(GetByDateRangeAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="statistic"/> is null.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<PlayerStatistic> AddAsync(PlayerStatistic statistic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(statistic);

        _logger.LogDebug("Adding new player statistic for team player ID {TeamPlayerId}, game date {GameDate}",
            statistic.TeamPlayerId, statistic.GameDate);

        try
        {
            _context.PlayerStatistics.Add(statistic);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully added player statistic with ID {StatisticId} for team player ID {TeamPlayerId}",
                statistic.PlayerStatisticId,
                statistic.TeamPlayerId);

            return statistic;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("AddAsync operation was cancelled for team player ID {TeamPlayerId}", statistic.TeamPlayerId);
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error adding player statistic for team player ID {TeamPlayerId}", statistic.TeamPlayerId);
            throw new RepositoryException(
                $"Failed to add player statistic for team player ID {statistic.TeamPlayerId}.",
                nameof(AddAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding player statistic for team player ID {TeamPlayerId}", statistic.TeamPlayerId);
            throw new RepositoryException(
                $"Failed to add player statistic for team player ID {statistic.TeamPlayerId}.",
                nameof(AddAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="statistic"/> is null.</exception>
    /// <exception cref="RepositoryException">Thrown when a database error occurs or entity not found.</exception>
    public async Task<PlayerStatistic> UpdateAsync(PlayerStatistic statistic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(statistic);

        _logger.LogDebug("Updating player statistic with ID {StatisticId}", statistic.PlayerStatisticId);

        try
        {
            var existingStatistic = await _context.PlayerStatistics.FindAsync([statistic.PlayerStatisticId], cancellationToken);

            if (existingStatistic is null)
            {
                _logger.LogWarning("Player statistic with ID {StatisticId} not found for update", statistic.PlayerStatisticId);
                throw new RepositoryException(
                    $"Player statistic with ID {statistic.PlayerStatisticId} not found.",
                    nameof(UpdateAsync),
                    nameof(PlayerStatistic),
                    statistic.PlayerStatisticId);
            }

            // Update all properties from the domain entity, including audit fields
            _context.Entry(existingStatistic).CurrentValues.SetValues(statistic);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated player statistic with ID {StatisticId}", statistic.PlayerStatisticId);

            return existingStatistic;
        }
        catch (RepositoryException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("UpdateAsync operation was cancelled for statistic ID {StatisticId}", statistic.PlayerStatisticId);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict updating player statistic with ID {StatisticId}", statistic.PlayerStatisticId);
            throw new RepositoryException(
                $"The player statistic with ID {statistic.PlayerStatisticId} was modified by another user. Please refresh and try again.",
                nameof(UpdateAsync),
                nameof(PlayerStatistic),
                statistic.PlayerStatisticId,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error updating player statistic with ID {StatisticId}", statistic.PlayerStatisticId);
            throw new RepositoryException(
                $"Failed to update player statistic with ID {statistic.PlayerStatisticId}.",
                nameof(UpdateAsync),
                nameof(PlayerStatistic),
                statistic.PlayerStatisticId,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating player statistic with ID {StatisticId}", statistic.PlayerStatisticId);
            throw new RepositoryException(
                $"Failed to update player statistic with ID {statistic.PlayerStatisticId}.",
                nameof(UpdateAsync),
                nameof(PlayerStatistic),
                statistic.PlayerStatisticId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> DeleteAsync(int statisticId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting player statistic with ID {StatisticId}", statisticId);

        try
        {
            var statistic = await _context.PlayerStatistics.FindAsync([statisticId], cancellationToken);

            if (statistic is null)
            {
                _logger.LogDebug("Player statistic with ID {StatisticId} not found for deletion", statisticId);
                return false;
            }

            _context.PlayerStatistics.Remove(statistic);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted player statistic with ID {StatisticId}", statisticId);
            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DeleteAsync operation was cancelled for statistic ID {StatisticId}", statisticId);
            throw;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict deleting player statistic with ID {StatisticId}", statisticId);
            throw new RepositoryException(
                $"The player statistic with ID {statisticId} was modified by another user. Please refresh and try again.",
                nameof(DeleteAsync),
                nameof(PlayerStatistic),
                statisticId,
                ex);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error deleting player statistic with ID {StatisticId}", statisticId);
            throw new RepositoryException(
                $"Failed to delete player statistic with ID {statisticId}.",
                nameof(DeleteAsync),
                nameof(PlayerStatistic),
                statisticId,
                ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting player statistic with ID {StatisticId}", statisticId);
            throw new RepositoryException(
                $"Failed to delete player statistic with ID {statisticId}.",
                nameof(DeleteAsync),
                nameof(PlayerStatistic),
                statisticId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<bool> ExistsAsync(int statisticId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking existence of player statistic with ID {StatisticId}", statisticId);

        try
        {
            var exists = await _context.PlayerStatistics.AnyAsync(ps => ps.PlayerStatisticId == statisticId, cancellationToken);

            _logger.LogDebug("Player statistic with ID {StatisticId} exists: {Exists}", statisticId, exists);
            return exists;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("ExistsAsync operation was cancelled for statistic ID {StatisticId}", statisticId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of player statistic with ID {StatisticId}", statisticId);
            throw new RepositoryException(
                $"Failed to check existence of player statistic with ID {statisticId}.",
                nameof(ExistsAsync),
                nameof(PlayerStatistic),
                statisticId,
                ex);
        }
    }

    /// <inheritdoc />
    /// <exception cref="RepositoryException">Thrown when a database error occurs.</exception>
    public async Task<PlayerStatisticAggregateResult> GetAggregatesAsync(
        int playerId,
        int? teamPlayerId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Calculating aggregate statistics for player ID {PlayerId}, team player ID {TeamPlayerId}",
            playerId, teamPlayerId);

        try
        {
            var query = _context.PlayerStatistics
                .AsNoTracking()
                .Include(ps => ps.TeamPlayer)
                .Where(ps => ps.TeamPlayer != null && ps.TeamPlayer.PlayerId == playerId);

            if (teamPlayerId.HasValue)
            {
                query = query.Where(ps => ps.TeamPlayerId == teamPlayerId.Value);
            }

            var statistics = await query.ToListAsync(cancellationToken);

            if (statistics.Count == 0)
            {
                _logger.LogDebug("No statistics found for player ID {PlayerId}", playerId);
                return PlayerStatisticAggregateResult.Empty();
            }

            var result = new PlayerStatisticAggregateResult
            {
                GameCount = statistics.Count,
                TotalGoals = statistics.Sum(s => s.Goals),
                TotalAssists = statistics.Sum(s => s.Assists),
                TotalMinutesPlayed = statistics.Sum(s => s.MinutesPlayed),
                AverageGoals = statistics.Average(s => s.Goals),
                AverageAssists = statistics.Average(s => s.Assists),
                AverageMinutesPlayed = statistics.Average(s => s.MinutesPlayed)
            };

            _logger.LogInformation(
                "Calculated aggregates for player ID {PlayerId}: {GameCount} games, {TotalGoals} goals, {TotalAssists} assists",
                playerId, result.GameCount, result.TotalGoals, result.TotalAssists);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetAggregatesAsync operation was cancelled for player ID {PlayerId}", playerId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating aggregate statistics for player ID {PlayerId}", playerId);
            throw new RepositoryException(
                $"Failed to calculate aggregate statistics for player ID {playerId}.",
                nameof(GetAggregatesAsync),
                nameof(PlayerStatistic),
                innerException: ex);
        }
    }
}
