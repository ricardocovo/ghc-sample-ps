using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Repositories.Implementations;

namespace GhcSamplePs.Core.Tests.Repositories.PlayerManagement;

public class MockPlayerRepositoryTests
{
    [Fact(DisplayName = "GetAllAsync returns pre-seeded players")]
    public async Task GetAllAsync_ReturnsPreSeededPlayers()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact(DisplayName = "GetByIdAsync returns player when valid ID provided")]
    public async Task GetByIdAsync_WithValidId_ReturnsPlayer()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact(DisplayName = "GetByIdAsync returns null when invalid ID provided")]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact(DisplayName = "AddAsync adds player with generated ID")]
    public async Task AddAsync_AddsPlayerWithGeneratedId()
    {
        var repository = new MockPlayerRepository();
        var player = new Player
        {
            UserId = "test-user",
            Name = "New Player",
            DateOfBirth = new DateTime(2010, 1, 1),
            CreatedBy = "test"
        };

        var result = await repository.AddAsync(player);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Player", result.Name);
    }

    [Fact(DisplayName = "AddAsync increments ID for subsequent players")]
    public async Task AddAsync_IncrementsIdForSubsequentPlayers()
    {
        var repository = new MockPlayerRepository();
        var player1 = new Player
        {
            UserId = "test-user",
            Name = "Player 1",
            DateOfBirth = new DateTime(2010, 1, 1),
            CreatedBy = "test"
        };
        var player2 = new Player
        {
            UserId = "test-user",
            Name = "Player 2",
            DateOfBirth = new DateTime(2010, 1, 1),
            CreatedBy = "test"
        };

        var result1 = await repository.AddAsync(player1);
        var result2 = await repository.AddAsync(player2);

        Assert.True(result2.Id > result1.Id);
    }

    [Fact(DisplayName = "AddAsync throws when player is null")]
    public async Task AddAsync_WhenPlayerNull_ThrowsArgumentNullException()
    {
        var repository = new MockPlayerRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repository.AddAsync(null!));
    }

    [Fact(DisplayName = "UpdateAsync updates existing player")]
    public async Task UpdateAsync_UpdatesExistingPlayer()
    {
        var repository = new MockPlayerRepository();
        var existingPlayer = await repository.GetByIdAsync(1);
        Assert.NotNull(existingPlayer);

        var updatedPlayer = new Player
        {
            Id = 1,
            UserId = existingPlayer.UserId,
            Name = "Updated Name",
            DateOfBirth = existingPlayer.DateOfBirth,
            CreatedBy = existingPlayer.CreatedBy,
            CreatedAt = existingPlayer.CreatedAt
        };

        var result = await repository.UpdateAsync(updatedPlayer);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);

        var verifyPlayer = await repository.GetByIdAsync(1);
        Assert.NotNull(verifyPlayer);
        Assert.Equal("Updated Name", verifyPlayer.Name);
    }

    [Fact(DisplayName = "UpdateAsync throws when player does not exist")]
    public async Task UpdateAsync_WithNonExistentId_ThrowsInvalidOperationException()
    {
        var repository = new MockPlayerRepository();
        var player = new Player
        {
            Id = 999,
            UserId = "test-user",
            Name = "Non-existent",
            DateOfBirth = new DateTime(2010, 1, 1),
            CreatedBy = "test"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repository.UpdateAsync(player));
    }

    [Fact(DisplayName = "UpdateAsync throws when player is null")]
    public async Task UpdateAsync_WhenPlayerNull_ThrowsArgumentNullException()
    {
        var repository = new MockPlayerRepository();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            repository.UpdateAsync(null!));
    }

    [Fact(DisplayName = "DeleteAsync deletes existing player")]
    public async Task DeleteAsync_WithExistingId_ReturnsTrue()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.DeleteAsync(1);

        Assert.True(result);

        var deletedPlayer = await repository.GetByIdAsync(1);
        Assert.Null(deletedPlayer);
    }

    [Fact(DisplayName = "DeleteAsync returns false for non-existent ID")]
    public async Task DeleteAsync_WithNonExistentId_ReturnsFalse()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "ExistsAsync returns true when player exists")]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.ExistsAsync(1);

        Assert.True(result);
    }

    [Fact(DisplayName = "ExistsAsync returns false when player does not exist")]
    public async Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        var repository = new MockPlayerRepository();

        var result = await repository.ExistsAsync(999);

        Assert.False(result);
    }

    [Fact(DisplayName = "Pre-seeded data contains diverse players")]
    public async Task PreSeededData_ContainsDiversePlayers()
    {
        var repository = new MockPlayerRepository();

        var players = await repository.GetAllAsync();

        Assert.Contains(players, p => p.Gender == "Male");
        Assert.Contains(players, p => p.Gender == "Female");
        Assert.Contains(players, p => p.Gender == "Non-binary");
        Assert.Contains(players, p => p.Gender == "Prefer not to say");
    }

    [Fact(DisplayName = "Pre-seeded data has valid properties")]
    public async Task PreSeededData_HasValidProperties()
    {
        var repository = new MockPlayerRepository();

        var players = await repository.GetAllAsync();

        foreach (var player in players)
        {
            Assert.True(player.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(player.Name));
            Assert.False(string.IsNullOrWhiteSpace(player.UserId));
            Assert.False(string.IsNullOrWhiteSpace(player.CreatedBy));
            Assert.True(player.DateOfBirth < DateTime.UtcNow);
        }
    }

    [Fact(DisplayName = "Repository is thread-safe for concurrent reads")]
    public async Task Repository_IsConcurrentReadSafe()
    {
        var repository = new MockPlayerRepository();
        var tasks = new List<Task<IReadOnlyList<Player>>>();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(repository.GetAllAsync());
        }

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal(10, r.Count));
    }

    [Fact(DisplayName = "Added player can be retrieved")]
    public async Task AddAsync_PlayerCanBeRetrieved()
    {
        var repository = new MockPlayerRepository();
        var player = new Player
        {
            UserId = "test-user",
            Name = "Test Player",
            DateOfBirth = new DateTime(2010, 1, 1),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg",
            CreatedBy = "test"
        };

        var addedPlayer = await repository.AddAsync(player);
        var retrievedPlayer = await repository.GetByIdAsync(addedPlayer.Id);

        Assert.NotNull(retrievedPlayer);
        Assert.Equal("Test Player", retrievedPlayer.Name);
        Assert.Equal("Male", retrievedPlayer.Gender);
        Assert.Equal("https://example.com/photo.jpg", retrievedPlayer.PhotoUrl);
    }
}
