namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object representing the result of a player picture upload operation.
/// Contains the outcome of the upload including success status, picture URL, or error details.
/// </summary>
public sealed record UploadPlayerPictureResultDto
{
    /// <summary>
    /// Gets a value indicating whether the upload was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets the URL to access the uploaded picture.
    /// Will be null if the upload failed.
    /// </summary>
    public string? PictureUrl { get; init; }

    /// <summary>
    /// Gets the name of the blob in storage.
    /// Will be null if the upload failed.
    /// </summary>
    public string? BlobName { get; init; }

    /// <summary>
    /// Gets the error message if the upload failed.
    /// Will be null if the upload was successful.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Creates a successful upload result.
    /// </summary>
    /// <param name="pictureUrl">The URL to access the uploaded picture.</param>
    /// <param name="blobName">The name of the blob in storage.</param>
    /// <returns>A successful <see cref="UploadPlayerPictureResultDto"/>.</returns>
    public static UploadPlayerPictureResultDto CreateSuccess(string pictureUrl, string blobName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pictureUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        return new UploadPlayerPictureResultDto
        {
            Success = true,
            PictureUrl = pictureUrl,
            BlobName = blobName,
            ErrorMessage = null
        };
    }

    /// <summary>
    /// Creates a failed upload result.
    /// </summary>
    /// <param name="errorMessage">The error message describing why the upload failed.</param>
    /// <returns>A failed <see cref="UploadPlayerPictureResultDto"/>.</returns>
    public static UploadPlayerPictureResultDto CreateFailure(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        return new UploadPlayerPictureResultDto
        {
            Success = false,
            PictureUrl = null,
            BlobName = null,
            ErrorMessage = errorMessage
        };
    }
}
