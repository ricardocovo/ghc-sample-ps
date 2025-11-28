using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Validation;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service implementation for player management operations.
/// Coordinates between presentation and data layers, applies business rules and validation.
/// </summary>
public sealed class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _repository;
    private readonly ILogger<PlayerService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerService"/> class.
    /// </summary>
    /// <param name="repository">The player repository for data access.</param>
    /// <param name="logger">The logger for operation logging.</param>
    /// <exception cref="ArgumentNullException">Thrown when repository or logger is null.</exception>
    public PlayerService(IPlayerRepository repository, ILogger<PlayerService> logger)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(logger);

        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<IReadOnlyList<PlayerDto>>> GetAllPlayersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all players");

        try
        {
            var players = await _repository.GetAllAsync(cancellationToken);
            var playerDtos = players.Select(PlayerDto.FromEntity).ToList();

            _logger.LogInformation("Retrieved {PlayerCount} players", playerDtos.Count);

            return ServiceResult<IReadOnlyList<PlayerDto>>.Ok(playerDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all players");
            return ServiceResult<IReadOnlyList<PlayerDto>>.Fail("Unable to load players. Please refresh the page.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerDto>> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving player with ID {PlayerId}", id);

        try
        {
            var player = await _repository.GetByIdAsync(id, cancellationToken);

            if (player is null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found", id);
                return ServiceResult<PlayerDto>.Fail($"Player with ID {id} could not be found.");
            }

            var dto = PlayerDto.FromEntity(player);

            _logger.LogInformation("Successfully retrieved player {PlayerId}: {PlayerName}", id, player.Name);

            return ServiceResult<PlayerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player with ID {PlayerId}", id);
            return ServiceResult<PlayerDto>.Fail("Unable to load player. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerDto>> CreatePlayerAsync(CreatePlayerDto createDto, string currentUserId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Creating new player: {PlayerName} by user {UserId}", createDto.Name, currentUserId);

        var validationResult = PlayerValidator.Validate(createDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Player creation validation failed for {PlayerName}: {Errors}",
                createDto.Name,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

            return ServiceResult<PlayerDto>.ValidationFailed(validationResult);
        }

        try
        {
            var player = createDto.ToEntity(currentUserId);
            var createdPlayer = await _repository.AddAsync(player, cancellationToken);
            var dto = PlayerDto.FromEntity(createdPlayer);

            _logger.LogInformation("Successfully created player {PlayerId}: {PlayerName}", createdPlayer.Id, createdPlayer.Name);

            return ServiceResult<PlayerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating player: {PlayerName}", createDto.Name);
            return ServiceResult<PlayerDto>.Fail("Unable to save player. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<PlayerDto>> UpdatePlayerAsync(int id, UpdatePlayerDto updateDto, string currentUserId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updateDto);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("Current user ID cannot be null or whitespace.", nameof(currentUserId));
        }

        _logger.LogInformation("Updating player {PlayerId} by user {UserId}", id, currentUserId);

        if (updateDto.Id != id)
        {
            _logger.LogWarning("Player ID mismatch: URL ID {UrlId} != DTO ID {DtoId}", id, updateDto.Id);
            return ServiceResult<PlayerDto>.Fail("Player ID mismatch.");
        }

        var validationResult = PlayerValidator.Validate(updateDto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Player update validation failed for ID {PlayerId}: {Errors}",
                id,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));

            return ServiceResult<PlayerDto>.ValidationFailed(validationResult);
        }

        try
        {
            var existingPlayer = await _repository.GetByIdAsync(id, cancellationToken);

            if (existingPlayer is null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for update", id);
                return ServiceResult<PlayerDto>.Fail($"Player with ID {id} could not be found.");
            }

            var updatedPlayer = updateDto.ApplyTo(existingPlayer, currentUserId);
            var savedPlayer = await _repository.UpdateAsync(updatedPlayer, cancellationToken);
            var dto = PlayerDto.FromEntity(savedPlayer);

            _logger.LogInformation("Successfully updated player {PlayerId}: {PlayerName}", savedPlayer.Id, savedPlayer.Name);

            return ServiceResult<PlayerDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating player {PlayerId}", id);
            return ServiceResult<PlayerDto>.Fail("Unable to save player. Please try again.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<bool>> DeletePlayerAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting player with ID {PlayerId}", id);

        try
        {
            var exists = await _repository.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for deletion", id);
                return ServiceResult<bool>.Fail($"Player with ID {id} could not be found.");
            }

            var deleted = await _repository.DeleteAsync(id, cancellationToken);

            if (deleted)
            {
                _logger.LogInformation("Successfully deleted player {PlayerId}", id);
                return ServiceResult<bool>.Ok(true);
            }

            _logger.LogWarning("Failed to delete player {PlayerId}", id);
            return ServiceResult<bool>.Fail("Unable to delete player. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting player {PlayerId}", id);
            return ServiceResult<bool>.Fail("Unable to delete player. Please try again.");
        }
    }

    /// <inheritdoc/>
    public Task<ValidationResult> ValidatePlayerAsync(CreatePlayerDto createDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createDto);

        _logger.LogDebug("Validating player data for {PlayerName}", createDto.Name);

        var validationResult = PlayerValidator.Validate(createDto);

        if (!validationResult.IsValid)
        {
            _logger.LogDebug("Validation failed for {PlayerName}: {Errors}",
                createDto.Name,
                string.Join(", ", validationResult.Errors.SelectMany(e => e.Value)));
        }

        return Task.FromResult(validationResult);
    }
}
