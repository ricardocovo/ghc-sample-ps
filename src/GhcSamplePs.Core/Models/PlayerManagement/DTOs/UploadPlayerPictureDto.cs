namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for uploading a player picture.
/// Contains the image file data and metadata required for upload validation and storage.
/// Validation is performed by PlayerPictureValidator in the service layer.
/// </summary>
public sealed record UploadPlayerPictureDto
{
    /// <summary>
    /// Gets the unique identifier of the player for whom the picture is being uploaded.
    /// </summary>
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the binary content of the image file.
    /// </summary>
    public required byte[] FileContent { get; init; }

    /// <summary>
    /// Gets the original name of the uploaded file.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the MIME content type of the file (e.g., "image/jpeg", "image/png").
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    public required long FileSizeBytes { get; init; }
}
