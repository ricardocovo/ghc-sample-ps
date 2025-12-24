using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Background service that initializes Azure Blob Storage containers on application startup.
/// Ensures that required blob containers exist before the application starts accepting requests.
/// </summary>
public sealed class BlobStorageInitializationService : IHostedService
{
    private readonly IBlobStorageService? _blobStorageService;
    private readonly ILogger<BlobStorageInitializationService> _logger;

    public BlobStorageInitializationService(
        IBlobStorageService? blobStorageService,
        ILogger<BlobStorageInitializationService> logger)
    {
        _blobStorageService = blobStorageService;
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <summary>
    /// Initializes blob storage containers when the application starts.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_blobStorageService is null)
        {
            _logger.LogWarning("Blob storage service is not configured. Skipping container initialization");
            return;
        }

        _logger.LogInformation("Initializing Azure Blob Storage containers...");

        try
        {
            var result = await _blobStorageService.EnsureContainerExistsAsync(cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation("Azure Blob Storage containers initialized successfully");
            }
            else
            {
                _logger.LogWarning(
                    "Failed to initialize Azure Blob Storage containers: {Errors}",
                    string.Join(", ", result.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Azure Blob Storage containers");
        }
    }

    /// <summary>
    /// Cleanup method called when the application stops.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Blob storage initialization service stopping");
        return Task.CompletedTask;
    }
}
