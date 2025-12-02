using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services.PlayerManagement;

public class PlayerStatisticServiceTests
{
    private readonly Mock<IPlayerStatisticRepository> _mockStatisticRepository;
    private readonly Mock<ITeamPlayerRepository> _mockTeamPlayerRepository;
    private readonly Mock<ILogger<PlayerStatisticService>> _mockLogger;
    private readonly PlayerStatisticService _service;

    public PlayerStatisticServiceTests()
    {
        _mockStatisticRepository = new Mock<IPlayerStatisticRepository>();
        _mockTeamPlayerRepository = new Mock<ITeamPlayerRepository>();
        _mockLogger = new Mock<ILogger<PlayerStatisticService>>();
        _service = new PlayerStatisticService(
            _mockStatisticRepository.Object,
            _mockTeamPlayerRepository.Object,
            _mockLogger.Object);
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when statisticRepository is null")]
    public void Constructor_WhenStatisticRepositoryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerStatisticService(null!, _mockTeamPlayerRepository.Object, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when teamPlayerRepository is null")]
    public void Constructor_WhenTeamPlayerRepositoryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerStatisticService(_mockStatisticRepository.Object, null!, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when logger is null")]
    public void Constructor_WhenLoggerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerStatisticService(_mockStatisticRepository.Object, _mockTeamPlayerRepository.Object, null!));
    }

    [Fact(DisplayName = "GetStatisticsByPlayerIdAsync with stats returns all")]
    public async Task GetStatisticsByPlayerIdAsync_WhenStatsExist_ReturnsAllStats()
    {
        var playerId = 1;
        var statistics = TestPlayerStatisticFactory.CreatePlayerStatisticList();

        _mockStatisticRepository.Setup(r => r.GetAllByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistics);

        var result = await _service.GetStatisticsByPlayerIdAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact(DisplayName = "GetStatisticsByPlayerIdAsync with no stats returns empty")]
    public async Task GetStatisticsByPlayerIdAsync_WhenNoStats_ReturnsEmptyList()
    {
        var playerId = 999;
        _mockStatisticRepository.Setup(r => r.GetAllByPlayerIdAsync(playerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayerStatistic>());

        var result = await _service.GetStatisticsByPlayerIdAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact(DisplayName = "GetStatisticsByTeamPlayerIdAsync filters correctly")]
    public async Task GetStatisticsByTeamPlayerIdAsync_WhenCalled_ReturnsFilteredStats()
    {
        var teamPlayerId = 1;
        var statistics = TestPlayerStatisticFactory.CreatePlayerStatisticList();

        _mockStatisticRepository.Setup(r => r.GetAllByTeamPlayerIdAsync(teamPlayerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistics);

        var result = await _service.GetStatisticsByTeamPlayerIdAsync(teamPlayerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact(DisplayName = "GetStatisticByIdAsync when exists returns it")]
    public async Task GetStatisticByIdAsync_WhenExists_ReturnsStatistic()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();
        _mockStatisticRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(statistic);

        var result = await _service.GetStatisticByIdAsync(1);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(statistic.PlayerStatisticId, result.Data.PlayerStatisticId);
        Assert.Equal(statistic.Goals, result.Data.Goals);
    }

    [Fact(DisplayName = "GetStatisticByIdAsync when not found returns fail")]
    public async Task GetStatisticByIdAsync_WhenNotFound_ReturnsFailResult()
    {
        _mockStatisticRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerStatistic?)null);

        var result = await _service.GetStatisticByIdAsync(999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "AddStatisticAsync with valid data creates successfully")]
    public async Task AddStatisticAsync_WithValidData_CreatesSuccessfully()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        _mockTeamPlayerRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockStatisticRepository.Setup(r => r.AddAsync(It.IsAny<PlayerStatistic>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerStatistic stat, CancellationToken _) =>
            {
                stat.PlayerStatisticId = 1;
                return stat;
            });

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Goals);
        Assert.Equal(1, result.Data.Assists);
    }

    [Fact(DisplayName = "AddStatisticAsync with negative goals returns validation error")]
    public async Task AddStatisticAsync_WithNegativeGoals_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = -1,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("Goals"));
    }

    [Fact(DisplayName = "AddStatisticAsync with negative assists returns validation error")]
    public async Task AddStatisticAsync_WithNegativeAssists_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = -1
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("Assists"));
    }

    [Fact(DisplayName = "AddStatisticAsync with negative minutes returns validation error")]
    public async Task AddStatisticAsync_WithNegativeMinutes_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = -1,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("MinutesPlayed"));
    }

    [Fact(DisplayName = "AddStatisticAsync with zero jersey returns validation error")]
    public async Task AddStatisticAsync_WithZeroJersey_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 0,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("JerseyNumber"));
    }

    [Fact(DisplayName = "AddStatisticAsync with future date returns validation error")]
    public async Task AddStatisticAsync_WithFutureDate_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("GameDate"));
    }

    [Fact(DisplayName = "AddStatisticAsync with invalid TeamPlayerId returns validation error")]
    public async Task AddStatisticAsync_WithInvalidTeamPlayerId_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 0,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("TeamPlayerId"));
    }

    [Fact(DisplayName = "AddStatisticAsync when team player not found returns fail")]
    public async Task AddStatisticAsync_WhenTeamPlayerNotFound_ReturnsFailure()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 999,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        _mockTeamPlayerRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "UpdateStatisticAsync with valid data updates")]
    public async Task UpdateStatisticAsync_WithValidData_UpdatesSuccessfully()
    {
        var existingStatistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();
        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 3,
            Assists = 2
        };

        _mockStatisticRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStatistic);
        _mockStatisticRepository.Setup(r => r.UpdateAsync(It.IsAny<PlayerStatistic>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerStatistic stat, CancellationToken _) => stat);

        var result = await _service.UpdateStatisticAsync(1, updateDto, "admin-user");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Goals);
        Assert.Equal(2, result.Data.Assists);
    }

    [Fact(DisplayName = "UpdateStatisticAsync with invalid data returns error")]
    public async Task UpdateStatisticAsync_WithInvalidData_ReturnsValidationError()
    {
        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = -1,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.UpdateStatisticAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("MinutesPlayed"));
    }

    [Fact(DisplayName = "UpdateStatisticAsync when not found returns fail")]
    public async Task UpdateStatisticAsync_WhenNotFound_ReturnsFailResult()
    {
        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 999,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        _mockStatisticRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlayerStatistic?)null);

        var result = await _service.UpdateStatisticAsync(999, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "UpdateStatisticAsync returns failure when ID mismatch")]
    public async Task UpdateStatisticAsync_WithIdMismatch_ReturnsFailure()
    {
        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 2,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.UpdateStatisticAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("mismatch", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "DeleteStatisticAsync removes successfully")]
    public async Task DeleteStatisticAsync_WhenExists_RemovesSuccessfully()
    {
        _mockStatisticRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockStatisticRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.DeleteStatisticAsync(1);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact(DisplayName = "DeleteStatisticAsync when not found returns fail")]
    public async Task DeleteStatisticAsync_WhenNotFound_ReturnsFailResult()
    {
        _mockStatisticRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeleteStatisticAsync(999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    [Fact(DisplayName = "GetPlayerAggregatesAsync calculates totals correctly")]
    public async Task GetPlayerAggregatesAsync_WhenCalled_CalculatesTotalsCorrectly()
    {
        var playerId = 1;
        var aggregates = new PlayerStatisticAggregateResult
        {
            GameCount = 3,
            TotalGoals = 5,
            TotalAssists = 6,
            TotalMinutesPlayed = 195,
            AverageGoals = 1.67,
            AverageAssists = 2.0,
            AverageMinutesPlayed = 65.0
        };

        _mockStatisticRepository.Setup(r => r.GetAggregatesAsync(playerId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        var result = await _service.GetPlayerAggregatesAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.GameCount);
        Assert.Equal(5, result.Data.TotalGoals);
        Assert.Equal(6, result.Data.TotalAssists);
        Assert.Equal(195, result.Data.TotalMinutesPlayed);
    }

    [Fact(DisplayName = "GetPlayerAggregatesAsync calculates averages correctly")]
    public async Task GetPlayerAggregatesAsync_WhenCalled_CalculatesAveragesCorrectly()
    {
        var playerId = 1;
        var aggregates = new PlayerStatisticAggregateResult
        {
            GameCount = 3,
            TotalGoals = 5,
            TotalAssists = 6,
            TotalMinutesPlayed = 195,
            AverageGoals = 1.67,
            AverageAssists = 2.0,
            AverageMinutesPlayed = 65.0
        };

        _mockStatisticRepository.Setup(r => r.GetAggregatesAsync(playerId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        var result = await _service.GetPlayerAggregatesAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1.67, result.Data.AverageGoals);
        Assert.Equal(2.0, result.Data.AverageAssists);
        Assert.Equal(65.0, result.Data.AverageMinutesPlayed);
    }

    [Fact(DisplayName = "GetPlayerAggregatesAsync with no stats returns zeros")]
    public async Task GetPlayerAggregatesAsync_WhenNoStats_ReturnsZeros()
    {
        var playerId = 999;
        var emptyAggregates = PlayerStatisticAggregateResult.Empty();

        _mockStatisticRepository.Setup(r => r.GetAggregatesAsync(playerId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyAggregates);

        var result = await _service.GetPlayerAggregatesAsync(playerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data.GameCount);
        Assert.Equal(0, result.Data.TotalGoals);
        Assert.Equal(0, result.Data.TotalAssists);
        Assert.Equal(0, result.Data.AverageGoals);
    }

    [Fact(DisplayName = "GetPlayerAggregatesAsync filters by teamPlayerId")]
    public async Task GetPlayerAggregatesAsync_WithTeamPlayerId_FiltersCorrectly()
    {
        var playerId = 1;
        var teamPlayerId = 1;
        var aggregates = new PlayerStatisticAggregateResult
        {
            GameCount = 2,
            TotalGoals = 3,
            TotalAssists = 2,
            TotalMinutesPlayed = 135,
            AverageGoals = 1.5,
            AverageAssists = 1.0,
            AverageMinutesPlayed = 67.5
        };

        _mockStatisticRepository.Setup(r => r.GetAggregatesAsync(playerId, teamPlayerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aggregates);

        var result = await _service.GetPlayerAggregatesAsync(playerId, teamPlayerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.GameCount);

        _mockStatisticRepository.Verify(r => r.GetAggregatesAsync(playerId, teamPlayerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "AddStatisticAsync throws when createDto is null")]
    public async Task AddStatisticAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.AddStatisticAsync(null!, "admin-user"));
    }

    [Fact(DisplayName = "AddStatisticAsync throws when currentUserId is null or empty")]
    public async Task AddStatisticAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddStatisticAsync(createDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddStatisticAsync(createDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddStatisticAsync(createDto, "   "));
    }

    [Fact(DisplayName = "UpdateStatisticAsync throws when updateDto is null")]
    public async Task UpdateStatisticAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateStatisticAsync(1, null!, "admin-user"));
    }

    [Fact(DisplayName = "UpdateStatisticAsync throws when currentUserId is null or empty")]
    public async Task UpdateStatisticAsync_WhenCurrentUserIdNullOrEmpty_ThrowsArgumentException()
    {
        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateStatisticAsync(1, updateDto, null!));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateStatisticAsync(1, updateDto, ""));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateStatisticAsync(1, updateDto, "   "));
    }

    [Fact(DisplayName = "ValidateStatisticAsync with valid data returns success")]
    public async Task ValidateStatisticAsync_WithValidData_ReturnsSuccess()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = await _service.ValidateStatisticAsync(createDto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateStatisticAsync throws when createDto is null")]
    public async Task ValidateStatisticAsync_WhenDtoNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ValidateStatisticAsync(null!));
    }

    [Fact(DisplayName = "AddStatisticAsync with jersey number over 99 returns validation error")]
    public async Task AddStatisticAsync_WithJerseyOverMax_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 100,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("JerseyNumber"));
    }

    [Fact(DisplayName = "AddStatisticAsync with minutes over 120 returns validation error")]
    public async Task AddStatisticAsync_WithMinutesOverMax_ReturnsValidationError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 121,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _service.AddStatisticAsync(createDto, "admin-user");

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("MinutesPlayed"));
    }

    [Fact(DisplayName = "UpdateStatisticAsync with changed team player verifies team player exists")]
    public async Task UpdateStatisticAsync_WithChangedTeamPlayer_VerifiesTeamPlayerExists()
    {
        var existingStatistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();
        existingStatistic.PlayerStatisticId = 1;

        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 2, // Different from existing
            GameDate = DateTime.UtcNow.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        _mockStatisticRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingStatistic);
        _mockTeamPlayerRepository.Setup(r => r.ExistsAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.UpdateStatisticAsync(1, updateDto, "admin-user");

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
        _mockTeamPlayerRepository.Verify(r => r.ExistsAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "GetStatisticsByTeamPlayerIdAsync with no stats returns empty list")]
    public async Task GetStatisticsByTeamPlayerIdAsync_WhenNoStats_ReturnsEmptyList()
    {
        var teamPlayerId = 999;
        _mockStatisticRepository.Setup(r => r.GetAllByTeamPlayerIdAsync(teamPlayerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PlayerStatistic>());

        var result = await _service.GetStatisticsByTeamPlayerIdAsync(teamPlayerId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact(DisplayName = "DeleteStatisticAsync when delete fails returns fail result")]
    public async Task DeleteStatisticAsync_WhenDeleteFails_ReturnsFailResult()
    {
        _mockStatisticRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockStatisticRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.DeleteStatisticAsync(1);

        Assert.False(result.Success);
        Assert.Contains("Unable to delete", result.ErrorMessages.First());
    }
}
