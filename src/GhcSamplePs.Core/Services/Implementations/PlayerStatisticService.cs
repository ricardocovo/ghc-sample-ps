using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Validation;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service implementation for player statistic management operations.
/// Coordinates between presentation and data layers, applies business rules and validation.
/// </summary>
public sealed class PlayerStatisticService : IPlayerStatisticService
{
    private readonly IPlayerStatisticRepository _statisticRepository;
    private readonly ITeamPlayerRepository _teamPlayerRepository;
    private readonly ILogger<PlayerStatisticService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerStatisticService"/> class.
    /// </summary>
    /// <param name="statisticRepository">The player statistic repository for data access.</param>
    /// <param name="teamPlayerRepository">The team player repository for team player validation.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public PlayerStatisticService(
        IPlayerStatisticRepository statisticRepository,
        ITeamPlayerRepository teamPlayerRepository,
        ILogger<PlayerStatisticService> logger)
    {
        ArgumentNullException.ThrowIfNull(statisticRepository);
        ArgumentNullException.ThrowIfNull(teamPlayerRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _statisticRepository = statisticRepository;
        _teamPlayerRepository = teamPlayerRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<PlayerStatisticDto>>> GetStatisticsByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving statistics for player {PlayerId}", playerId);

        try
        {
            var statistics = await _statisticRepository.GetAllByPlayerIdAsync(playerId, cancellationToken);
            var dtos = statistics.Select(PlayerStatisticDto.FromEntity).ToList();

            _logger.LogInformation("Retrieved {Count} statistics for player {PlayerId}",
                dtos.Count, playerId);

            return ServiceResult<IReadOnlyList<PlayerStatisticDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for player {PlayerId}", playerId);
            return ServiceResult<IReadOnlyList<PlayerStatisticDto>>.Fail("Unable to load statistics. Please refresh the page.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<PlayerStatisticDto>>> GetStatisticsByTeamPlayerIdAsync(
        int teamPlayerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving statistics for team player {TeamPlayerId}", teamPlayerId);

        try
        {
            var statistics = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayerId, cancellationToken);
            var dtos = statistics.Select(PlayerStatisticDto.FromEntity).ToList();

            _logger.LogInformation("Retrieved {Count} statistics for team player {TeamPlayerId}",
                dtos.Count, teamPlayerId);

            return ServiceResult<IReadOnlyList<PlayerStatisticDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistics for team player {TeamPlayerId}", teamPlayerId);
            return ServiceResult<IReadOnlyList<PlayerStatisticDto>>.Fail("Unable to load statistics. Please refresh the page.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerStatisticDto>> GetStatisticByIdAsync(
        int statisticId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving statistic with ID {StatisticId}", statisticId);

        try
        {
            var statistic = await _statisticRepository.GetByIdAsync(statisticId, cancellationToken);

            if (statistic is null)
            {
                _logger.LogWarning("Statistic with ID {StatisticId} not found", statisticId);
                return ServiceResult<PlayerStatisticDto>.Fail($"Statistic with ID {statisticId} could not be found.");
            }

            var dto = PlayerStatisticDto.FromEntity(statistic);

            _logger.LogInformation("Successfully retrieved statistic {StatisticId}", statisticId);

            return ServiceResult<PlayerStatisticDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statistic with ID {StatisticId}", statisticId);
            return ServiceResult<PlayerStatisticDto>.Fail("Unable to load statistic. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerStatisticDto>> AddStatisticAsync(
        CreatePlayerStatisticDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Creating new statistic for team player {TeamPlayerId} by user {UserId}",
            createDto.TeamPlayerId, currentUserId);

        var validationResult = PlayerStatisticValidator.ValidateCreatePlayerStatistic(createDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Statistic creation validation failed for team player {TeamPlayerId}: {Errors}",
                createDto.TeamPlayerId,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

            return ServiceResult<PlayerStatisticDto>.ValidationFailed(validationResult);
        }

        try
        {
            // Verify team player exists
            var teamPlayerExists = await _teamPlayerRepository.ExistsAsync(createDto.TeamPlayerId, cancellationToken);
            if (!teamPlayerExists)
            {
                _logger.LogWarning("Team player {TeamPlayerId} not found when creating statistic", createDto.TeamPlayerId);
                return ServiceResult<PlayerStatisticDto>.Fail($"Team player with ID {createDto.TeamPlayerId} could not be found.");
            }

            var statistic = createDto.ToEntity(currentUserId);
            var createdStatistic = await _statisticRepository.AddAsync(statistic, cancellationToken);
            var dto = PlayerStatisticDto.FromEntity(createdStatistic);

            _logger.LogInformation("Successfully created statistic {StatisticId} for team player {TeamPlayerId}",
                createdStatistic.PlayerStatisticId, createDto.TeamPlayerId);

            return ServiceResult<PlayerStatisticDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating statistic for team player {TeamPlayerId}",
                createDto.TeamPlayerId);
            return ServiceResult<PlayerStatisticDto>.Fail("Unable to save statistic. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerStatisticDto>> UpdateStatisticAsync(
        int statisticId,
        UpdatePlayerStatisticDto updateDto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Updating statistic {StatisticId} by user {UserId}",
            statisticId, currentUserId);

        if (updateDto.PlayerStatisticId != statisticId)
        {
            _logger.LogWarning("Statistic ID mismatch: URL ID {UrlId} != DTO ID {DtoId}",
                statisticId, updateDto.PlayerStatisticId);
            return ServiceResult<PlayerStatisticDto>.Fail("Statistic ID mismatch.");
        }

        var validationResult = PlayerStatisticValidator.ValidateUpdatePlayerStatistic(updateDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Statistic update validation failed for ID {StatisticId}: {Errors}",
                statisticId,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

            return ServiceResult<PlayerStatisticDto>.ValidationFailed(validationResult);
        }

        try
        {
            var existingStatistic = await _statisticRepository.GetByIdAsync(statisticId, cancellationToken);

            if (existingStatistic is null)
            {
                _logger.LogWarning("Statistic with ID {StatisticId} not found for update", statisticId);
                return ServiceResult<PlayerStatisticDto>.Fail($"Statistic with ID {statisticId} could not be found.");
            }

            // Verify team player exists if changed
            if (existingStatistic.TeamPlayerId != updateDto.TeamPlayerId)
            {
                var teamPlayerExists = await _teamPlayerRepository.ExistsAsync(updateDto.TeamPlayerId, cancellationToken);
                if (!teamPlayerExists)
                {
                    _logger.LogWarning("Team player {TeamPlayerId} not found when updating statistic", updateDto.TeamPlayerId);
                    return ServiceResult<PlayerStatisticDto>.Fail($"Team player with ID {updateDto.TeamPlayerId} could not be found.");
                }
            }

            // Create updated entity
            var updatedStatistic = new PlayerStatistic
            {
                PlayerStatisticId = existingStatistic.PlayerStatisticId,
                TeamPlayerId = updateDto.TeamPlayerId,
                GameDate = updateDto.GameDate,
                MinutesPlayed = updateDto.MinutesPlayed,
                IsStarter = updateDto.IsStarter,
                JerseyNumber = updateDto.JerseyNumber,
                Goals = updateDto.Goals,
                Assists = updateDto.Assists,
                CreatedBy = existingStatistic.CreatedBy,
                CreatedAt = existingStatistic.CreatedAt
            };
            updatedStatistic.UpdateLastModified(currentUserId);

            var savedStatistic = await _statisticRepository.UpdateAsync(updatedStatistic, cancellationToken);
            var dto = PlayerStatisticDto.FromEntity(savedStatistic);

            _logger.LogInformation("Successfully updated statistic {StatisticId}", statisticId);

            return ServiceResult<PlayerStatisticDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating statistic {StatisticId}", statisticId);
            return ServiceResult<PlayerStatisticDto>.Fail("Unable to save statistic. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<bool>> DeleteStatisticAsync(
        int statisticId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting statistic with ID {StatisticId}", statisticId);

        try
        {
            var exists = await _statisticRepository.ExistsAsync(statisticId, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("Statistic with ID {StatisticId} not found for deletion", statisticId);
                return ServiceResult<bool>.Fail($"Statistic with ID {statisticId} could not be found.");
            }

            var deleted = await _statisticRepository.DeleteAsync(statisticId, cancellationToken);

            if (deleted)
            {
                _logger.LogInformation("Successfully deleted statistic {StatisticId}", statisticId);
                return ServiceResult<bool>.Ok(true);
            }

            _logger.LogWarning("Failed to delete statistic {StatisticId}", statisticId);
            return ServiceResult<bool>.Fail("Unable to delete statistic. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting statistic {StatisticId}", statisticId);
            return ServiceResult<bool>.Fail("Unable to delete statistic. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerStatisticAggregateResult>> GetPlayerAggregatesAsync(
        int playerId,
        int? teamPlayerId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving aggregates for player {PlayerId}, teamPlayerId: {TeamPlayerId}",
            playerId, teamPlayerId?.ToString() ?? "all");

        try
        {
            var aggregates = await _statisticRepository.GetAggregatesAsync(playerId, teamPlayerId, cancellationToken);

            _logger.LogInformation("Retrieved aggregates for player {PlayerId}: Games={GameCount}, Goals={TotalGoals}, Assists={TotalAssists}",
                playerId, aggregates.GameCount, aggregates.TotalGoals, aggregates.TotalAssists);

            return ServiceResult<PlayerStatisticAggregateResult>.Ok(aggregates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving aggregates for player {PlayerId}", playerId);
            return ServiceResult<PlayerStatisticAggregateResult>.Fail("Unable to load aggregates. Please try again.");
        }
    }

    /// <inheritdoc/>
    public Task<ValidationResult> ValidateStatisticAsync(
        CreatePlayerStatisticDto createDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        _logger.LogDebug("Validating statistic data for team player {TeamPlayerId}", createDto.TeamPlayerId);

        var validationResult = PlayerStatisticValidator.ValidateCreatePlayerStatistic(createDto);

        if (!validationResult.IsValid)
        {
            _logger.LogDebug("Validation failed for team player {TeamPlayerId}: {Errors}",
                createDto.TeamPlayerId,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));
        }

        return Task.FromResult(validationResult);
    }
}
