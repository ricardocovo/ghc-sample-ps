using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Integration;

/// <summary>
/// Integration tests for PlayerStatistic management workflows.
/// These tests verify end-to-end behavior with an in-memory database.
/// </summary>
public class PlayerStatisticIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EfPlayerStatisticRepository _statisticRepository;
    private readonly EfTeamPlayerRepository _teamPlayerRepository;
    private readonly EfPlayerRepository _playerRepository;
    private readonly PlayerStatisticService _statisticService;
    private readonly Mock<ILogger<EfPlayerStatisticRepository>> _statisticRepoLoggerMock;
    private readonly Mock<ILogger<EfTeamPlayerRepository>> _teamPlayerRepoLoggerMock;
    private readonly Mock<ILogger<EfPlayerRepository>> _playerRepoLoggerMock;
    private readonly Mock<ILogger<PlayerStatisticService>> _serviceLoggerMock;
    private const string CurrentUserId = "integration-test-user";
    private bool _disposed;

    public PlayerStatisticIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, CurrentUserId);
        _statisticRepoLoggerMock = new Mock<ILogger<EfPlayerStatisticRepository>>();
        _teamPlayerRepoLoggerMock = new Mock<ILogger<EfTeamPlayerRepository>>();
        _playerRepoLoggerMock = new Mock<ILogger<EfPlayerRepository>>();
        _serviceLoggerMock = new Mock<ILogger<PlayerStatisticService>>();

        _statisticRepository = new EfPlayerStatisticRepository(_context, _statisticRepoLoggerMock.Object);
        _teamPlayerRepository = new EfTeamPlayerRepository(_context, _teamPlayerRepoLoggerMock.Object);
        _playerRepository = new EfPlayerRepository(_context, _playerRepoLoggerMock.Object);
        _statisticService = new PlayerStatisticService(
            _statisticRepository,
            _teamPlayerRepository,
            _serviceLoggerMock.Object);
    }

    #region Complete Workflow Tests

    [Fact(DisplayName = "AddAndRetrieveStatistic_CompleteWorkflow_Succeeds")]
    public async Task AddAndRetrieveStatistic_CompleteWorkflow_Succeeds()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Team Alpha", "Championship 2024");

        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = DateTime.UtcNow.AddDays(-7),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var createResult = await _statisticService.AddStatisticAsync(createDto, CurrentUserId);
        Assert.True(createResult.Success);
        Assert.NotNull(createResult.Data);
        Assert.True(createResult.Data.PlayerStatisticId > 0);

        var retrieveResult = await _statisticService.GetStatisticByIdAsync(createResult.Data.PlayerStatisticId);
        Assert.True(retrieveResult.Success);
        Assert.NotNull(retrieveResult.Data);
        Assert.Equal(90, retrieveResult.Data.MinutesPlayed);
        Assert.True(retrieveResult.Data.IsStarter);
        Assert.Equal(10, retrieveResult.Data.JerseyNumber);
        Assert.Equal(2, retrieveResult.Data.Goals);
        Assert.Equal(1, retrieveResult.Data.Assists);

        Assert.NotEqual(default, retrieveResult.Data.CreatedAt);
        Assert.Equal(CurrentUserId, retrieveResult.Data.CreatedBy);
    }

    [Fact(DisplayName = "UpdateStatistic_ModifiesDataCorrectly")]
    public async Task UpdateStatistic_ModifiesDataCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");
        var statistic = await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-5), 60, 1, 0);

        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = statistic.PlayerStatisticId,
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = statistic.GameDate,
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 3,
            Assists = 2
        };

        var result = await _statisticService.UpdateStatisticAsync(
            statistic.PlayerStatisticId,
            updateDto,
            CurrentUserId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(90, result.Data.MinutesPlayed);
        Assert.True(result.Data.IsStarter);
        Assert.Equal(3, result.Data.Goals);
        Assert.Equal(2, result.Data.Assists);

        var verifyResult = await _statisticService.GetStatisticByIdAsync(statistic.PlayerStatisticId);
        Assert.True(verifyResult.Success);
        Assert.NotNull(verifyResult.Data);
        Assert.Equal(90, verifyResult.Data.MinutesPlayed);
        Assert.Equal(3, verifyResult.Data.Goals);
        Assert.Equal(2, verifyResult.Data.Assists);
    }

    [Fact(DisplayName = "DeleteTeamPlayer_CascadesDeleteToStatistics")]
    public async Task DeleteTeamPlayer_CascadesDeleteToStatistics()
    {
        var player = await SeedPlayerAsync("Player to Delete");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Team to Delete", "Championship");
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-30), 90, 2, 1);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-23), 85, 1, 2);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-16), 75, 0, 1);

        var statsBeforeDelete = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);
        Assert.Equal(3, statsBeforeDelete.Count);

        _context.TeamPlayers.Remove(teamPlayer);
        await _context.SaveChangesAsync();

        var statsAfterDelete = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);
        Assert.Empty(statsAfterDelete);
    }

    [Fact(DisplayName = "GetAggregates_WithMultipleStats_CalculatesCorrectly")]
    public async Task GetAggregates_WithMultipleStats_CalculatesCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-30), 90, goals: 2, assists: 1);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-23), 90, goals: 1, assists: 2);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-16), 90, goals: 0, assists: 3);

        var result = await _statisticService.GetPlayerAggregatesAsync(player.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.GameCount);
        Assert.Equal(3, result.Data.TotalGoals); // 2 + 1 + 0
        Assert.Equal(6, result.Data.TotalAssists); // 1 + 2 + 3
        Assert.Equal(270, result.Data.TotalMinutesPlayed); // 90 * 3
        Assert.Equal(1.0, result.Data.AverageGoals, precision: 2);
        Assert.Equal(2.0, result.Data.AverageAssists, precision: 2);
        Assert.Equal(90.0, result.Data.AverageMinutesPlayed, precision: 2);
    }

    [Fact(DisplayName = "GetStatisticsByDateRange_FiltersCorrectly")]
    public async Task GetStatisticsByDateRange_FiltersCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var dateInRange1 = DateTime.UtcNow.AddDays(-20);
        var dateInRange2 = DateTime.UtcNow.AddDays(-15);
        var dateOutOfRange1 = DateTime.UtcNow.AddDays(-60);
        var dateOutOfRange2 = DateTime.UtcNow.AddDays(-5);

        await SeedStatisticAsync(teamPlayer.TeamPlayerId, dateOutOfRange1, 90, 1, 1);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, dateInRange1, 90, 2, 2);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, dateInRange2, 90, 3, 3);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, dateOutOfRange2, 90, 4, 4);

        var startDate = DateTime.UtcNow.AddDays(-25);
        var endDate = DateTime.UtcNow.AddDays(-10);
        var statsInRange = await _statisticRepository.GetByDateRangeAsync(player.Id, startDate, endDate);

        Assert.Equal(2, statsInRange.Count);
        Assert.All(statsInRange, s => Assert.True(s.GameDate >= startDate && s.GameDate <= endDate));
    }

    [Fact(DisplayName = "MultipleStatisticsSameDay_AllowedAndRetrieved")]
    public async Task MultipleStatisticsSameDay_AllowedAndRetrieved()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var sameDate = DateTime.UtcNow.Date.AddDays(-10);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, sameDate, 45, 1, 0);
        await SeedStatisticAsync(teamPlayer.TeamPlayerId, sameDate, 45, 0, 1);

        var allStats = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);

        Assert.Equal(2, allStats.Count);
        Assert.All(allStats, s => Assert.Equal(sameDate.Date, s.GameDate.Date));
    }

    [Fact(DisplayName = "StatisticsAcrossMultipleTeams_FilteredCorrectly")]
    public async Task StatisticsAcrossMultipleTeams_FilteredCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer1 = await SeedTeamPlayerAsync(player.Id, "Team A", "Championship 1");
        var teamPlayer2 = await SeedTeamPlayerAsync(player.Id, "Team B", "Championship 2");

        await SeedStatisticAsync(teamPlayer1.TeamPlayerId, DateTime.UtcNow.AddDays(-30), 90, 2, 1);
        await SeedStatisticAsync(teamPlayer1.TeamPlayerId, DateTime.UtcNow.AddDays(-23), 85, 1, 0);
        await SeedStatisticAsync(teamPlayer2.TeamPlayerId, DateTime.UtcNow.AddDays(-20), 90, 3, 2);
        await SeedStatisticAsync(teamPlayer2.TeamPlayerId, DateTime.UtcNow.AddDays(-13), 60, 0, 1);

        var team1Stats = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayer1.TeamPlayerId);
        Assert.Equal(2, team1Stats.Count);
        Assert.All(team1Stats, s => Assert.Equal(teamPlayer1.TeamPlayerId, s.TeamPlayerId));

        var team2Stats = await _statisticRepository.GetAllByTeamPlayerIdAsync(teamPlayer2.TeamPlayerId);
        Assert.Equal(2, team2Stats.Count);
        Assert.All(team2Stats, s => Assert.Equal(teamPlayer2.TeamPlayerId, s.TeamPlayerId));

        var allPlayerStats = await _statisticService.GetStatisticsByPlayerIdAsync(player.Id);
        Assert.True(allPlayerStats.Success);
        Assert.NotNull(allPlayerStats.Data);
        Assert.Equal(4, allPlayerStats.Data.Count);

        var allTeamsAggregates = await _statisticService.GetPlayerAggregatesAsync(player.Id);
        Assert.True(allTeamsAggregates.Success);
        Assert.NotNull(allTeamsAggregates.Data);
        Assert.Equal(4, allTeamsAggregates.Data.GameCount);
        Assert.Equal(6, allTeamsAggregates.Data.TotalGoals); // 2 + 1 + 3 + 0

        var team1Aggregates = await _statisticService.GetPlayerAggregatesAsync(player.Id, teamPlayer1.TeamPlayerId);
        Assert.True(team1Aggregates.Success);
        Assert.NotNull(team1Aggregates.Data);
        Assert.Equal(2, team1Aggregates.Data.GameCount);
        Assert.Equal(3, team1Aggregates.Data.TotalGoals); // 2 + 1
    }

    #endregion

    #region Validation Error Scenarios

    [Fact(DisplayName = "AddStatistic_WithNegativeGoals_ReturnsValidationError")]
    public async Task AddStatistic_WithNegativeGoals_ReturnsValidationError()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = DateTime.UtcNow.AddDays(-7),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = -1,
            Assists = 0
        };

        var result = await _statisticService.AddStatisticAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("Goals"));
    }

    [Fact(DisplayName = "AddStatistic_WithZeroJerseyNumber_ReturnsValidationError")]
    public async Task AddStatistic_WithZeroJerseyNumber_ReturnsValidationError()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = DateTime.UtcNow.AddDays(-7),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 0,
            Goals = 0,
            Assists = 0
        };

        var result = await _statisticService.AddStatisticAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("JerseyNumber"));
    }

    [Fact(DisplayName = "AddStatistic_WithFutureDate_ReturnsValidationError")]
    public async Task AddStatistic_WithFutureDate_ReturnsValidationError()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = DateTime.UtcNow.AddYears(1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _statisticService.AddStatisticAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.True(result.ValidationErrors.ContainsKey("GameDate"));
    }

    [Fact(DisplayName = "AddStatistic_WithInvalidTeamPlayerId_ReturnsError")]
    public async Task AddStatistic_WithInvalidTeamPlayerId_ReturnsError()
    {
        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 9999, // Non-existent team player
            GameDate = DateTime.UtcNow.AddDays(-7),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = await _statisticService.AddStatisticAsync(createDto, CurrentUserId);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    #endregion

    #region Audit Fields Tests

    [Fact(DisplayName = "Statistic_AuditFieldsPopulatedCorrectly")]
    public async Task Statistic_AuditFieldsPopulatedCorrectly()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Audit Test Team", "Audit Championship");
        var beforeCreate = DateTime.UtcNow;

        var createDto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = DateTime.UtcNow.AddDays(-7),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var createResult = await _statisticService.AddStatisticAsync(createDto, "create-user");
        Assert.True(createResult.Success);
        Assert.NotNull(createResult.Data);

        var afterCreate = DateTime.UtcNow;

        Assert.True(createResult.Data.CreatedAt >= beforeCreate);
        Assert.True(createResult.Data.CreatedAt <= afterCreate);
        Assert.Equal("create-user", createResult.Data.CreatedBy);

        var beforeUpdate = DateTime.UtcNow;

        var updateDto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = createResult.Data.PlayerStatisticId,
            TeamPlayerId = teamPlayer.TeamPlayerId,
            GameDate = createResult.Data.GameDate,
            MinutesPlayed = 85,
            IsStarter = false,
            JerseyNumber = 10,
            Goals = 1,
            Assists = 1
        };

        var updateResult = await _statisticService.UpdateStatisticAsync(
            createResult.Data.PlayerStatisticId,
            updateDto,
            "update-user");
        Assert.True(updateResult.Success);
        Assert.NotNull(updateResult.Data);

        var afterUpdate = DateTime.UtcNow;

        // CreatedBy is preserved from creation
        Assert.Equal("create-user", updateResult.Data.CreatedBy);

        // UpdatedAt and UpdatedBy are set
        Assert.NotNull(updateResult.Data.UpdatedAt);
        Assert.True(updateResult.Data.UpdatedAt >= beforeUpdate);
        Assert.True(updateResult.Data.UpdatedAt <= afterUpdate);
    }

    #endregion

    #region Delete Statistics Tests

    [Fact(DisplayName = "DeleteStatistic_RemovesFromDatabase")]
    public async Task DeleteStatistic_RemovesFromDatabase()
    {
        var player = await SeedPlayerAsync("Test Player");
        var teamPlayer = await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");
        var statistic = await SeedStatisticAsync(teamPlayer.TeamPlayerId, DateTime.UtcNow.AddDays(-7), 90, 1, 1);

        var existsBefore = await _statisticRepository.ExistsAsync(statistic.PlayerStatisticId);
        Assert.True(existsBefore);

        var result = await _statisticService.DeleteStatisticAsync(statistic.PlayerStatisticId);
        Assert.True(result.Success);

        var existsAfter = await _statisticRepository.ExistsAsync(statistic.PlayerStatisticId);
        Assert.False(existsAfter);
    }

    [Fact(DisplayName = "DeleteStatistic_WithInvalidId_ReturnsError")]
    public async Task DeleteStatistic_WithInvalidId_ReturnsError()
    {
        var result = await _statisticService.DeleteStatisticAsync(9999);

        Assert.False(result.Success);
        Assert.Contains("could not be found", result.ErrorMessages.First());
    }

    #endregion

    #region Aggregate Edge Cases

    [Fact(DisplayName = "GetAggregates_WithZeroGames_ReturnsZeroAverages")]
    public async Task GetAggregates_WithZeroGames_ReturnsZeroAverages()
    {
        var player = await SeedPlayerAsync("Test Player");
        await SeedTeamPlayerAsync(player.Id, "Test Team", "Test Championship");

        // No statistics added

        var result = await _statisticService.GetPlayerAggregatesAsync(player.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data.GameCount);
        Assert.Equal(0, result.Data.TotalGoals);
        Assert.Equal(0, result.Data.TotalAssists);
        Assert.Equal(0, result.Data.AverageGoals);
        Assert.Equal(0, result.Data.AverageAssists);
    }

    #endregion

    #region Helper Methods

    private async Task<Player> SeedPlayerAsync(string name = "Test Player")
    {
        var player = new Player
        {
            UserId = Guid.NewGuid().ToString(),
            Name = name,
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            CreatedBy = CurrentUserId
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return player;
    }

    private async Task<TeamPlayer> SeedTeamPlayerAsync(int playerId, string teamName, string championshipName)
    {
        var teamPlayer = new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = championshipName,
            JoinedDate = DateTime.UtcNow.AddMonths(-6),
            CreatedBy = CurrentUserId
        };
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();
        return teamPlayer;
    }

    private async Task<PlayerStatistic> SeedStatisticAsync(
        int teamPlayerId,
        DateTime gameDate,
        int minutesPlayed,
        int goals,
        int assists)
    {
        var statistic = new PlayerStatistic
        {
            TeamPlayerId = teamPlayerId,
            GameDate = gameDate,
            MinutesPlayed = minutesPlayed,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = goals,
            Assists = assists,
            CreatedBy = CurrentUserId
        };
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();
        return statistic;
    }

    #endregion

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
