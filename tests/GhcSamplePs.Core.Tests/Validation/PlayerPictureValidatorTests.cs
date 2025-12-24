using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Validation;

namespace GhcSamplePs.Core.Tests.Validation;

public sealed class PlayerPictureValidatorTests
{
    [Fact]
    public void ValidateUpload_ValidData_ReturnsValidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateUpload_NullDto_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PlayerPictureValidator.ValidateUpload(null!));
    }

    [Fact]
    public void ValidateUpload_InvalidPlayerId_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 0,
            FileContent = new byte[1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("PlayerId", result.Errors.Keys);
        Assert.Contains("Player ID must be greater than 0", result.Errors["PlayerId"][0]);
    }

    [Fact]
    public void ValidateUpload_EmptyFileContent_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = [],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 0
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileContent", result.Errors.Keys);
    }

    [Fact]
    public void ValidateUpload_FileSizeExceedsLimit_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[6 * 1024 * 1024],
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 6 * 1024 * 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileSizeBytes", result.Errors.Keys);
        Assert.Contains("exceeds the maximum", result.Errors["FileSizeBytes"][0]);
    }

    [Fact]
    public void ValidateUpload_EmptyFileName_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileName", result.Errors.Keys);
    }

    [Fact]
    public void ValidateUpload_UnsupportedContentType_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.txt",
            ContentType = "text/plain",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("ContentType", result.Errors.Keys);
        Assert.Contains("Unsupported content type", result.Errors["ContentType"][0]);
    }

    [Theory]
    [InlineData("image/jpeg")]
    [InlineData("image/png")]
    [InlineData("image/gif")]
    [InlineData("image/webp")]
    public void IsValidImageContentType_SupportedTypes_ReturnsTrue(string contentType)
    {
        var result = PlayerPictureValidator.IsValidImageContentType(contentType);

        Assert.True(result);
    }

    [Theory]
    [InlineData("text/plain")]
    [InlineData("application/pdf")]
    [InlineData("video/mp4")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValidImageContentType_UnsupportedTypes_ReturnsFalse(string? contentType)
    {
        var result = PlayerPictureValidator.IsValidImageContentType(contentType!);

        Assert.False(result);
    }

    [Fact]
    public void ValidateUpload_ExtensionMismatchesContentType_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.png",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileName", result.Errors.Keys);
        Assert.Contains("does not match content type", result.Errors["FileName"][0]);
    }

    [Theory]
    [InlineData("test.jpg", "image/jpeg")]
    [InlineData("test.jpeg", "image/jpeg")]
    [InlineData("test.png", "image/png")]
    [InlineData("test.gif", "image/gif")]
    [InlineData("test.webp", "image/webp")]
    public void ValidateUpload_MatchingExtensionAndContentType_ReturnsValidResult(
        string fileName,
        string contentType)
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateUpload_UnsupportedExtension_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test.bmp",
            ContentType = "image/bmp",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileName", result.Errors.Keys);
    }

    [Fact]
    public void ValidateUpload_FileWithoutExtension_ReturnsInvalidResult()
    {
        var uploadDto = new UploadPlayerPictureDto
        {
            PlayerId = 1,
            FileContent = new byte[1024],
            FileName = "test",
            ContentType = "image/jpeg",
            FileSizeBytes = 1024
        };

        var result = PlayerPictureValidator.ValidateUpload(uploadDto);

        Assert.False(result.IsValid);
        Assert.Contains("FileName", result.Errors.Keys);
    }

    [Fact]
    public void ValidateFileSize_ValidSize_NoErrors()
    {
        var errors = new Dictionary<string, List<string>>();

        PlayerPictureValidator.ValidateFileSize(1024 * 1024, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateFileSize_MaxSize_NoErrors()
    {
        var errors = new Dictionary<string, List<string>>();

        PlayerPictureValidator.ValidateFileSize(PlayerPictureValidator.MaxFileSizeBytes, errors);

        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateFileSize_ExceedsMax_AddsError()
    {
        var errors = new Dictionary<string, List<string>>();

        PlayerPictureValidator.ValidateFileSize(PlayerPictureValidator.MaxFileSizeBytes + 1, errors);

        Assert.Contains("FileSizeBytes", errors.Keys);
    }

    [Fact]
    public void ValidateFileSize_Zero_AddsError()
    {
        var errors = new Dictionary<string, List<string>>();

        PlayerPictureValidator.ValidateFileSize(0, errors);

        Assert.Contains("FileSizeBytes", errors.Keys);
    }

    [Fact]
    public void ValidateFileSize_Negative_AddsError()
    {
        var errors = new Dictionary<string, List<string>>();

        PlayerPictureValidator.ValidateFileSize(-1, errors);

        Assert.Contains("FileSizeBytes", errors.Keys);
    }
}
