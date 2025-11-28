using GhcSamplePs.Core.Data;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace GhcSamplePs.Core.Tests.Data.Configurations;

/// <summary>
/// Tests for the PlayerConfiguration entity configuration.
/// </summary>
public class PlayerConfigurationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;

    public PlayerConfigurationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, "test-user");
    }

    #region Table Configuration Tests

    [Fact(DisplayName = "Player entity maps to Players table")]
    public void PlayerEntity_WhenQueried_MapsToPlayersTable()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        Assert.Equal("Players", entityType.GetTableName());
    }

    #endregion

    #region Primary Key Tests

    [Fact(DisplayName = "Player entity has Id as primary key")]
    public void PlayerEntity_PrimaryKey_IsId()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var primaryKey = entityType.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal(nameof(Player.Id), primaryKey.Properties[0].Name);
    }

    [Fact(DisplayName = "Player Id is configured for value generation")]
    public void PlayerId_IsConfiguredForValueGeneration()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var idProperty = entityType.FindProperty(nameof(Player.Id));
        Assert.NotNull(idProperty);
        Assert.Equal(Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd, idProperty.ValueGenerated);
    }

    #endregion

    #region Required Properties Tests

    [Fact(DisplayName = "Player UserId is required")]
    public void PlayerUserId_IsRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var userIdProperty = entityType.FindProperty(nameof(Player.UserId));
        Assert.NotNull(userIdProperty);
        Assert.False(userIdProperty.IsNullable);
    }

    [Fact(DisplayName = "Player Name is required")]
    public void PlayerName_IsRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var nameProperty = entityType.FindProperty(nameof(Player.Name));
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
    }

    [Fact(DisplayName = "Player DateOfBirth is required")]
    public void PlayerDateOfBirth_IsRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var dateOfBirthProperty = entityType.FindProperty(nameof(Player.DateOfBirth));
        Assert.NotNull(dateOfBirthProperty);
        Assert.False(dateOfBirthProperty.IsNullable);
    }

    [Fact(DisplayName = "Player CreatedAt is required")]
    public void PlayerCreatedAt_IsRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var createdAtProperty = entityType.FindProperty(nameof(Player.CreatedAt));
        Assert.NotNull(createdAtProperty);
        Assert.False(createdAtProperty.IsNullable);
    }

    [Fact(DisplayName = "Player CreatedBy is required")]
    public void PlayerCreatedBy_IsRequired()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var createdByProperty = entityType.FindProperty(nameof(Player.CreatedBy));
        Assert.NotNull(createdByProperty);
        Assert.False(createdByProperty.IsNullable);
    }

    #endregion

    #region Optional Properties Tests

    [Fact(DisplayName = "Player Gender is optional")]
    public void PlayerGender_IsOptional()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var genderProperty = entityType.FindProperty(nameof(Player.Gender));
        Assert.NotNull(genderProperty);
        Assert.True(genderProperty.IsNullable);
    }

    [Fact(DisplayName = "Player PhotoUrl is optional")]
    public void PlayerPhotoUrl_IsOptional()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var photoUrlProperty = entityType.FindProperty(nameof(Player.PhotoUrl));
        Assert.NotNull(photoUrlProperty);
        Assert.True(photoUrlProperty.IsNullable);
    }

    [Fact(DisplayName = "Player UpdatedAt is optional")]
    public void PlayerUpdatedAt_IsOptional()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var updatedAtProperty = entityType.FindProperty(nameof(Player.UpdatedAt));
        Assert.NotNull(updatedAtProperty);
        Assert.True(updatedAtProperty.IsNullable);
    }

    [Fact(DisplayName = "Player UpdatedBy is optional")]
    public void PlayerUpdatedBy_IsOptional()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var updatedByProperty = entityType.FindProperty(nameof(Player.UpdatedBy));
        Assert.NotNull(updatedByProperty);
        Assert.True(updatedByProperty.IsNullable);
    }

    #endregion

    #region Max Length Tests

    [Fact(DisplayName = "Player UserId has max length 450")]
    public void PlayerUserId_HasMaxLength450()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var userIdProperty = entityType.FindProperty(nameof(Player.UserId));
        Assert.NotNull(userIdProperty);
        Assert.Equal(450, userIdProperty.GetMaxLength());
    }

    [Fact(DisplayName = "Player Name has max length 200")]
    public void PlayerName_HasMaxLength200()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var nameProperty = entityType.FindProperty(nameof(Player.Name));
        Assert.NotNull(nameProperty);
        Assert.Equal(200, nameProperty.GetMaxLength());
    }

    [Fact(DisplayName = "Player Gender has max length 50")]
    public void PlayerGender_HasMaxLength50()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var genderProperty = entityType.FindProperty(nameof(Player.Gender));
        Assert.NotNull(genderProperty);
        Assert.Equal(50, genderProperty.GetMaxLength());
    }

    [Fact(DisplayName = "Player PhotoUrl has max length 500")]
    public void PlayerPhotoUrl_HasMaxLength500()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var photoUrlProperty = entityType.FindProperty(nameof(Player.PhotoUrl));
        Assert.NotNull(photoUrlProperty);
        Assert.Equal(500, photoUrlProperty.GetMaxLength());
    }

    [Fact(DisplayName = "Player CreatedBy has max length 450")]
    public void PlayerCreatedBy_HasMaxLength450()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var createdByProperty = entityType.FindProperty(nameof(Player.CreatedBy));
        Assert.NotNull(createdByProperty);
        Assert.Equal(450, createdByProperty.GetMaxLength());
    }

    [Fact(DisplayName = "Player UpdatedBy has max length 450")]
    public void PlayerUpdatedBy_HasMaxLength450()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var updatedByProperty = entityType.FindProperty(nameof(Player.UpdatedBy));
        Assert.NotNull(updatedByProperty);
        Assert.Equal(450, updatedByProperty.GetMaxLength());
    }

    #endregion

    #region Computed Property Tests

    [Fact(DisplayName = "Player Age property is ignored in entity configuration")]
    public void PlayerAge_IsIgnored()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var ageProperty = entityType.FindProperty(nameof(Player.Age));
        Assert.Null(ageProperty);
    }

    #endregion

    #region Index Tests

    [Fact(DisplayName = "Player UserId has index")]
    public void PlayerUserId_HasIndex()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var indexes = entityType.GetIndexes().ToList();
        var userIdIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Player.UserId)));
        Assert.NotNull(userIdIndex);
    }

    [Fact(DisplayName = "Player Name has index")]
    public void PlayerName_HasIndex()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var indexes = entityType.GetIndexes().ToList();
        var nameIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Player.Name)));
        Assert.NotNull(nameIndex);
    }

    [Fact(DisplayName = "Player DateOfBirth has index")]
    public void PlayerDateOfBirth_HasIndex()
    {
        var entityType = _context.Model.FindEntityType(typeof(Player));

        Assert.NotNull(entityType);
        var indexes = entityType.GetIndexes().ToList();
        var dateOfBirthIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == nameof(Player.DateOfBirth)));
        Assert.NotNull(dateOfBirthIndex);
    }

    #endregion

    #region Integration Tests

    [Fact(DisplayName = "Player can be saved with all properties")]
    public async Task Player_WithAllProperties_CanBeSaved()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var savedPlayer = await _context.Players.FindAsync(player.Id);

        Assert.NotNull(savedPlayer);
        Assert.Equal(player.Name, savedPlayer.Name);
        Assert.Equal(player.UserId, savedPlayer.UserId);
        Assert.Equal(player.DateOfBirth, savedPlayer.DateOfBirth);
        Assert.Equal(player.Gender, savedPlayer.Gender);
        Assert.Equal(player.PhotoUrl, savedPlayer.PhotoUrl);
    }

    [Fact(DisplayName = "Player can be saved with only required properties")]
    public async Task Player_WithOnlyRequiredProperties_CanBeSaved()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var savedPlayer = await _context.Players.FindAsync(player.Id);

        Assert.NotNull(savedPlayer);
        Assert.Equal(player.Name, savedPlayer.Name);
        Assert.Null(savedPlayer.Gender);
        Assert.Null(savedPlayer.PhotoUrl);
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
