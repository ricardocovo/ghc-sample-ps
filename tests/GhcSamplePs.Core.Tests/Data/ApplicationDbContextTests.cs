using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace GhcSamplePs.Core.Tests.Data;

/// <summary>
/// Tests for the ApplicationDbContext class.
/// </summary>
public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly string _currentUserId = "test-user-id";
    private bool _disposed;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, _currentUserId);
    }

    #region Constructor Tests

    [Fact(DisplayName = "ApplicationDbContext uses 'system' as default user when currentUserId is null")]
    public async Task Constructor_WhenCurrentUserIdIsNull_UsesSystemAsDefault()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options, null);
        var player = CreateValidPlayerWithoutCreatedBy();

        context.Players.Add(player);
        await context.SaveChangesAsync();

        var savedPlayer = await context.Players.FirstAsync();
        Assert.Equal("system", savedPlayer.CreatedBy);
    }

    #endregion

    #region SaveChangesAsync Audit Field Tests

    [Fact(DisplayName = "SaveChangesAsync sets CreatedAt for new entities")]
    public async Task SaveChangesAsync_WhenAddingEntity_SetsCreatedAt()
    {
        var beforeSave = DateTime.UtcNow;
        var player = TestPlayerFactory.CreateMinimalPlayer();

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var afterSave = DateTime.UtcNow;
        var savedPlayer = await _context.Players.FirstAsync();

        Assert.True(savedPlayer.CreatedAt >= beforeSave);
        Assert.True(savedPlayer.CreatedAt <= afterSave);
    }

    [Fact(DisplayName = "SaveChangesAsync sets CreatedBy when not provided")]
    public async Task SaveChangesAsync_WhenAddingEntityWithoutCreatedBy_SetsCreatedBy()
    {
        var player = CreateValidPlayerWithoutCreatedBy();

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var savedPlayer = await _context.Players.FirstAsync();
        Assert.Equal(_currentUserId, savedPlayer.CreatedBy);
    }

    [Fact(DisplayName = "SaveChangesAsync preserves existing CreatedBy when provided")]
    public async Task SaveChangesAsync_WhenAddingEntityWithCreatedBy_PreservesCreatedBy()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var savedPlayer = await _context.Players.FirstAsync();
        Assert.Equal("test-user", savedPlayer.CreatedBy);
    }

    [Fact(DisplayName = "SaveChangesAsync sets UpdatedAt for modified entities")]
    public async Task SaveChangesAsync_WhenModifyingEntity_SetsUpdatedAt()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        _context.Entry(player).State = EntityState.Modified;
        var beforeUpdate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var afterUpdate = DateTime.UtcNow;

        var updatedPlayer = await _context.Players.FirstAsync();
        Assert.NotNull(updatedPlayer.UpdatedAt);
        Assert.True(updatedPlayer.UpdatedAt >= beforeUpdate);
        Assert.True(updatedPlayer.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "SaveChangesAsync sets UpdatedBy for modified entities")]
    public async Task SaveChangesAsync_WhenModifyingEntity_SetsUpdatedBy()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        _context.Entry(player).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updatedPlayer = await _context.Players.FirstAsync();
        Assert.Equal(_currentUserId, updatedPlayer.UpdatedBy);
    }

    [Fact(DisplayName = "SaveChangesAsync does not modify CreatedAt for modified entities")]
    public async Task SaveChangesAsync_WhenModifyingEntity_DoesNotModifyCreatedAt()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var originalCreatedAt = player.CreatedAt;
        await Task.Delay(10);

        _context.Entry(player).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updatedPlayer = await _context.Players.FirstAsync();
        Assert.Equal(originalCreatedAt, updatedPlayer.CreatedAt);
    }

    [Fact(DisplayName = "SaveChangesAsync does not modify CreatedBy for modified entities")]
    public async Task SaveChangesAsync_WhenModifyingEntity_DoesNotModifyCreatedBy()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var originalCreatedBy = player.CreatedBy;

        _context.Entry(player).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        var updatedPlayer = await _context.Players.FirstAsync();
        Assert.Equal(originalCreatedBy, updatedPlayer.CreatedBy);
    }

    #endregion

    #region SaveChanges (Synchronous) Audit Field Tests

    [Fact(DisplayName = "SaveChanges sets CreatedAt for new entities")]
    public void SaveChanges_WhenAddingEntity_SetsCreatedAt()
    {
        var beforeSave = DateTime.UtcNow;
        var player = TestPlayerFactory.CreateMinimalPlayer();

        _context.Players.Add(player);
        _context.SaveChanges();

        var afterSave = DateTime.UtcNow;
        var savedPlayer = _context.Players.First();

        Assert.True(savedPlayer.CreatedAt >= beforeSave);
        Assert.True(savedPlayer.CreatedAt <= afterSave);
    }

    [Fact(DisplayName = "SaveChanges sets UpdatedAt for modified entities")]
    public void SaveChanges_WhenModifyingEntity_SetsUpdatedAt()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        _context.SaveChanges();

        _context.Entry(player).State = EntityState.Modified;
        var beforeUpdate = DateTime.UtcNow;
        _context.SaveChanges();
        var afterUpdate = DateTime.UtcNow;

        var updatedPlayer = _context.Players.First();
        Assert.NotNull(updatedPlayer.UpdatedAt);
        Assert.True(updatedPlayer.UpdatedAt >= beforeUpdate);
        Assert.True(updatedPlayer.UpdatedAt <= afterUpdate);
    }

    #endregion

    #region Multiple Entities Tests

    [Fact(DisplayName = "SaveChangesAsync handles multiple new entities")]
    public async Task SaveChangesAsync_WithMultipleNewEntities_SetsAuditFieldsForAll()
    {
        var beforeSave = DateTime.UtcNow;
        var player1 = TestPlayerFactory.CreateMinimalPlayer();
        var player2 = CreatePlayerWithDifferentName("Second Player");

        _context.Players.AddRange(player1, player2);
        await _context.SaveChangesAsync();

        var afterSave = DateTime.UtcNow;
        var players = await _context.Players.ToListAsync();

        Assert.Equal(2, players.Count);
        foreach (var player in players)
        {
            Assert.True(player.CreatedAt >= beforeSave);
            Assert.True(player.CreatedAt <= afterSave);
        }
    }

    [Fact(DisplayName = "SaveChangesAsync handles mixed add and update operations")]
    public async Task SaveChangesAsync_WithMixedOperations_SetsAppropriateAuditFields()
    {
        var existingPlayer = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(existingPlayer);
        await _context.SaveChangesAsync();

        var originalCreatedAt = existingPlayer.CreatedAt;

        await Task.Delay(10);

        var newPlayer = CreatePlayerWithDifferentName("New Player");
        _context.Players.Add(newPlayer);
        _context.Entry(existingPlayer).State = EntityState.Modified;

        var beforeSave = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var afterSave = DateTime.UtcNow;

        var savedNewPlayer = await _context.Players.FirstAsync(p => p.Id == newPlayer.Id);
        var savedExistingPlayer = await _context.Players.FirstAsync(p => p.Id == existingPlayer.Id);

        Assert.True(savedNewPlayer.CreatedAt >= beforeSave);
        Assert.True(savedNewPlayer.CreatedAt <= afterSave);

        Assert.Equal(originalCreatedAt, savedExistingPlayer.CreatedAt);
        Assert.NotNull(savedExistingPlayer.UpdatedAt);
        Assert.True(savedExistingPlayer.UpdatedAt >= beforeSave);
    }

    #endregion

    #region Entity Configuration Tests

    [Fact(DisplayName = "Players DbSet is configured correctly")]
    public void Players_DbSet_IsNotNull()
    {
        Assert.NotNull(_context.Players);
    }

    [Fact(DisplayName = "Player Id is auto-generated on add")]
    public async Task PlayerId_WhenAddingEntity_IsAutoGenerated()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        Assert.Equal(0, player.Id);

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        Assert.NotEqual(0, player.Id);
    }

    [Fact(DisplayName = "Player can be retrieved by Id")]
    public async Task Player_WhenAdded_CanBeRetrievedById()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var retrievedPlayer = await _context.Players.FindAsync(player.Id);

        Assert.NotNull(retrievedPlayer);
        Assert.Equal(player.Name, retrievedPlayer.Name);
        Assert.Equal(player.UserId, retrievedPlayer.UserId);
    }

    #endregion

    #region Helper Methods

    private static Player CreateValidPlayerWithoutCreatedBy()
    {
        return new Player
        {
            UserId = "test-owner-id",
            Name = "Test Player",
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedBy = ""
        };
    }

    private static Player CreatePlayerWithDifferentName(string name)
    {
        return new Player
        {
            UserId = "test-owner-id",
            Name = name,
            DateOfBirth = new DateTime(1985, 6, 15),
            CreatedBy = "test-user"
        };
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
