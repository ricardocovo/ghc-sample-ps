using GhcSamplePs.Core.Common;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Service interface for Azure Blob Storage operations related to player pictures.
/// Handles upload, deletion, and secure URL generation for pictures stored in Azure Blob Storage.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Uploads a player picture to Azure Blob Storage.
    /// </summary>
    /// <param name="fileContent">The binary content of the image file.</param>
    /// <param name="fileName">The original name of the file.</param>
    /// <param name="contentType">The MIME content type of the file (e.g., "image/jpeg").</param>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the blob URL and blob name if successful, or error details if failed.</returns>
    /// <example>
    /// <code>
    /// var result = await blobStorageService.UploadPlayerPictureAsync(
    ///     fileContent: imageBytes,
    ///     fileName: "player-photo.jpg",
    ///     contentType: "image/jpeg",
    ///     playerId: 123
    /// );
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Uploaded to: {result.Data!.BlobUrl}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<(string BlobUrl, string BlobName)>> UploadPlayerPictureAsync(
        byte[] fileContent,
        string fileName,
        string contentType,
        int playerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player picture from Azure Blob Storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result indicating success or failure.</returns>
    /// <example>
    /// <code>
    /// var result = await blobStorageService.DeletePlayerPictureAsync("player-123-20251224.jpg");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine("Picture deleted successfully");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<bool>> DeletePlayerPictureAsync(
        string blobName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a time-limited SAS URL for secure access to a player picture.
    /// </summary>
    /// <param name="blobName">The name of the blob.</param>
    /// <param name="expirationMinutes">The number of minutes until the SAS token expires.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the SAS URL if successful.</returns>
    /// <example>
    /// <code>
    /// var result = await blobStorageService.GetPictureUrlWithSasAsync("player-123-20251224.jpg", 60);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Access URL: {result.Data}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<string>> GetPictureUrlWithSasAsync(
        string blobName,
        int expirationMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique blob name for a player picture.
    /// </summary>
    /// <param name="playerId">The unique identifier of the player.</param>
    /// <param name="fileExtension">The file extension (e.g., ".jpg").</param>
    /// <returns>A unique blob name for the player picture.</returns>
    /// <example>
    /// <code>
    /// var blobName = blobStorageService.GeneratePlayerBlobName(123, ".jpg");
    /// // Returns: "player-123-20251224161530.jpg"
    /// </code>
    /// </example>
    string GeneratePlayerBlobName(int playerId, string fileExtension);

    /// <summary>
    /// Ensures that the player pictures blob container exists.
    /// Creates the container if it doesn't exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result indicating success or failure.</returns>
    /// <example>
    /// <code>
    /// var result = await blobStorageService.EnsureContainerExistsAsync();
    /// if (result.Success)
    /// {
    ///     Console.WriteLine("Container is ready");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<bool>> EnsureContainerExistsAsync(CancellationToken cancellationToken = default);
}
