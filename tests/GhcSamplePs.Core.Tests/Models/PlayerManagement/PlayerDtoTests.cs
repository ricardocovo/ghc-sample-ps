using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.PlayerManagement;

public class PlayerDtoTests
{
    #region PlayerDto Tests

    [Fact(DisplayName = "PlayerDto can be created with required properties")]
    public void PlayerDto_WithRequiredProperties_CreatesSuccessfully()
    {
        var dto = new PlayerDto
        {
            Id = 1,
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Age = 34,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test-user"
        };

        Assert.NotNull(dto);
        Assert.Equal(1, dto.Id);
        Assert.Equal("owner-123", dto.UserId);
        Assert.Equal("John Doe", dto.Name);
        Assert.Equal(34, dto.Age);
    }

    [Fact(DisplayName = "PlayerDto.FromEntity creates correct DTO from Player")]
    public void FromEntity_WithValidPlayer_CreatesCorrectDto()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        var dto = PlayerDto.FromEntity(player);

        Assert.Equal(player.Id, dto.Id);
        Assert.Equal(player.UserId, dto.UserId);
        Assert.Equal(player.Name.Trim(), dto.Name);
        Assert.Equal(player.DateOfBirth, dto.DateOfBirth);
        Assert.Equal(player.Age, dto.Age);
        Assert.Equal(player.Gender?.Trim(), dto.Gender);
        Assert.Equal(player.PhotoUrl?.Trim(), dto.PhotoUrl);
        Assert.Equal(player.CreatedAt, dto.CreatedAt);
        Assert.Equal(player.CreatedBy, dto.CreatedBy);
    }

    [Fact(DisplayName = "PlayerDto.FromEntity trims string values")]
    public void FromEntity_TrimsStringValues()
    {
        var player = new Player
        {
            Id = 1,
            UserId = "owner-123",
            Name = "  John Doe  ",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "  Male  ",
            PhotoUrl = "  https://example.com/photo.jpg  ",
            CreatedBy = "test-user"
        };

        var dto = PlayerDto.FromEntity(player);

        Assert.Equal("John Doe", dto.Name);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("https://example.com/photo.jpg", dto.PhotoUrl);
    }

    [Fact(DisplayName = "PlayerDto.FromEntity throws when player is null")]
    public void FromEntity_WhenPlayerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerDto.FromEntity(null!));
    }

    [Fact(DisplayName = "PlayerDto.FromEntity includes UpdatedAt and UpdatedBy")]
    public void FromEntity_IncludesUpdateFields()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.UpdateLastModified("update-user");

        var dto = PlayerDto.FromEntity(player);

        Assert.NotNull(dto.UpdatedAt);
        Assert.Equal("update-user", dto.UpdatedBy);
    }

    #endregion

    #region CreatePlayerDto Tests

    [Fact(DisplayName = "CreatePlayerDto can be created with required properties")]
    public void CreatePlayerDto_WithRequiredProperties_CreatesSuccessfully()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        Assert.NotNull(dto);
        Assert.Equal("owner-123", dto.UserId);
        Assert.Equal("John Doe", dto.Name);
        Assert.Equal(new DateTime(1990, 6, 15), dto.DateOfBirth);
        Assert.Equal("Male", dto.Gender);
        Assert.Equal("https://example.com/photo.jpg", dto.PhotoUrl);
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity creates correct Player")]
    public void ToEntity_WithValidDto_CreatesCorrectPlayer()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        var player = dto.ToEntity("test-user");

        Assert.Equal("owner-123", player.UserId);
        Assert.Equal("John Doe", player.Name);
        Assert.Equal(new DateTime(1990, 6, 15), player.DateOfBirth);
        Assert.Equal("Male", player.Gender);
        Assert.Equal("https://example.com/photo.jpg", player.PhotoUrl);
        Assert.Equal("test-user", player.CreatedBy);
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity trims string values")]
    public void ToEntity_TrimsStringValues()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "  owner-123  ",
            Name = "  John Doe  ",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "  Male  ",
            PhotoUrl = "  https://example.com/photo.jpg  "
        };

        var player = dto.ToEntity("test-user");

        Assert.Equal("owner-123", player.UserId);
        Assert.Equal("John Doe", player.Name);
        Assert.Equal("Male", player.Gender);
        Assert.Equal("https://example.com/photo.jpg", player.PhotoUrl);
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity sets CreatedAt to current UTC time")]
    public void ToEntity_SetsCreatedAtToCurrentUtcTime()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var beforeCreation = DateTime.UtcNow;
        var player = dto.ToEntity("test-user");
        var afterCreation = DateTime.UtcNow;

        Assert.True(player.CreatedAt >= beforeCreation);
        Assert.True(player.CreatedAt <= afterCreation);
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity throws when createdBy is null")]
    public void ToEntity_WhenCreatedByIsNull_ThrowsArgumentNullException()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        Assert.Throws<ArgumentNullException>(() => dto.ToEntity(null!));
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity throws when createdBy is empty")]
    public void ToEntity_WhenCreatedByIsEmpty_ThrowsArgumentException()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        Assert.Throws<ArgumentException>(() => dto.ToEntity(""));
    }

    [Fact(DisplayName = "CreatePlayerDto.ToEntity throws when createdBy is whitespace")]
    public void ToEntity_WhenCreatedByIsWhitespace_ThrowsArgumentException()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        Assert.Throws<ArgumentException>(() => dto.ToEntity("   "));
    }

    #endregion

    #region UpdatePlayerDto Tests

    [Fact(DisplayName = "UpdatePlayerDto can be created with required properties")]
    public void UpdatePlayerDto_WithRequiredProperties_CreatesSuccessfully()
    {
        var dto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "John Doe Updated",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo-updated.jpg"
        };

        Assert.NotNull(dto);
        Assert.Equal(1, dto.Id);
        Assert.Equal("John Doe Updated", dto.Name);
        Assert.Equal(new DateTime(1990, 6, 15), dto.DateOfBirth);
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo creates updated Player")]
    public void ApplyTo_WithValidDto_CreatesUpdatedPlayer()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20),
            Gender = "Non-binary",
            PhotoUrl = "https://example.com/new-photo.jpg"
        };

        var updatedPlayer = dto.ApplyTo(existingPlayer, "update-user");

        Assert.Equal(existingPlayer.Id, updatedPlayer.Id);
        Assert.Equal(existingPlayer.UserId, updatedPlayer.UserId);
        Assert.Equal("Updated Name", updatedPlayer.Name);
        Assert.Equal(new DateTime(1991, 7, 20), updatedPlayer.DateOfBirth);
        Assert.Equal("Non-binary", updatedPlayer.Gender);
        Assert.Equal("https://example.com/new-photo.jpg", updatedPlayer.PhotoUrl);
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo preserves original UserId, CreatedAt and CreatedBy")]
    public void ApplyTo_PreservesOriginalAuditFields()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var originalUserId = existingPlayer.UserId;
        var originalCreatedAt = existingPlayer.CreatedAt;
        var originalCreatedBy = existingPlayer.CreatedBy;
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        var updatedPlayer = dto.ApplyTo(existingPlayer, "update-user");

        Assert.Equal(originalUserId, updatedPlayer.UserId);
        Assert.Equal(originalCreatedAt, updatedPlayer.CreatedAt);
        Assert.Equal(originalCreatedBy, updatedPlayer.CreatedBy);
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo sets UpdatedAt and UpdatedBy")]
    public void ApplyTo_SetsUpdateAuditFields()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        var beforeUpdate = DateTime.UtcNow;
        var updatedPlayer = dto.ApplyTo(existingPlayer, "update-user");
        var afterUpdate = DateTime.UtcNow;

        Assert.NotNull(updatedPlayer.UpdatedAt);
        Assert.True(updatedPlayer.UpdatedAt >= beforeUpdate);
        Assert.True(updatedPlayer.UpdatedAt <= afterUpdate);
        Assert.Equal("update-user", updatedPlayer.UpdatedBy);
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo trims string values")]
    public void ApplyTo_TrimsStringValues()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "  Updated Name  ",
            DateOfBirth = new DateTime(1991, 7, 20),
            Gender = "  Male  ",
            PhotoUrl = "  https://example.com/photo.jpg  "
        };

        var updatedPlayer = dto.ApplyTo(existingPlayer, "update-user");

        Assert.Equal("Updated Name", updatedPlayer.Name);
        Assert.Equal("Male", updatedPlayer.Gender);
        Assert.Equal("https://example.com/photo.jpg", updatedPlayer.PhotoUrl);
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo throws when existingPlayer is null")]
    public void ApplyTo_WhenExistingPlayerIsNull_ThrowsArgumentNullException()
    {
        var dto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        Assert.Throws<ArgumentNullException>(() => dto.ApplyTo(null!, "update-user"));
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo throws when updatedBy is null")]
    public void ApplyTo_WhenUpdatedByIsNull_ThrowsArgumentNullException()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        Assert.Throws<ArgumentNullException>(() => dto.ApplyTo(existingPlayer, null!));
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo throws when updatedBy is empty")]
    public void ApplyTo_WhenUpdatedByIsEmpty_ThrowsArgumentException()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        Assert.Throws<ArgumentException>(() => dto.ApplyTo(existingPlayer, ""));
    }

    [Fact(DisplayName = "UpdatePlayerDto.ApplyTo throws when updatedBy is whitespace")]
    public void ApplyTo_WhenUpdatedByIsWhitespace_ThrowsArgumentException()
    {
        var existingPlayer = TestPlayerFactory.CreateValidPlayer();
        var dto = new UpdatePlayerDto
        {
            Id = existingPlayer.Id,
            Name = "Updated Name",
            DateOfBirth = new DateTime(1991, 7, 20)
        };

        Assert.Throws<ArgumentException>(() => dto.ApplyTo(existingPlayer, "   "));
    }

    #endregion
}
