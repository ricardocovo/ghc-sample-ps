using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for uploading a player picture.
/// Contains the image file data and metadata required for upload validation and storage.
/// </summary>
public sealed record UploadPlayerPictureDto
{
    /// <summary>
    /// Gets the unique identifier of the player for whom the picture is being uploaded.
    /// </summary>
    [Required(ErrorMessage = "Player ID is required.")]
    public required int PlayerId { get; init; }

    /// <summary>
    /// Gets the binary content of the image file.
    /// </summary>
    [Required(ErrorMessage = "File content is required.")]
    public required byte[] FileContent { get; init; }

    /// <summary>
    /// Gets the original name of the uploaded file.
    /// </summary>
    [Required(ErrorMessage = "File name is required.")]
    [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the MIME content type of the file (e.g., "image/jpeg", "image/png").
    /// </summary>
    [Required(ErrorMessage = "Content type is required.")]
    [StringLength(100, ErrorMessage = "Content type cannot exceed 100 characters.")]
    public required string ContentType { get; init; }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    [Range(1, long.MaxValue, ErrorMessage = "File size must be greater than 0.")]
    public required long FileSizeBytes { get; init; }
}
