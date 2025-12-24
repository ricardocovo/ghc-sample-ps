using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Validation;

/// <summary>
/// Provides validation logic for player picture uploads, enforcing file size, format, and content type rules.
/// </summary>
/// <remarks>
/// <para>Validation Rules:</para>
/// <list type="bullet">
///   <item><description><b>File Size:</b> Must not exceed 5 MB (5,242,880 bytes)</description></item>
///   <item><description><b>File Format:</b> Must be JPEG, PNG, GIF, or WebP based on content type</description></item>
///   <item><description><b>Content Type:</b> Must match: image/jpeg, image/png, image/gif, image/webp</description></item>
///   <item><description><b>File Extension:</b> Must match content type to prevent mismatches</description></item>
///   <item><description><b>File Content:</b> Must not be empty (size > 0)</description></item>
/// </list>
/// </remarks>
public static class PlayerPictureValidator
{
    /// <summary>
    /// Maximum allowed file size for picture uploads in bytes (5 MB).
    /// </summary>
    public const long MaxFileSizeBytes = 5_242_880; // 5 MB

    /// <summary>
    /// Supported image content types for player pictures.
    /// </summary>
    public static readonly string[] SupportedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    ];

    /// <summary>
    /// Supported image file extensions for player pictures.
    /// </summary>
    public static readonly string[] SupportedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    ];

    /// <summary>
    /// Validates a picture upload DTO against all business rules.
    /// </summary>
    /// <param name="uploadDto">The upload DTO to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when uploadDto is null.</exception>
    public static ValidationResult ValidateUpload(UploadPlayerPictureDto uploadDto)
    {
        ArgumentNullException.ThrowIfNull(uploadDto);

        var errors = new Dictionary<string, List<string>>();

        ValidatePlayerId(uploadDto.PlayerId, errors);
        ValidateFileContent(uploadDto.FileContent, errors);
        ValidateFileSize(uploadDto.FileSizeBytes, errors);
        ValidateFileName(uploadDto.FileName, errors);
        ValidateContentType(uploadDto.ContentType, errors);
        ValidateFileExtension(uploadDto.FileName, uploadDto.ContentType, errors);

        if (errors.Count > 0)
        {
            var errorDict = errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
            return ValidationResult.Invalid(errorDict);
        }

        return ValidationResult.Valid();
    }

    /// <summary>
    /// Validates that the player ID is positive.
    /// </summary>
    public static void ValidatePlayerId(int playerId, Dictionary<string, List<string>> errors)
    {
        if (playerId <= 0)
        {
            AddError(errors, nameof(UploadPlayerPictureDto.PlayerId), "Player ID must be greater than 0.");
        }
    }

    /// <summary>
    /// Validates that the file content is not null or empty.
    /// </summary>
    public static void ValidateFileContent(byte[] fileContent, Dictionary<string, List<string>> errors)
    {
        if (fileContent is null || fileContent.Length == 0)
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileContent), "File content cannot be empty.");
        }
    }

    /// <summary>
    /// Validates that the file size does not exceed the maximum allowed size.
    /// </summary>
    public static void ValidateFileSize(long fileSizeBytes, Dictionary<string, List<string>> errors)
    {
        if (fileSizeBytes <= 0)
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileSizeBytes), "File size must be greater than 0.");
        }
        else if (fileSizeBytes > MaxFileSizeBytes)
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileSizeBytes),
                $"File size exceeds the maximum allowed size of {MaxFileSizeBytes / 1_048_576} MB.");
        }
    }

    /// <summary>
    /// Validates that the file name is not null or empty.
    /// </summary>
    public static void ValidateFileName(string fileName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileName), "File name is required.");
        }
    }

    /// <summary>
    /// Validates that the content type is supported.
    /// </summary>
    public static void ValidateContentType(string contentType, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.ContentType), "Content type is required.");
            return;
        }

        if (!IsValidImageContentType(contentType))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.ContentType),
                $"Unsupported content type '{contentType}'. Supported types: {string.Join(", ", SupportedContentTypes)}.");
        }
    }

    /// <summary>
    /// Validates that the file extension matches the content type.
    /// </summary>
    public static void ValidateFileExtension(string fileName, string contentType, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(extension))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileName), "File must have a valid extension.");
            return;
        }

        if (!SupportedExtensions.Contains(extension))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileName),
                $"Unsupported file extension '{extension}'. Supported extensions: {string.Join(", ", SupportedExtensions)}.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(contentType) && !DoesExtensionMatchContentType(extension, contentType))
        {
            AddError(errors, nameof(UploadPlayerPictureDto.FileName),
                $"File extension '{extension}' does not match content type '{contentType}'.");
        }
    }

    /// <summary>
    /// Checks if the content type is a supported image format.
    /// </summary>
    /// <param name="contentType">The MIME content type to check.</param>
    /// <returns>True if the content type is supported; otherwise, false.</returns>
    public static bool IsValidImageContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return SupportedContentTypes.Contains(contentType.ToLowerInvariant());
    }

    /// <summary>
    /// Checks if the file extension matches the content type.
    /// </summary>
    private static bool DoesExtensionMatchContentType(string extension, string contentType)
    {
        var normalizedExtension = extension.ToLowerInvariant();
        var normalizedContentType = contentType.ToLowerInvariant();

        return normalizedContentType switch
        {
            "image/jpeg" => normalizedExtension is ".jpg" or ".jpeg",
            "image/png" => normalizedExtension == ".png",
            "image/gif" => normalizedExtension == ".gif",
            "image/webp" => normalizedExtension == ".webp",
            _ => false
        };
    }

    private static void AddError(Dictionary<string, List<string>> errors, string fieldName, string errorMessage)
    {
        if (!errors.ContainsKey(fieldName))
        {
            errors[fieldName] = [];
        }
        errors[fieldName].Add(errorMessage);
    }
}
