using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services.PlayerManagement;

public class PlayerServiceTests
{
    private readonly Mock<IPlayerRepository> _mockRepository;
    private readonly Mock<ILogger<PlayerService>> _mockLogger;
    private readonly PlayerService _service;

    public PlayerServiceTests()
    {
        _mockRepository = new Mock<IPlayerRepository>();
        _mockLogger = new Mock<ILogger<PlayerService>>();
        _service = new PlayerService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when repository is null")]
    public void Constructor_WhenRepositoryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerService(null!, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when logger is null")]
    public void Constructor_WhenLoggerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerService(_mockRepository.Object, null!));
    }

    [Fact(DisplayName = "GetAllPlayersAsync returns all players when players exist")]
    public async Task GetAllPlayersAsync_WhenPlayersExist_ReturnsAllPlayers()
    {
        var players = new List<Player>
        {
            TestPlayerFactory.CreateCustomPlayer("Player 1", new DateTime(2010, 1, 1)),
            TestPlayerFactory.CreateCustomPlayer("Player 2", new DateTime(2012, 6, 15))
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        var result = await _service.GetAllPlayersAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact(DisplayName = "GetAllPlayersAsync returns empty list when no players exist")]
    public async Task GetAllPlayersAsync_WhenNoPlayers_ReturnsEmptyList()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Player>());

        var result = await _service.GetAllPlayersAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact(DisplayName = "GetAllPlayersAsync returns failure when repository throws exception")]
    public async Task GetAllPlayersAsync_WhenRepositoryThrows_ReturnsFailure()
    {
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var result = await _service.GetAllPlayersAsync();

        Assert.False(result.Success);
        Assert.Contains("Unable to load players", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "GetPlayerByIdAsync returns player when player exists")]
    public async Task GetPlayerByIdAsync_WhenPlayerExists_ReturnsPlayer()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var result = await _service.GetPlayerByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal(player.Name, result.Data.Name);
    }

    [Fact(DisplayName = "GetPlayerByIdAsync returns failure when player not found")]
    public async Task GetPlayerByIdAsync_WhenPlayerNotFound_ReturnsFailResult()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var result = await _service.GetPlayerByIdAsync(999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "GetPlayerByIdAsync returns failure when repository throws exception")]
    public async Task GetPlayerByIdAsync_WhenRepositoryThrows_ReturnsFailure()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var result = await _service.GetPlayerByIdAsync(1);

        Assert.False(result.Success);
        Assert.Contains("Unable to load player", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "CreatePlayerAsync creates player with valid data")]
    public async Task CreatePlayerAsync_WithValidData_CreatesPlayerSuccessfully()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "New Player",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Male"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken _) =>
            {
                p.Id = 1;
                return p;
            });

        var result = await _service.CreatePlayerAsync(createDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("New Player", result.Data.Name);
    }

    [Fact(DisplayName = "CreatePlayerAsync returns validation error with invalid name")]
    public async Task CreatePlayerAsync_WithInvalidName_ReturnsValidationError()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = await _service.CreatePlayerAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("Name"));
    }

    [Fact(DisplayName = "CreatePlayerAsync returns validation error with future date of birth")]
    public async Task CreatePlayerAsync_WithFutureDateOfBirth_ReturnsValidationError()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "Test Player",
            DateOfBirth = DateTime.UtcNow.AddYears(1)
        };

        var result = await _service.CreatePlayerAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("DateOfBirth"));
    }

    [Fact(DisplayName = "CreatePlayerAsync throws when createDto is null")]
    public async Task CreatePlayerAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.CreatePlayerAsync(null!, "admin-user"));
    }

    [Fact(DisplayName = "CreatePlayerAsync throws when currentUserId is null or empty")]
    public async Task CreatePlayerAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "Test Player",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreatePlayerAsync(createDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreatePlayerAsync(createDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreatePlayerAsync(createDto, "   "));
    }

    [Fact(DisplayName = "UpdatePlayerAsync updates player with valid data")]
    public async Task UpdatePlayerAsync_WithValidData_UpdatesPlayerSuccessfully()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        existingPlayer.Id = 1;

        var updateDto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "Updated Name",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Female"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlayer);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken _) => p);

        var result = await _service.UpdatePlayerAsync(1, updateDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Updated Name", result.Data.Name);
    }

    [Fact(DisplayName = "UpdatePlayerAsync returns failure when player not found")]
    public async Task UpdatePlayerAsync_WhenPlayerNotFound_ReturnsFailResult()
    {
        var updateDto = new UpdatePlayerDto
        {
            Id = 999,
            Name = "Updated Name",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var result = await _service.UpdatePlayerAsync(999, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "UpdatePlayerAsync returns failure when ID mismatch")]
    public async Task UpdatePlayerAsync_WithIdMismatch_ReturnsFailure()
    {
        var updateDto = new UpdatePlayerDto
        {
            Id = 2,
            Name = "Updated Name",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = await _service.UpdatePlayerAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("mismatch", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "UpdatePlayerAsync returns validation error with invalid data")]
    public async Task UpdatePlayerAsync_WithInvalidData_ReturnsValidationError()
    {
        var updateDto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = await _service.UpdatePlayerAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("Name"));
    }

    [Fact(DisplayName = "UpdatePlayerAsync throws when updateDto is null")]
    public async Task UpdatePlayerAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdatePlayerAsync(1, null!, "admin-user"));
    }

    [Fact(DisplayName = "UpdatePlayerAsync throws when currentUserId is null or empty")]
    public async Task UpdatePlayerAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var updateDto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "Test Player",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdatePlayerAsync(1, updateDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdatePlayerAsync(1, updateDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdatePlayerAsync(1, updateDto, "   "));
    }

    [Fact(DisplayName = "DeletePlayerAsync deletes existing player successfully")]
    public async Task DeletePlayerAsync_WhenPlayerExists_DeletesSuccessfully()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.DeletePlayerAsync(1);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact(DisplayName = "DeletePlayerAsync returns failure when player not found")]
    public async Task DeletePlayerAsync_WhenPlayerNotFound_ReturnsFailure()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeletePlayerAsync(999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "DeletePlayerAsync returns failure when delete operation fails")]
    public async Task DeletePlayerAsync_WhenDeleteFails_ReturnsFailure()
    {
        _mockRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeletePlayerAsync(1);

        Assert.False(result.Success);
        Assert.Contains("Unable to delete", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "ValidatePlayerAsync returns valid result for valid data")]
    public async Task ValidatePlayerAsync_WithValidData_ReturnsValidResult()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "Valid Player",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Male"
        };

        var result = await _service.ValidatePlayerAsync(createDto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidatePlayerAsync returns invalid result for invalid data")]
    public async Task ValidatePlayerAsync_WithInvalidData_ReturnsInvalidResult()
    {
        var createDto = new CreatePlayerDto
        {
            UserId = "",
            Name = "",
            DateOfBirth = DateTime.UtcNow.AddYears(1)
        };

        var result = await _service.ValidatePlayerAsync(createDto);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    [Fact(DisplayName = "ValidatePlayerAsync throws when createDto is null")]
    public async Task ValidatePlayerAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ValidatePlayerAsync(null!));
    }
}
