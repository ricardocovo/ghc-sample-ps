using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Repositories.PlayerManagement;

public class EfPlayerStatisticRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<EfPlayerStatisticRepository>> _loggerMock;
    private readonly EfPlayerStatisticRepository _repository;
    private const string CurrentUserId = "test-user-id";

    public EfPlayerStatisticRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, CurrentUserId);
        _loggerMock = new Mock<ILogger<EfPlayerStatisticRepository>>();
        _repository = new EfPlayerStatisticRepository(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Constructor Tests

    [Fact(DisplayName = "Constructor throws when context is null")]
    public void Constructor_WhenContextIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EfPlayerStatisticRepository(null!, _loggerMock.Object));
    }

    [Fact(DisplayName = "Constructor throws when logger is null")]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EfPlayerStatisticRepository(_context, null!));
    }

    #endregion

    #region GetAllByPlayerIdAsync Tests

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns empty list when no statistics exist")]
    public async Task GetAllByPlayerIdAsync_WhenNoStatisticsExist_ReturnsEmptyList()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns all statistics for player")]
    public async Task GetAllByPlayerIdAsync_WhenStatisticsExist_ReturnsAllStatistics()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 3);

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal(3, result.Count);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns statistics ordered by GameDate DESC")]
    public async Task GetAllByPlayerIdAsync_WhenStatisticsExist_ReturnsOrderedByGameDateDesc()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 6, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal(new DateTime(2024, 6, 15), result[0].GameDate);
        Assert.Equal(new DateTime(2024, 3, 15), result[1].GameDate);
        Assert.Equal(new DateTime(2024, 1, 15), result[2].GameDate);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync includes TeamPlayer navigation")]
    public async Task GetAllByPlayerIdAsync_WhenStatisticsExist_IncludesTeamPlayerNavigation()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 1);

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.NotNull(result[0].TeamPlayer);
        Assert.Equal(teamPlayer.TeamPlayerId, result[0].TeamPlayer!.TeamPlayerId);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync returns statistics from all team player assignments")]
    public async Task GetAllByPlayerIdAsync_WhenMultipleTeamPlayers_ReturnsStatisticsFromAll()
    {
        var (player, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A");
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 2, 1));
        _context.TeamPlayers.Add(teamPlayer2);
        await _context.SaveChangesAsync();

        await SeedPlayerStatisticsAsync(teamPlayer1.TeamPlayerId, 2);
        await SeedPlayerStatisticsAsync(teamPlayer2.TeamPlayerId, 3);

        var result = await _repository.GetAllByPlayerIdAsync(player.Id);

        Assert.Equal(5, result.Count);
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync respects cancellation token")]
    public async Task GetAllByPlayerIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 5);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetAllByPlayerIdAsync(player.Id, cts.Token));
    }

    [Fact(DisplayName = "GetAllByPlayerIdAsync does not return statistics for other players")]
    public async Task GetAllByPlayerIdAsync_WhenMultiplePlayers_ReturnsOnlyRequestedPlayerStatistics()
    {
        var (player1, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A", "Player 1");
        var (player2, teamPlayer2) = await SeedPlayerWithTeamPlayerAsync("Team B", "Player 2");
        await SeedPlayerStatisticsAsync(teamPlayer1.TeamPlayerId, 2);
        await SeedPlayerStatisticsAsync(teamPlayer2.TeamPlayerId, 3);

        var result = await _repository.GetAllByPlayerIdAsync(player1.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal(teamPlayer1.TeamPlayerId, s.TeamPlayerId));
    }

    #endregion

    #region GetAllByTeamPlayerIdAsync Tests

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync returns empty list when no statistics exist")]
    public async Task GetAllByTeamPlayerIdAsync_WhenNoStatisticsExist_ReturnsEmptyList()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();

        var result = await _repository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync returns all statistics for team player")]
    public async Task GetAllByTeamPlayerIdAsync_WhenStatisticsExist_ReturnsAllStatistics()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 3);

        var result = await _repository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);

        Assert.Equal(3, result.Count);
        Assert.All(result, s => Assert.Equal(teamPlayer.TeamPlayerId, s.TeamPlayerId));
    }

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync returns statistics ordered by GameDate DESC")]
    public async Task GetAllByTeamPlayerIdAsync_WhenStatisticsExist_ReturnsOrderedByGameDateDesc()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 6, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);

        Assert.Equal(new DateTime(2024, 6, 15), result[0].GameDate);
        Assert.Equal(new DateTime(2024, 3, 15), result[1].GameDate);
        Assert.Equal(new DateTime(2024, 1, 15), result[2].GameDate);
    }

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync includes TeamPlayer navigation")]
    public async Task GetAllByTeamPlayerIdAsync_WhenStatisticsExist_IncludesTeamPlayerNavigation()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 1);

        var result = await _repository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId);

        Assert.NotNull(result[0].TeamPlayer);
        Assert.Equal(teamPlayer.TeamPlayerId, result[0].TeamPlayer!.TeamPlayerId);
    }

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync filters by specific team player")]
    public async Task GetAllByTeamPlayerIdAsync_WhenMultipleTeamPlayers_FiltersCorrectly()
    {
        var (player, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A");
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 2, 1));
        _context.TeamPlayers.Add(teamPlayer2);
        await _context.SaveChangesAsync();

        await SeedPlayerStatisticsAsync(teamPlayer1.TeamPlayerId, 2);
        await SeedPlayerStatisticsAsync(teamPlayer2.TeamPlayerId, 3);

        var result = await _repository.GetAllByTeamPlayerIdAsync(teamPlayer1.TeamPlayerId);

        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal(teamPlayer1.TeamPlayerId, s.TeamPlayerId));
    }

    [Fact(DisplayName = "GetAllByTeamPlayerIdAsync respects cancellation token")]
    public async Task GetAllByTeamPlayerIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetAllByTeamPlayerIdAsync(teamPlayer.TeamPlayerId, cts.Token));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact(DisplayName = "GetByIdAsync returns statistic when it exists")]
    public async Task GetByIdAsync_WhenStatisticExists_ReturnsStatistic()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(statistic.PlayerStatisticId);

        Assert.NotNull(result);
        Assert.Equal(statistic.PlayerStatisticId, result.PlayerStatisticId);
    }

    [Fact(DisplayName = "GetByIdAsync returns null when statistic does not exist")]
    public async Task GetByIdAsync_WhenStatisticDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact(DisplayName = "GetByIdAsync includes TeamPlayer navigation")]
    public async Task GetByIdAsync_WhenStatisticExists_IncludesTeamPlayerNavigation()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(statistic.PlayerStatisticId);

        Assert.NotNull(result?.TeamPlayer);
        Assert.Equal(teamPlayer.TeamPlayerId, result.TeamPlayer!.TeamPlayerId);
    }

    [Fact(DisplayName = "GetByIdAsync respects cancellation token")]
    public async Task GetByIdAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByIdAsync(1, cts.Token));
    }

    #endregion

    #region GetByDateRangeAsync Tests

    [Fact(DisplayName = "GetByDateRangeAsync returns statistics within date range")]
    public async Task GetByDateRangeAsync_WhenStatisticsInRange_ReturnsStatistics()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 15)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 6, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByDateRangeAsync(
            player.Id,
            new DateTime(2024, 2, 1),
            new DateTime(2024, 4, 30));

        Assert.Single(result);
        Assert.Equal(new DateTime(2024, 3, 15), result[0].GameDate);
    }

    [Fact(DisplayName = "GetByDateRangeAsync includes boundary dates")]
    public async Task GetByDateRangeAsync_WhenStatisticsOnBoundary_IncludesThese()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 2, 1)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 4, 30)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByDateRangeAsync(
            player.Id,
            new DateTime(2024, 2, 1),
            new DateTime(2024, 4, 30));

        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "GetByDateRangeAsync returns empty when no statistics in range")]
    public async Task GetByDateRangeAsync_WhenNoStatisticsInRange_ReturnsEmpty()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.Add(CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByDateRangeAsync(
            player.Id,
            new DateTime(2024, 6, 1),
            new DateTime(2024, 12, 31));

        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetByDateRangeAsync returns ordered by GameDate DESC")]
    public async Task GetByDateRangeAsync_WhenStatisticsExist_ReturnsOrderedByGameDateDesc()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 2, 1)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 4, 1)),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 1)));
        await _context.SaveChangesAsync();

        var result = await _repository.GetByDateRangeAsync(
            player.Id,
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));

        Assert.Equal(new DateTime(2024, 4, 1), result[0].GameDate);
        Assert.Equal(new DateTime(2024, 3, 1), result[1].GameDate);
        Assert.Equal(new DateTime(2024, 2, 1), result[2].GameDate);
    }

    [Fact(DisplayName = "GetByDateRangeAsync throws when startDate is greater than endDate")]
    public async Task GetByDateRangeAsync_WhenStartDateGreaterThanEndDate_ThrowsArgumentException()
    {
        var (player, _) = await SeedPlayerWithTeamPlayerAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.GetByDateRangeAsync(
                player.Id,
                new DateTime(2024, 12, 31),
                new DateTime(2024, 1, 1)));
    }

    [Fact(DisplayName = "GetByDateRangeAsync respects cancellation token")]
    public async Task GetByDateRangeAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, _) = await SeedPlayerWithTeamPlayerAsync();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.GetByDateRangeAsync(
                player.Id,
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                cts.Token));
    }

    #endregion

    #region AddAsync Tests

    [Fact(DisplayName = "AddAsync adds statistic with generated ID")]
    public async Task AddAsync_WhenValidStatistic_AddsWithGeneratedId()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));

        var result = await _repository.AddAsync(statistic);

        Assert.NotNull(result);
        Assert.True(result.PlayerStatisticId > 0);
    }

    [Fact(DisplayName = "AddAsync persists statistic to database")]
    public async Task AddAsync_WhenValidStatistic_PersistsToDatabase()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var gameDate = new DateTime(2024, 1, 15);
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, gameDate);

        await _repository.AddAsync(statistic);

        var savedStatistic = await _context.PlayerStatistics.FirstOrDefaultAsync(s => s.GameDate == gameDate);
        Assert.NotNull(savedStatistic);
        Assert.Equal(statistic.PlayerStatisticId, savedStatistic.PlayerStatisticId);
    }

    [Fact(DisplayName = "AddAsync sets CreatedAt automatically")]
    public async Task AddAsync_WhenValidStatistic_SetsCreatedAtAutomatically()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var beforeAdd = DateTime.UtcNow;
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));

        var result = await _repository.AddAsync(statistic);

        var afterAdd = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= beforeAdd);
        Assert.True(result.CreatedAt <= afterAdd);
    }

    [Fact(DisplayName = "AddAsync throws when statistic is null")]
    public async Task AddAsync_WhenStatisticIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.AddAsync(null!));
    }

    [Fact(DisplayName = "AddAsync respects cancellation token")]
    public async Task AddAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.AddAsync(statistic, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "AddAsync increments ID for each new statistic")]
    public async Task AddAsync_WhenMultipleStatistics_IncrementsId()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic1 = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        var statistic2 = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 2, 15));

        var result1 = await _repository.AddAsync(statistic1);
        var result2 = await _repository.AddAsync(statistic2);

        Assert.True(result2.PlayerStatisticId > result1.PlayerStatisticId);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact(DisplayName = "UpdateAsync updates existing statistic")]
    public async Task UpdateAsync_WhenStatisticExists_UpdatesStatistic()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15), goals: 2);
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var toUpdate = await _context.PlayerStatistics.FindAsync(statistic.PlayerStatisticId);
        var updatedStatistic = new PlayerStatistic
        {
            PlayerStatisticId = toUpdate!.PlayerStatisticId,
            TeamPlayerId = toUpdate.TeamPlayerId,
            GameDate = toUpdate.GameDate,
            MinutesPlayed = toUpdate.MinutesPlayed,
            IsStarter = toUpdate.IsStarter,
            JerseyNumber = toUpdate.JerseyNumber,
            Goals = 5,  // Changed value
            Assists = toUpdate.Assists,
            CreatedBy = toUpdate.CreatedBy,
            CreatedAt = toUpdate.CreatedAt
        };
        updatedStatistic.UpdateLastModified("test-user");

        var result = await _repository.UpdateAsync(updatedStatistic);

        Assert.NotNull(result);
        Assert.Equal(5, result.Goals);
    }

    [Fact(DisplayName = "UpdateAsync sets UpdatedAt automatically")]
    public async Task UpdateAsync_WhenStatisticExists_SetsUpdatedAtAutomatically()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var toUpdate = await _context.PlayerStatistics.FindAsync(statistic.PlayerStatisticId);
        toUpdate!.UpdateLastModified("test-user");

        var beforeUpdate = DateTime.UtcNow;
        var result = await _repository.UpdateAsync(toUpdate);
        var afterUpdate = DateTime.UtcNow;

        Assert.NotNull(result.UpdatedAt);
        Assert.True(result.UpdatedAt >= beforeUpdate);
        Assert.True(result.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateAsync throws RepositoryException when statistic does not exist")]
    public async Task UpdateAsync_WhenStatisticDoesNotExist_ThrowsRepositoryException()
    {
        var statistic = new PlayerStatistic
        {
            PlayerStatisticId = 999,
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 1, 1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test"
        };

        await Assert.ThrowsAsync<RepositoryException>(() =>
            _repository.UpdateAsync(statistic));
    }

    [Fact(DisplayName = "UpdateAsync throws when statistic is null")]
    public async Task UpdateAsync_WhenStatisticIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.UpdateAsync(null!));
    }

    [Fact(DisplayName = "UpdateAsync respects cancellation token")]
    public async Task UpdateAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var toUpdate = await _context.PlayerStatistics.FindAsync(statistic.PlayerStatisticId);
        toUpdate!.UpdateLastModified("test-user");

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.UpdateAsync(toUpdate, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "UpdateAsync preserves original CreatedAt and CreatedBy")]
    public async Task UpdateAsync_WhenStatisticExists_PreservesCreatedAtAndCreatedBy()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var originalCreatedAt = statistic.CreatedAt;
        var originalCreatedBy = statistic.CreatedBy;

        var toUpdate = await _context.PlayerStatistics.FindAsync(statistic.PlayerStatisticId);
        toUpdate!.UpdateLastModified("different-user");

        var result = await _repository.UpdateAsync(toUpdate);

        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.Equal(originalCreatedBy, result.CreatedBy);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact(DisplayName = "DeleteAsync returns true when statistic exists")]
    public async Task DeleteAsync_WhenStatisticExists_ReturnsTrue()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(statistic.PlayerStatisticId);

        Assert.True(result);
    }

    [Fact(DisplayName = "DeleteAsync removes statistic from database")]
    public async Task DeleteAsync_WhenStatisticExists_RemovesFromDatabase()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();
        var statisticId = statistic.PlayerStatisticId;

        await _repository.DeleteAsync(statisticId);

        var deletedStatistic = await _context.PlayerStatistics.FindAsync(statisticId);
        Assert.Null(deletedStatistic);
    }

    [Fact(DisplayName = "DeleteAsync returns false when statistic does not exist")]
    public async Task DeleteAsync_WhenStatisticDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "DeleteAsync respects cancellation token")]
    public async Task DeleteAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.DeleteAsync(statistic.PlayerStatisticId, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "DeleteAsync reduces statistic count")]
    public async Task DeleteAsync_WhenStatisticExists_ReducesCount()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 3);
        var statistic = await _context.PlayerStatistics.FirstAsync();

        await _repository.DeleteAsync(statistic.PlayerStatisticId);

        var remainingCount = await _context.PlayerStatistics.CountAsync();
        Assert.Equal(2, remainingCount);
    }

    #endregion

    #region ExistsAsync Tests

    [Fact(DisplayName = "ExistsAsync returns true when statistic exists")]
    public async Task ExistsAsync_WhenStatisticExists_ReturnsTrue()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));
        _context.PlayerStatistics.Add(statistic);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(statistic.PlayerStatisticId);

        Assert.True(result);
    }

    [Fact(DisplayName = "ExistsAsync returns false when statistic does not exist")]
    public async Task ExistsAsync_WhenStatisticDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "ExistsAsync respects cancellation token")]
    public async Task ExistsAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.ExistsAsync(1, cts.Token));
    }

    #endregion

    #region GetAggregatesAsync Tests

    [Fact(DisplayName = "GetAggregatesAsync calculates totals correctly")]
    public async Task GetAggregatesAsync_WhenStatisticsExist_CalculatesTotalsCorrectly()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15), goals: 2, assists: 1, minutes: 90),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 2, 15), goals: 1, assists: 2, minutes: 45),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 15), goals: 0, assists: 3, minutes: 60));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAggregatesAsync(player.Id);

        Assert.Equal(3, result.GameCount);
        Assert.Equal(3, result.TotalGoals);
        Assert.Equal(6, result.TotalAssists);
        Assert.Equal(195, result.TotalMinutesPlayed);
    }

    [Fact(DisplayName = "GetAggregatesAsync calculates averages correctly")]
    public async Task GetAggregatesAsync_WhenStatisticsExist_CalculatesAveragesCorrectly()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15), goals: 2, assists: 1, minutes: 90),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 2, 15), goals: 1, assists: 2, minutes: 45),
            CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 3, 15), goals: 0, assists: 3, minutes: 60));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAggregatesAsync(player.Id);

        Assert.Equal(1.0, result.AverageGoals);
        Assert.Equal(2.0, result.AverageAssists);
        Assert.Equal(65.0, result.AverageMinutesPlayed);
    }

    [Fact(DisplayName = "GetAggregatesAsync returns empty result when no statistics")]
    public async Task GetAggregatesAsync_WhenNoStatistics_ReturnsEmptyResult()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();

        var result = await _repository.GetAggregatesAsync(player.Id);

        Assert.Equal(0, result.GameCount);
        Assert.Equal(0, result.TotalGoals);
        Assert.Equal(0, result.TotalAssists);
        Assert.Equal(0, result.TotalMinutesPlayed);
        Assert.Equal(0, result.AverageGoals);
        Assert.Equal(0, result.AverageAssists);
        Assert.Equal(0, result.AverageMinutesPlayed);
    }

    [Fact(DisplayName = "GetAggregatesAsync filters by teamPlayerId when provided")]
    public async Task GetAggregatesAsync_WhenTeamPlayerIdProvided_FiltersCorrectly()
    {
        var (player, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A");
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 2, 1));
        _context.TeamPlayers.Add(teamPlayer2);
        await _context.SaveChangesAsync();

        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer1.TeamPlayerId, new DateTime(2024, 1, 15), goals: 2, assists: 1, minutes: 90),
            CreatePlayerStatistic(teamPlayer2.TeamPlayerId, new DateTime(2024, 2, 15), goals: 10, assists: 5, minutes: 90));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAggregatesAsync(player.Id, teamPlayer1.TeamPlayerId);

        Assert.Equal(1, result.GameCount);
        Assert.Equal(2, result.TotalGoals);
        Assert.Equal(1, result.TotalAssists);
    }

    [Fact(DisplayName = "GetAggregatesAsync includes all team players when teamPlayerId not provided")]
    public async Task GetAggregatesAsync_WhenTeamPlayerIdNotProvided_IncludesAllTeamPlayers()
    {
        var (player, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A");
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 2, 1));
        _context.TeamPlayers.Add(teamPlayer2);
        await _context.SaveChangesAsync();

        _context.PlayerStatistics.AddRange(
            CreatePlayerStatistic(teamPlayer1.TeamPlayerId, new DateTime(2024, 1, 15), goals: 2, assists: 1, minutes: 90),
            CreatePlayerStatistic(teamPlayer2.TeamPlayerId, new DateTime(2024, 2, 15), goals: 10, assists: 5, minutes: 90));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAggregatesAsync(player.Id);

        Assert.Equal(2, result.GameCount);
        Assert.Equal(12, result.TotalGoals);
        Assert.Equal(6, result.TotalAssists);
    }

    [Fact(DisplayName = "GetAggregatesAsync respects cancellation token")]
    public async Task GetAggregatesAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var (player, _) = await SeedPlayerWithTeamPlayerAsync();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetAggregatesAsync(player.Id, null, cts.Token));
    }

    #endregion

    #region Cascade Delete Tests

    [Fact(DisplayName = "Statistics are deleted when TeamPlayer is deleted")]
    public async Task CascadeDelete_WhenTeamPlayerDeleted_StatisticsAreDeleted()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 3);
        var initialCount = await _context.PlayerStatistics.CountAsync();
        Assert.Equal(3, initialCount);

        _context.TeamPlayers.Remove(teamPlayer);
        await _context.SaveChangesAsync();

        var remainingCount = await _context.PlayerStatistics.CountAsync();
        Assert.Equal(0, remainingCount);
    }

    [Fact(DisplayName = "Statistics for other TeamPlayers are not affected when one TeamPlayer is deleted")]
    public async Task CascadeDelete_WhenTeamPlayerDeleted_OtherStatisticsNotAffected()
    {
        var (player, teamPlayer1) = await SeedPlayerWithTeamPlayerAsync("Team A");
        var teamPlayer2 = CreateTeamPlayer(player.Id, "Team B", new DateTime(2024, 2, 1));
        _context.TeamPlayers.Add(teamPlayer2);
        await _context.SaveChangesAsync();

        await SeedPlayerStatisticsAsync(teamPlayer1.TeamPlayerId, 2);
        await SeedPlayerStatisticsAsync(teamPlayer2.TeamPlayerId, 3);

        _context.TeamPlayers.Remove(teamPlayer1);
        await _context.SaveChangesAsync();

        var remainingCount = await _context.PlayerStatistics.CountAsync();
        Assert.Equal(3, remainingCount);
        Assert.All(
            await _context.PlayerStatistics.ToListAsync(),
            s => Assert.Equal(teamPlayer2.TeamPlayerId, s.TeamPlayerId));
    }

    #endregion

    #region Logging Tests

    [Fact(DisplayName = "GetAllByPlayerIdAsync logs retrieval operation")]
    public async Task GetAllByPlayerIdAsync_WhenCalled_LogsOperation()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        await SeedPlayerStatisticsAsync(teamPlayer.TeamPlayerId, 3);

        await _repository.GetAllByPlayerIdAsync(player.Id);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved 3 player statistics")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "AddAsync logs successful addition")]
    public async Task AddAsync_WhenSuccessful_LogsOperation()
    {
        var (player, teamPlayer) = await SeedPlayerWithTeamPlayerAsync();
        var statistic = CreatePlayerStatistic(teamPlayer.TeamPlayerId, new DateTime(2024, 1, 15));

        await _repository.AddAsync(statistic);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully added player statistic")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static PlayerStatistic CreatePlayerStatistic(
        int teamPlayerId,
        DateTime gameDate,
        int goals = 0,
        int assists = 0,
        int minutes = 90)
    {
        return new PlayerStatistic
        {
            TeamPlayerId = teamPlayerId,
            GameDate = gameDate,
            MinutesPlayed = minutes,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = goals,
            Assists = assists,
            CreatedBy = "test-creator"
        };
    }

    private static TeamPlayer CreateTeamPlayer(int playerId, string teamName, DateTime joinedDate)
    {
        return new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = "Test Championship",
            JoinedDate = joinedDate,
            CreatedBy = "test-creator"
        };
    }

    private async Task<(Player player, TeamPlayer teamPlayer)> SeedPlayerWithTeamPlayerAsync(
        string teamName = "Test Team",
        string playerName = "Test Player")
    {
        var player = new Player
        {
            UserId = Guid.NewGuid().ToString(),
            Name = playerName,
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            CreatedBy = "test-creator"
        };
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var teamPlayer = CreateTeamPlayer(player.Id, teamName, new DateTime(2024, 1, 1));
        _context.TeamPlayers.Add(teamPlayer);
        await _context.SaveChangesAsync();

        return (player, teamPlayer);
    }

    private async Task SeedPlayerStatisticsAsync(int teamPlayerId, int count)
    {
        for (var i = 1; i <= count; i++)
        {
            _context.PlayerStatistics.Add(
                CreatePlayerStatistic(teamPlayerId, DateTime.UtcNow.AddDays(-i)));
        }

        await _context.SaveChangesAsync();
    }

    #endregion
}
