using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service implementation for Azure Blob Storage operations related to player pictures.
/// Handles upload, deletion, and secure URL generation for pictures stored in Azure Blob Storage.
/// </summary>
public sealed class BlobStorageService : IBlobStorageService
{
    private readonly ILogger<BlobStorageService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _containerName;
    private readonly int _sasExpirationMinutes;
    private readonly Lazy<BlobServiceClient> _blobServiceClient;

    public BlobStorageService(
        ILogger<BlobStorageService> logger,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);

        _logger = logger;
        _configuration = configuration;
        _containerName = _configuration["AzureStorage:PlayerPicturesContainer"] ?? "player-pictures";
        _sasExpirationMinutes = int.TryParse(_configuration["AzureStorage:SasExpirationMinutes"], out var minutes) ? minutes : 60;
        _blobServiceClient = new Lazy<BlobServiceClient>(CreateBlobServiceClient);
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<(string BlobUrl, string BlobName)>> UploadPlayerPictureAsync(
        byte[] fileContent,
        string fileName,
        string contentType,
        int playerId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileContent);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        if (playerId <= 0)
        {
            return ServiceResult<(string, string)>.Fail("Player ID must be greater than 0.");
        }

        if (fileContent.Length == 0)
        {
            return ServiceResult<(string, string)>.Fail("File content cannot be empty.");
        }

        try
        {
            var blobServiceClient = GetBlobServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var fileExtension = Path.GetExtension(fileName);
            var blobName = GeneratePlayerBlobName(playerId, fileExtension);
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = new MemoryStream(fileContent);
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                },
                cancellationToken);

            var blobUrl = blobClient.Uri.ToString();

            _logger.LogInformation(
                "Successfully uploaded player picture. PlayerId: {PlayerId}, BlobName: {BlobName}, Size: {Size} bytes",
                playerId, blobName, fileContent.Length);

            return ServiceResult<(string, string)>.Ok((blobUrl, blobName));
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex,
                "Azure Storage error while uploading player picture. PlayerId: {PlayerId}, Error: {ErrorCode}",
                playerId, ex.ErrorCode);
            return ServiceResult<(string, string)>.Fail($"Failed to upload picture to storage: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while uploading player picture. PlayerId: {PlayerId}",
                playerId);
            return ServiceResult<(string, string)>.Fail("An unexpected error occurred while uploading the picture.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<bool>> DeletePlayerPictureAsync(
        string blobName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        try
        {
            var blobServiceClient = GetBlobServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (response.Value)
            {
                _logger.LogInformation("Successfully deleted player picture. BlobName: {BlobName}", blobName);
            }
            else
            {
                _logger.LogWarning("Blob not found when attempting to delete. BlobName: {BlobName}", blobName);
            }

            return ServiceResult<bool>.Ok(true);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex,
                "Azure Storage error while deleting player picture. BlobName: {BlobName}, Error: {ErrorCode}",
                blobName, ex.ErrorCode);
            return ServiceResult<bool>.Fail($"Failed to delete picture from storage: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while deleting player picture. BlobName: {BlobName}",
                blobName);
            return ServiceResult<bool>.Fail("An unexpected error occurred while deleting the picture.");
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<string>> GetPictureUrlWithSasAsync(
        string blobName,
        int expirationMinutes = 60,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        if (expirationMinutes <= 0)
        {
            return ServiceResult<string>.Fail("Expiration minutes must be greater than 0.");
        }

        try
        {
            var blobServiceClient = GetBlobServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var exists = await blobClient.ExistsAsync(cancellationToken);
            if (!exists.Value)
            {
                return ServiceResult<string>.Fail($"Blob '{blobName}' does not exist.");
            }

            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUri = blobClient.GenerateSasUri(sasBuilder);

                _logger.LogDebug(
                    "Generated SAS URL for blob. BlobName: {BlobName}, ExpiresIn: {Minutes} minutes",
                    blobName, expirationMinutes);

                return ServiceResult<string>.Ok(sasUri.ToString());
            }

            return ServiceResult<string>.Ok(blobClient.Uri.ToString());
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex,
                "Azure Storage error while generating SAS URL. BlobName: {BlobName}, Error: {ErrorCode}",
                blobName, ex.ErrorCode);
            return ServiceResult<string>.Fail($"Failed to generate picture URL: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while generating SAS URL. BlobName: {BlobName}",
                blobName);
            return ServiceResult<string>.Fail("An unexpected error occurred while generating the picture URL.");
        }
    }

    /// <inheritdoc/>
    public string GeneratePlayerBlobName(int playerId, string fileExtension)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileExtension);

        if (playerId <= 0)
        {
            throw new ArgumentException("Player ID must be greater than 0.", nameof(playerId));
        }

        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var normalizedExtension = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}";

        return $"player-{playerId}-{timestamp}{normalizedExtension}";
    }

    /// <inheritdoc/>
    public async Task<ServiceResult<bool>> EnsureContainerExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var blobServiceClient = GetBlobServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            var response = await containerClient.CreateIfNotExistsAsync(
                publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.None,
                cancellationToken: cancellationToken);

            if (response is not null && response.Value is not null)
            {
                _logger.LogInformation(
                    "Created blob container '{ContainerName}' successfully",
                    _containerName);
            }
            else
            {
                _logger.LogInformation(
                    "Blob container '{ContainerName}' already exists",
                    _containerName);
            }

            return ServiceResult<bool>.Ok(true);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex,
                "Azure Storage error while ensuring container exists. Container: {ContainerName}, Error: {ErrorCode}",
                _containerName, ex.ErrorCode);
            return ServiceResult<bool>.Fail($"Failed to ensure container exists: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while ensuring container exists. Container: {ContainerName}",
                _containerName);
            return ServiceResult<bool>.Fail("An unexpected error occurred while ensuring container exists.");
        }
    }

    private BlobServiceClient CreateBlobServiceClient()
    {
        var connectionString = _configuration["AzureStorage:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Storage connection string is not configured. " +
                "Please set 'AzureStorage:ConnectionString' in application configuration.");
        }

        return new BlobServiceClient(connectionString);
    }

    private BlobServiceClient GetBlobServiceClient() => _blobServiceClient.Value;
}
