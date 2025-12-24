using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services;

public sealed class BlobStorageServiceTests
{
    private readonly Mock<ILogger<BlobStorageService>> _loggerMock;
    private readonly Mock<IConfiguration> _configurationMock;

    public BlobStorageServiceTests()
    {
        _loggerMock = new Mock<ILogger<BlobStorageService>>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["AzureStorage:PlayerPicturesContainer"])
            .Returns("player-pictures");
        _configurationMock.Setup(c => c["AzureStorage:SasExpirationMinutes"])
            .Returns("60");
        _configurationMock.Setup(c => c["AzureStorage:ConnectionString"])
            .Returns("UseDevelopmentStorage=true");
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BlobStorageService(null!, _configurationMock.Object));
    }

    [Fact]
    public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new BlobStorageService(_loggerMock.Object, null!));
    }

    [Fact]
    public void GeneratePlayerBlobName_ValidPlayerId_ReturnsUniqueName()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var blobName = service.GeneratePlayerBlobName(123, ".jpg");

        Assert.NotNull(blobName);
        Assert.StartsWith("player-123-", blobName);
        Assert.EndsWith(".jpg", blobName);
    }

    [Fact]
    public void GeneratePlayerBlobName_ExtensionWithoutDot_AddsDot()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var blobName = service.GeneratePlayerBlobName(123, "jpg");

        Assert.EndsWith(".jpg", blobName);
    }

    [Fact]
    public void GeneratePlayerBlobName_InvalidPlayerId_ThrowsArgumentException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        Assert.Throws<ArgumentException>(() =>
            service.GeneratePlayerBlobName(0, ".jpg"));
    }

    [Fact]
    public void GeneratePlayerBlobName_NullExtension_ThrowsArgumentNullException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        Assert.Throws<ArgumentNullException>(() =>
            service.GeneratePlayerBlobName(123, null!));
    }

    [Fact]
    public void GeneratePlayerBlobName_EmptyExtension_ThrowsArgumentException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        Assert.Throws<ArgumentException>(() =>
            service.GeneratePlayerBlobName(123, ""));
    }

    [Fact]
    public void GeneratePlayerBlobName_DifferentPlayerIds_ReturnsDifferentNames()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var blobName1 = service.GeneratePlayerBlobName(1, ".jpg");
        var blobName2 = service.GeneratePlayerBlobName(2, ".jpg");

        Assert.NotEqual(blobName1, blobName2);
        Assert.Contains("player-1-", blobName1);
        Assert.Contains("player-2-", blobName2);
    }

    [Fact]
    public void GeneratePlayerBlobName_SamePlayerIdCalledTwice_ReturnsConsistentFormat()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var blobName1 = service.GeneratePlayerBlobName(123, ".jpg");
        var blobName2 = service.GeneratePlayerBlobName(123, ".jpg");

        Assert.StartsWith("player-123-", blobName1);
        Assert.StartsWith("player-123-", blobName2);
        Assert.EndsWith(".jpg", blobName1);
        Assert.EndsWith(".jpg", blobName2);
    }

    [Theory]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".png")]
    [InlineData(".gif")]
    [InlineData(".webp")]
    public void GeneratePlayerBlobName_DifferentExtensions_PreservesExtension(string extension)
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var blobName = service.GeneratePlayerBlobName(123, extension);

        Assert.EndsWith(extension, blobName);
    }

    [Fact]
    public async Task UploadPlayerPictureAsync_NullFileContent_ThrowsArgumentNullException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.UploadPlayerPictureAsync(null!, "test.jpg", "image/jpeg", 1));
    }

    [Fact]
    public async Task UploadPlayerPictureAsync_EmptyFileName_ThrowsArgumentException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);
        var fileContent = new byte[1024];

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UploadPlayerPictureAsync(fileContent, "", "image/jpeg", 1));
    }

    [Fact]
    public async Task UploadPlayerPictureAsync_NullContentType_ThrowsArgumentNullException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);
        var fileContent = new byte[1024];

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.UploadPlayerPictureAsync(fileContent, "test.jpg", null!, 1));
    }

    [Fact]
    public async Task UploadPlayerPictureAsync_InvalidPlayerId_ReturnsFailure()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);
        var fileContent = new byte[1024];

        var result = await service.UploadPlayerPictureAsync(fileContent, "test.jpg", "image/jpeg", 0);

        Assert.False(result.Success);
        Assert.Contains("Player ID must be greater than 0", result.ErrorMessages[0]);
    }

    [Fact]
    public async Task UploadPlayerPictureAsync_EmptyFileContent_ReturnsFailure()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);
        var fileContent = Array.Empty<byte>();

        var result = await service.UploadPlayerPictureAsync(fileContent, "test.jpg", "image/jpeg", 1);

        Assert.False(result.Success);
        Assert.Contains("File content cannot be empty", result.ErrorMessages[0]);
    }

    [Fact]
    public async Task DeletePlayerPictureAsync_NullBlobName_ThrowsArgumentNullException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.DeletePlayerPictureAsync(null!));
    }

    [Fact]
    public async Task DeletePlayerPictureAsync_EmptyBlobName_ThrowsArgumentException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.DeletePlayerPictureAsync(""));
    }

    [Fact]
    public async Task GetPictureUrlWithSasAsync_NullBlobName_ThrowsArgumentNullException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.GetPictureUrlWithSasAsync(null!));
    }

    [Fact]
    public async Task GetPictureUrlWithSasAsync_EmptyBlobName_ThrowsArgumentException()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetPictureUrlWithSasAsync(""));
    }

    [Fact]
    public async Task GetPictureUrlWithSasAsync_InvalidExpirationMinutes_ReturnsFailure()
    {
        var service = new BlobStorageService(_loggerMock.Object, _configurationMock.Object);

        var result = await service.GetPictureUrlWithSasAsync("test-blob.jpg", 0);

        Assert.False(result.Success);
        Assert.Contains("Expiration minutes must be greater than 0", result.ErrorMessages[0]);
    }
}
