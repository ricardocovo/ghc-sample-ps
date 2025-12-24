using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Repositories.Interfaces;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services.PlayerManagement;

public sealed class PlayerServicePictureTests
{
    private readonly Mock<IPlayerRepository> _mockRepository;
    private readonly Mock<ILogger<PlayerService>> _mockLogger;
    private readonly Mock<IBlobStorageService> _mockBlobStorage;
    private readonly PlayerService _service;
    private const string TestUserId = "test-owner-id";

    public PlayerServicePictureTests()
    {
        _mockRepository = new Mock<IPlayerRepository>();
        _mockLogger = new Mock<ILogger<PlayerService>>();
        _mockBlobStorage = new Mock<IBlobStorageService>();
        _service = new PlayerService(
            _mockRepository.Object,
            _mockLogger.Object,
            _mockBlobStorage.Object);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync uploads picture successfully for valid data")]
    public async Task UploadPlayerPictureAsync_ValidData_ReturnsSuccess()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = null;

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.UploadPlayerPictureAsync(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<(string, string)>.Ok(("https://blob.url/test.jpg", "player-1-test.jpg")));

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Success);
        Assert.Equal("https://blob.url/test.jpg", result.Data.PictureUrl);

        _mockRepository.Verify(r => r.UpdateAsync(
            It.Is<Player>(p => p.PhotoUrl == "https://blob.url/test.jpg"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync throws when uploadDto is null")]
    public async Task UploadPlayerPictureAsync_NullDto_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadPlayerPictureAsync(null!, TestUserId));
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync throws when currentUserId is null")]
    public async Task UploadPlayerPictureAsync_NullUserId_ThrowsArgumentNullException()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadPlayerPictureAsync(uploadDto, null!));
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync returns failure when blob storage not configured")]
    public async Task UploadPlayerPictureAsync_NoBlobStorage_ReturnsFailure()
    {
        var serviceWithoutBlobStorage = new PlayerService(
            _mockRepository.Object,
            _mockLogger.Object,
            null);

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = await serviceWithoutBlobStorage.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("not configured", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync returns validation error for invalid file size")]
    public async Task UploadPlayerPictureAsync_InvalidFileSize_ReturnsValidationError()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[6 * 1024 * 1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 6 * 1024 * 1024
        };

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.False(result.Success);
        Assert.NotEmpty(result.ValidationErrors);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync returns failure when player not found")]
    public async Task UploadPlayerPictureAsync_PlayerNotFound_ReturnsFailure()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 999,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync returns failure when user not authorized")]
    public async Task UploadPlayerPictureAsync_UnauthorizedUser_ReturnsFailure()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var result = await _service.UploadPlayerPictureAsync(uploadDto, "different-user");

        Assert.False(result.Success);
        Assert.Contains("permission", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync replaces existing picture")]
    public async Task UploadPlayerPictureAsync_ExistingPicture_ReplacesOldPicture()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = "https://blob.url/old-picture.jpg";

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "new-picture.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.DeletePlayerPictureAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Ok(true));

        _mockBlobStorage.Setup(b => b.UploadPlayerPictureAsync(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<(string, string)>.Ok(("https://blob.url/new-picture.jpg", "player-1-new.jpg")));

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.True(result.Success);

        _mockBlobStorage.Verify(b => b.DeletePlayerPictureAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync continues when old picture deletion fails")]
    public async Task UploadPlayerPictureAsync_OldPictureDeletionFails_ContinuesWithUpload()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = "https://blob.url/old-picture.jpg";

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "new-picture.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.DeletePlayerPictureAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Deletion failed"));

        _mockBlobStorage.Setup(b => b.UploadPlayerPictureAsync(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<(string, string)>.Ok(("https://blob.url/new-picture.jpg", "player-1-new.jpg")));

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.True(result.Success);
        Assert.Equal("https://blob.url/new-picture.jpg", result.Data!.PictureUrl);

        _mockBlobStorage.Verify(b => b.DeletePlayerPictureAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockBlobStorage.Verify(b => b.UploadPlayerPictureAsync(
            It.IsAny<byte[]>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "UploadPlayerPictureAsync returns failure when blob upload fails")]
    public async Task UploadPlayerPictureAsync_BlobUploadFails_ReturnsFailure()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;

        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.UploadPlayerPictureAsync(
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<(string, string)>.Fail("Storage error"));

        var result = await _service.UploadPlayerPictureAsync(uploadDto, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("Storage error", result.ErrorMessages[0]);

        _mockRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Player>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync deletes picture successfully")]
    public async Task DeletePlayerPictureAsync_ValidData_ReturnsSuccess()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = "https://blob.url/player-1-test.jpg";

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.DeletePlayerPictureAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Ok(true));

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Player>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player p, CancellationToken ct) => p);

        var result = await _service.DeletePlayerPictureAsync(1, TestUserId);

        Assert.True(result.Success);

        _mockRepository.Verify(r => r.UpdateAsync(
            It.Is<Player>(p => p.PhotoUrl == null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync throws when currentUserId is null")]
    public async Task DeletePlayerPictureAsync_NullUserId_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.DeletePlayerPictureAsync(1, null!));
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync returns failure for invalid player ID")]
    public async Task DeletePlayerPictureAsync_InvalidPlayerId_ReturnsFailure()
    {
        var result = await _service.DeletePlayerPictureAsync(0, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("Invalid player ID", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync returns failure when blob storage not configured")]
    public async Task DeletePlayerPictureAsync_NoBlobStorage_ReturnsFailure()
    {
        var serviceWithoutBlobStorage = new PlayerService(
            _mockRepository.Object,
            _mockLogger.Object,
            null);

        var result = await serviceWithoutBlobStorage.DeletePlayerPictureAsync(1, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("not configured", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync returns failure when player not found")]
    public async Task DeletePlayerPictureAsync_PlayerNotFound_ReturnsFailure()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Player?)null);

        var result = await _service.DeletePlayerPictureAsync(999, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync returns failure when user not authorized")]
    public async Task DeletePlayerPictureAsync_UnauthorizedUser_ReturnsFailure()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = "https://blob.url/test.jpg";

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var result = await _service.DeletePlayerPictureAsync(1, "different-user");

        Assert.False(result.Success);
        Assert.Contains("permission", result.ErrorMessages[0]);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync succeeds when player has no picture")]
    public async Task DeletePlayerPictureAsync_NoPicture_ReturnsSuccess()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = null;

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        var result = await _service.DeletePlayerPictureAsync(1, TestUserId);

        Assert.True(result.Success);

        _mockBlobStorage.Verify(b => b.DeletePlayerPictureAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "DeletePlayerPictureAsync returns failure when blob deletion fails")]
    public async Task DeletePlayerPictureAsync_BlobDeletionFails_ReturnsFailure()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        player.Id = 1;
        player.PhotoUrl = "https://blob.url/player-1-test.jpg";

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(player);

        _mockBlobStorage.Setup(b => b.DeletePlayerPictureAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<bool>.Fail("Deletion failed"));

        var result = await _service.DeletePlayerPictureAsync(1, TestUserId);

        Assert.False(result.Success);
        Assert.Contains("Deletion failed", result.ErrorMessages[0]);

        _mockRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Player>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
