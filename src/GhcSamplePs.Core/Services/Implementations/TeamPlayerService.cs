using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Validation;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service implementation for team player management operations.
/// Coordinates between presentation and data layers, applies business rules and validation.
/// </summary>
public sealed class TeamPlayerService : ITeamPlayerService
{
    private readonly ITeamPlayerRepository _teamPlayerRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<TeamPlayerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamPlayerService"/> class.
    /// </summary>
    /// <param name="teamPlayerRepository">The team player repository for data access.</param>
    /// <param name="playerRepository">The player repository for player validation.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public TeamPlayerService(
        ITeamPlayerRepository teamPlayerRepository,
        IPlayerRepository playerRepository,
        ILogger<TeamPlayerService> logger)
    {
        ArgumentNullException.ThrowIfNull(teamPlayerRepository);
        ArgumentNullException.ThrowIfNull(playerRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _teamPlayerRepository = teamPlayerRepository;
        _playerRepository = playerRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<TeamPlayerDto>>> GetTeamsByPlayerIdAsync(
        int playerId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving team assignments for player {PlayerId}, includeInactive: {IncludeInactive}",
            playerId, includeInactive);

        try
        {
            IReadOnlyList<TeamPlayer> teamPlayers;

            if (includeInactive)
            {
                teamPlayers = await _teamPlayerRepository.GetAllByPlayerIdAsync(playerId, cancellationToken);
            }
            else
            {
                teamPlayers = await _teamPlayerRepository.GetActiveByPlayerIdAsync(playerId, cancellationToken);
            }

            var dtos = teamPlayers.Select(TeamPlayerDto.FromEntity).ToList();

            _logger.LogInformation("Retrieved {Count} team assignments for player {PlayerId}",
                dtos.Count, playerId);

            return ServiceResult<IReadOnlyList<TeamPlayerDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team assignments for player {PlayerId}", playerId);
            return ServiceResult<IReadOnlyList<TeamPlayerDto>>.Fail("Unable to load team assignments. Please refresh the page.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<TeamPlayerDto>>> GetActiveTeamsByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving active team assignments for player {PlayerId}", playerId);

        try
        {
            var teamPlayers = await _teamPlayerRepository.GetActiveByPlayerIdAsync(playerId, cancellationToken);
            var dtos = teamPlayers.Select(TeamPlayerDto.FromEntity).ToList();

            _logger.LogInformation("Retrieved {Count} active team assignments for player {PlayerId}",
                dtos.Count, playerId);

            return ServiceResult<IReadOnlyList<TeamPlayerDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active team assignments for player {PlayerId}", playerId);
            return ServiceResult<IReadOnlyList<TeamPlayerDto>>.Fail("Unable to load team assignments. Please refresh the page.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TeamPlayerDto>> GetTeamAssignmentByIdAsync(
        int teamPlayerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving team assignment with ID {TeamPlayerId}", teamPlayerId);

        try
        {
            var teamPlayer = await _teamPlayerRepository.GetByIdAsync(teamPlayerId, cancellationToken);

            if (teamPlayer is null)
            {
                _logger.LogWarning("Team assignment with ID {TeamPlayerId} not found", teamPlayerId);
                return ServiceResult<TeamPlayerDto>.Fail($"Team assignment with ID {teamPlayerId} could not be found.");
            }

            var dto = TeamPlayerDto.FromEntity(teamPlayer);

            _logger.LogInformation("Successfully retrieved team assignment {TeamPlayerId}: {TeamName}",
                teamPlayerId, teamPlayer.TeamName);

            return ServiceResult<TeamPlayerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving team assignment with ID {TeamPlayerId}", teamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail("Unable to load team assignment. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TeamPlayerDto>> AddPlayerToTeamAsync(
        CreateTeamPlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Adding player {PlayerId} to team {TeamName} by user {UserId}",
            createDto.PlayerId, createDto.TeamName, currentUserId);

        var validationResult = TeamPlayerValidator.ValidateCreateTeamPlayer(createDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Team player creation validation failed for player {PlayerId}: {Errors}",
                createDto.PlayerId,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

            return ServiceResult<TeamPlayerDto>.ValidationFailed(validationResult);
        }

        try
        {
            // Verify player exists
            var playerExists = await _playerRepository.ExistsAsync(createDto.PlayerId, cancellationToken);
            if (!playerExists)
            {
                _logger.LogWarning("Player {PlayerId} not found when adding to team", createDto.PlayerId);
                return ServiceResult<TeamPlayerDto>.Fail($"Player with ID {createDto.PlayerId} could not be found.");
            }

            // Check for duplicate active assignment
            var hasDuplicate = await _teamPlayerRepository.HasActiveDuplicateAsync(
                createDto.PlayerId,
                createDto.TeamName.Trim(),
                createDto.ChampionshipName.Trim(),
                null,
                cancellationToken);

            if (hasDuplicate)
            {
                _logger.LogWarning("Duplicate active assignment found for player {PlayerId} on team {TeamName} in {ChampionshipName}",
                    createDto.PlayerId, createDto.TeamName, createDto.ChampionshipName);

                return ServiceResult<TeamPlayerDto>.ValidationFailed("DuplicateAssignment",
                    "Player already has an active assignment to this team and championship.");
            }

            var teamPlayer = createDto.ToEntity(currentUserId);
            var createdTeamPlayer = await _teamPlayerRepository.AddAsync(teamPlayer, cancellationToken);
            var dto = TeamPlayerDto.FromEntity(createdTeamPlayer);

            _logger.LogInformation("Successfully added player {PlayerId} to team {TeamName} with ID {TeamPlayerId}",
                createDto.PlayerId, createdTeamPlayer.TeamName, createdTeamPlayer.TeamPlayerId);

            return ServiceResult<TeamPlayerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding player {PlayerId} to team {TeamName}",
                createDto.PlayerId, createDto.TeamName);
            return ServiceResult<TeamPlayerDto>.Fail("Unable to save team assignment. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TeamPlayerDto>> UpdateTeamAssignmentAsync(
        int teamPlayerId,
        UpdateTeamPlayerDto updateDto,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Updating team assignment {TeamPlayerId} by user {UserId}",
            teamPlayerId, currentUserId);

        if (updateDto.TeamPlayerId != teamPlayerId)
        {
            _logger.LogWarning("Team player ID mismatch: URL ID {UrlId} != DTO ID {DtoId}",
                teamPlayerId, updateDto.TeamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail("Team player ID mismatch.");
        }

        try
        {
            var existingTeamPlayer = await _teamPlayerRepository.GetByIdAsync(teamPlayerId, cancellationToken);

            if (existingTeamPlayer is null)
            {
                _logger.LogWarning("Team assignment with ID {TeamPlayerId} not found for update", teamPlayerId);
                return ServiceResult<TeamPlayerDto>.Fail($"Team assignment with ID {teamPlayerId} could not be found.");
            }

            var validationResult = TeamPlayerValidator.ValidateUpdateTeamPlayer(updateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Team player update validation failed for ID {TeamPlayerId}: {Errors}",
                    teamPlayerId,
                    string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

                return ServiceResult<TeamPlayerDto>.ValidationFailed(validationResult);
            }

            // Apply update
            if (updateDto.LeftDate.HasValue)
            {
                existingTeamPlayer.MarkAsLeft(updateDto.LeftDate.Value, currentUserId);
            }
            else
            {
                existingTeamPlayer.UpdateLastModified(currentUserId);
            }

            var updatedTeamPlayer = await _teamPlayerRepository.UpdateAsync(existingTeamPlayer, cancellationToken);
            var dto = TeamPlayerDto.FromEntity(updatedTeamPlayer);

            _logger.LogInformation("Successfully updated team assignment {TeamPlayerId}", teamPlayerId);

            return ServiceResult<TeamPlayerDto>.Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation when updating team assignment {TeamPlayerId}", teamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team assignment {TeamPlayerId}", teamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail("Unable to save team assignment. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<TeamPlayerDto>> RemovePlayerFromTeamAsync(
        int teamPlayerId,
        DateTime leftDate,
        string currentUserId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Removing player from team assignment {TeamPlayerId} by user {UserId}",
            teamPlayerId, currentUserId);

        try
        {
            var existingTeamPlayer = await _teamPlayerRepository.GetByIdAsync(teamPlayerId, cancellationToken);

            if (existingTeamPlayer is null)
            {
                _logger.LogWarning("Team assignment with ID {TeamPlayerId} not found for removal", teamPlayerId);
                return ServiceResult<TeamPlayerDto>.Fail($"Team assignment with ID {teamPlayerId} could not be found.");
            }

            // Validate left date
            var errors = new Dictionary<string, List<string>>();
            if (leftDate <= existingTeamPlayer.JoinedDate)
            {
                _logger.LogWarning("Invalid left date for team assignment {TeamPlayerId}: Left date must be after joined date",
                    teamPlayerId);
                return ServiceResult<TeamPlayerDto>.ValidationFailed("LeftDate", "Left date must be after the joined date");
            }

            if (leftDate > DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid left date for team assignment {TeamPlayerId}: Left date cannot be in the future",
                    teamPlayerId);
                return ServiceResult<TeamPlayerDto>.ValidationFailed("LeftDate", "Left date cannot be in the future");
            }

            existingTeamPlayer.MarkAsLeft(leftDate, currentUserId);

            var updatedTeamPlayer = await _teamPlayerRepository.UpdateAsync(existingTeamPlayer, cancellationToken);
            var dto = TeamPlayerDto.FromEntity(updatedTeamPlayer);

            _logger.LogInformation("Successfully removed player from team assignment {TeamPlayerId}, IsActive: {IsActive}",
                teamPlayerId, updatedTeamPlayer.IsActive);

            return ServiceResult<TeamPlayerDto>.Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation when removing player from team assignment {TeamPlayerId}", teamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing player from team assignment {TeamPlayerId}", teamPlayerId);
            return ServiceResult<TeamPlayerDto>.Fail("Unable to update team assignment. Please try again.");
        }
    }

    /// <inheritdoc/>
    public Task<ValidationResult> ValidateTeamAssignmentAsync(
        CreateTeamPlayerDto createDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        _logger.LogDebug("Validating team assignment data for player {PlayerId}", createDto.PlayerId);

        var validationResult = TeamPlayerValidator.ValidateCreateTeamPlayer(createDto);

        if (!validationResult.IsValid)
        {
            _logger.LogDebug("Validation failed for player {PlayerId}: {Errors}",
                createDto.PlayerId,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));
        }

        return Task.FromResult(validationResult);
    }
}
