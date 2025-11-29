using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Exceptions;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Implementations;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Repositories.PlayerManagement;

public class EfPlayerRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<EfPlayerRepository>> _loggerMock;
    private readonly EfPlayerRepository _repository;
    private const string CurrentUserId = "test-user-id";

    public EfPlayerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, CurrentUserId);
        _loggerMock = new Mock<ILogger<EfPlayerRepository>>();
        _repository = new EfPlayerRepository(_context, _loggerMock.Object);
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
            new EfPlayerRepository(null!, _loggerMock.Object));
    }

    [Fact(DisplayName = "Constructor throws when logger is null")]
    public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new EfPlayerRepository(_context, null!));
    }

    #endregion

    #region GetAllAsync Tests

    [Fact(DisplayName = "GetAllAsync returns empty list when no players exist")]
    public async Task GetAllAsync_WhenNoPlayersExist_ReturnsEmptyList()
    {
        var result = await _repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "GetAllAsync returns all players")]
    public async Task GetAllAsync_WhenPlayersExist_ReturnsAllPlayers()
    {
        await SeedPlayersAsync(3);

        var result = await _repository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact(DisplayName = "GetAllAsync returns players ordered by name")]
    public async Task GetAllAsync_WhenPlayersExist_ReturnsPlayersOrderedByName()
    {
        _context.Players.AddRange(
            CreatePlayer("Zack"),
            CreatePlayer("Alice"),
            CreatePlayer("Mike"));
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Mike", result[1].Name);
        Assert.Equal("Zack", result[2].Name);
    }

    [Fact(DisplayName = "GetAllAsync respects cancellation token")]
    public async Task GetAllAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        await SeedPlayersAsync(5);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetAllAsync(cts.Token));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact(DisplayName = "GetByIdAsync returns player when player exists")]
    public async Task GetByIdAsync_WhenPlayerExists_ReturnsPlayer()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(player.Id);

        Assert.NotNull(result);
        Assert.Equal(player.Id, result.Id);
        Assert.Equal("Test Player", result.Name);
    }

    [Fact(DisplayName = "GetByIdAsync returns null when player does not exist")]
    public async Task GetByIdAsync_WhenPlayerDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(999);

        Assert.Null(result);
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

    #region AddAsync Tests

    [Fact(DisplayName = "AddAsync adds player with generated ID")]
    public async Task AddAsync_WhenValidPlayer_AddsPlayerWithGeneratedId()
    {
        var player = CreatePlayer("New Player");

        var result = await _repository.AddAsync(player);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Player", result.Name);
    }

    [Fact(DisplayName = "AddAsync persists player to database")]
    public async Task AddAsync_WhenValidPlayer_PersistsToDatabase()
    {
        var player = CreatePlayer("Persisted Player");

        await _repository.AddAsync(player);

        var savedPlayer = await _context.Players.FirstOrDefaultAsync(p => p.Name == "Persisted Player");
        Assert.NotNull(savedPlayer);
        Assert.Equal(player.Id, savedPlayer.Id);
    }

    [Fact(DisplayName = "AddAsync sets CreatedAt automatically")]
    public async Task AddAsync_WhenValidPlayer_SetsCreatedAtAutomatically()
    {
        var beforeAdd = DateTime.UtcNow;
        var player = CreatePlayer("Test Player");

        var result = await _repository.AddAsync(player);

        var afterAdd = DateTime.UtcNow;
        Assert.True(result.CreatedAt >= beforeAdd);
        Assert.True(result.CreatedAt <= afterAdd);
    }

    [Fact(DisplayName = "AddAsync throws when player is null")]
    public async Task AddAsync_WhenPlayerIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.AddAsync(null!));
    }

    [Fact(DisplayName = "AddAsync respects cancellation token")]
    public async Task AddAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = CreatePlayer("Test Player");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.AddAsync(player, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "AddAsync increments ID for each new player")]
    public async Task AddAsync_WhenMultiplePlayers_IncrementsId()
    {
        var player1 = CreatePlayer("Player 1");
        var player2 = CreatePlayer("Player 2");

        var result1 = await _repository.AddAsync(player1);
        var result2 = await _repository.AddAsync(player2);

        Assert.True(result2.Id > result1.Id);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact(DisplayName = "UpdateAsync updates existing player")]
    public async Task UpdateAsync_WhenPlayerExists_UpdatesPlayer()
    {
        var player = CreatePlayer("Original Name");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var updatedPlayer = new Player
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = "Updated Name",
            DateOfBirth = player.DateOfBirth,
            Gender = "Female",
            CreatedBy = player.CreatedBy
        };

        var result = await _repository.UpdateAsync(updatedPlayer);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Female", result.Gender);
    }

    [Fact(DisplayName = "UpdateAsync sets UpdatedAt automatically")]
    public async Task UpdateAsync_WhenPlayerExists_SetsUpdatedAtAutomatically()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var updatedPlayer = new Player
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = "Updated Name",
            DateOfBirth = player.DateOfBirth,
            CreatedBy = player.CreatedBy
        };

        var beforeUpdate = DateTime.UtcNow;
        var result = await _repository.UpdateAsync(updatedPlayer);
        var afterUpdate = DateTime.UtcNow;

        Assert.NotNull(result.UpdatedAt);
        Assert.True(result.UpdatedAt >= beforeUpdate);
        Assert.True(result.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateAsync throws PlayerNotFoundException when player does not exist")]
    public async Task UpdateAsync_WhenPlayerDoesNotExist_ThrowsPlayerNotFoundException()
    {
        var player = new Player
        {
            Id = 999,
            UserId = "test-user",
            Name = "Non-existent",
            DateOfBirth = new DateTime(2010, 1, 1),
            CreatedBy = "test"
        };

        await Assert.ThrowsAsync<PlayerNotFoundException>(() =>
            _repository.UpdateAsync(player));
    }

    [Fact(DisplayName = "UpdateAsync throws when player is null")]
    public async Task UpdateAsync_WhenPlayerIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.UpdateAsync(null!));
    }

    [Fact(DisplayName = "UpdateAsync respects cancellation token")]
    public async Task UpdateAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var updatedPlayer = new Player
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = "Updated Name",
            DateOfBirth = player.DateOfBirth,
            CreatedBy = player.CreatedBy
        };

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.UpdateAsync(updatedPlayer, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "UpdateAsync preserves original CreatedAt and CreatedBy")]
    public async Task UpdateAsync_WhenPlayerExists_PreservesCreatedAtAndCreatedBy()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var originalCreatedAt = player.CreatedAt;
        var originalCreatedBy = player.CreatedBy;

        var updatedPlayer = new Player
        {
            Id = player.Id,
            UserId = player.UserId,
            Name = "Updated Name",
            DateOfBirth = player.DateOfBirth,
            CreatedBy = "different-user" // This should be ignored
        };

        var result = await _repository.UpdateAsync(updatedPlayer);

        Assert.Equal(originalCreatedAt, result.CreatedAt);
        Assert.Equal(originalCreatedBy, result.CreatedBy);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact(DisplayName = "DeleteAsync returns true when player exists")]
    public async Task DeleteAsync_WhenPlayerExists_ReturnsTrue()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var result = await _repository.DeleteAsync(player.Id);

        Assert.True(result);
    }

    [Fact(DisplayName = "DeleteAsync removes player from database")]
    public async Task DeleteAsync_WhenPlayerExists_RemovesFromDatabase()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        var playerId = player.Id;

        await _repository.DeleteAsync(playerId);

        var deletedPlayer = await _context.Players.FindAsync(playerId);
        Assert.Null(deletedPlayer);
    }

    [Fact(DisplayName = "DeleteAsync returns false when player does not exist")]
    public async Task DeleteAsync_WhenPlayerDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "DeleteAsync respects cancellation token")]
    public async Task DeleteAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.DeleteAsync(player.Id, cts.Token));

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "DeleteAsync reduces player count")]
    public async Task DeleteAsync_WhenPlayerExists_ReducesPlayerCount()
    {
        await SeedPlayersAsync(3);
        var player = await _context.Players.FirstAsync();

        await _repository.DeleteAsync(player.Id);

        var remainingCount = await _context.Players.CountAsync();
        Assert.Equal(2, remainingCount);
    }

    #endregion

    #region ExistsAsync Tests

    [Fact(DisplayName = "ExistsAsync returns true when player exists")]
    public async Task ExistsAsync_WhenPlayerExists_ReturnsTrue()
    {
        var player = CreatePlayer("Test Player");
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(player.Id);

        Assert.True(result);
    }

    [Fact(DisplayName = "ExistsAsync returns false when player does not exist")]
    public async Task ExistsAsync_WhenPlayerDoesNotExist_ReturnsFalse()
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

    #region Concurrent Operations Tests

    [Fact(DisplayName = "Repository handles concurrent read operations")]
    public async Task ConcurrentReads_WhenReadingMultiplePlayers_AllSucceed()
    {
        await SeedPlayersAsync(10);

        var tasks = Enumerable.Range(1, 10)
            .SelectMany(_ => Enumerable.Range(1, 5).Select(_ => _repository.GetAllAsync()));

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal(10, r.Count));
    }

    #endregion

    #region Logging Tests

    [Fact(DisplayName = "GetAllAsync logs retrieval operation")]
    public async Task GetAllAsync_WhenCalled_LogsOperation()
    {
        await SeedPlayersAsync(3);

        await _repository.GetAllAsync();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieved 3 players")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact(DisplayName = "AddAsync logs successful addition")]
    public async Task AddAsync_WhenSuccessful_LogsOperation()
    {
        var player = CreatePlayer("Test Player");

        await _repository.AddAsync(player);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully added player")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private static Player CreatePlayer(string name)
    {
        return new Player
        {
            UserId = "test-user",
            Name = name,
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg",
            CreatedBy = "test-creator"
        };
    }

    private async Task SeedPlayersAsync(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            _context.Players.Add(CreatePlayer($"Player {i}"));
        }

        await _context.SaveChangesAsync();
    }

    #endregion
}
