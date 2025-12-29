# Player Picture Services - Developer Guide

This guide provides technical documentation for developers working with the Player Picture Upload feature in the GhcSamplePs application.

## Architecture Overview

The Player Picture Upload feature follows clean architecture principles with strict separation between UI and business logic:

```
GhcSamplePs.Web (UI Layer)
    ↓ calls
GhcSamplePs.Core (Business Logic Layer)
    ├── Services (IBlobStorageService, IPlayerService)
    ├── Validation (PlayerPictureValidator)
    └── Models (DTOs: UploadPlayerPictureDto, UploadPlayerPictureResultDto)
    ↓ stores in
Azure Blob Storage (player-pictures container)
```

### Design Principles

1. **UI-Agnostic Core**: All business logic is in the Core project with no UI dependencies
2. **Service Layer Pattern**: Business logic encapsulated in services with interfaces
3. **Repository Pattern**: Data access abstracted through repositories
4. **Validation First**: All inputs validated before processing
5. **Async/Await**: All I/O operations are asynchronous
6. **ServiceResult Pattern**: Consistent result handling with success/failure states

## Components

### 1. BlobStorageService

**Location:** `src/GhcSamplePs.Core/Services/Implementations/BlobStorageService.cs`

**Purpose:** Handles all Azure Blob Storage operations for player pictures.

#### Interface: IBlobStorageService

```csharp
public interface IBlobStorageService
{
    Task<ServiceResult<(string Url, string BlobName)>> UploadPlayerPictureAsync(
        byte[] fileContent,
        string fileName,
        string contentType,
        int playerId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> DeletePlayerPictureAsync(
        string blobName,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<string>> GetPictureUrlWithSasAsync(
        string blobName,
        int expirationMinutes = 60,
        CancellationToken cancellationToken = default);

    string GeneratePlayerBlobName(int playerId, string extension);
}
```

#### Key Methods

**UploadPlayerPictureAsync**
- Uploads image bytes to Azure Blob Storage
- Generates unique blob name: `player-{playerId}-{timestamp}.{extension}`
- Returns blob URL and blob name
- Validates file content, size, and format
- Logs all operations and errors

**DeletePlayerPictureAsync**
- Deletes a blob from Azure Storage
- Succeeds even if blob doesn't exist (idempotent)
- Logs deletion operations

**GetPictureUrlWithSasAsync**
- Generates time-limited SAS URL for secure access
- Default expiration: 60 minutes
- Returns full URL with SAS token

**GeneratePlayerBlobName**
- Creates unique blob name for a player's picture
- Format: `player-{playerId}-{timestamp}.{extension}`
- Ensures uniqueness with timestamp

#### Configuration

Required configuration in `appsettings.json`:

```json
{
  "AzureStorage": {
    "ConnectionString": "Your Azure Storage connection string",
    "PlayerPicturesContainer": "player-pictures",
    "SasExpirationMinutes": 60,
    "MaxUploadSizeBytes": 5242880
  }
}
```

#### Error Handling

The service catches and wraps Azure Storage exceptions:

| Azure Exception | ServiceResult Error |
|----------------|---------------------|
| RequestFailedException (404) | "Blob not found" |
| RequestFailedException (403) | "Access denied to storage" |
| RequestFailedException (5xx) | "Storage service temporarily unavailable" |
| ArgumentException | Validation error message |
| Generic Exception | "An unexpected error occurred" |

#### Dependencies

```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.x" />
<PackageReference Include="Azure.Identity" Version="1.x" />
```

### 2. PlayerService (Picture Methods)

**Location:** `src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs`

**Purpose:** Orchestrates picture upload/delete operations with authorization and player record updates.

#### Interface Methods

```csharp
public interface IPlayerService
{
    Task<ServiceResult<UploadPlayerPictureResultDto>> UploadPlayerPictureAsync(
        UploadPlayerPictureDto uploadDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> DeletePlayerPictureAsync(
        int playerId,
        string currentUserId,
        CancellationToken cancellationToken = default);
}
```

#### UploadPlayerPictureAsync Flow

1. **Validate Input**: Check uploadDto and currentUserId are not null
2. **Validate File**: Use PlayerPictureValidator to validate file size, format, content type
3. **Retrieve Player**: Get player from repository
4. **Authorize**: Verify currentUserId matches player.UserId (only owner can upload)
5. **Delete Old Picture**: If player already has a picture, delete it from blob storage (best effort)
6. **Upload New Picture**: Call BlobStorageService to upload to Azure
7. **Update Player**: Set player.PhotoUrl and PhotoBlobName properties
8. **Save Changes**: Call repository.UpdateAsync to persist changes
9. **Return Result**: Return UploadPlayerPictureResultDto with success and URL

#### DeletePlayerPictureAsync Flow

1. **Validate Input**: Check playerId > 0 and currentUserId is not null
2. **Retrieve Player**: Get player from repository
3. **Authorize**: Verify currentUserId matches player.UserId (only owner can delete)
4. **Check Picture Exists**: If no picture, return success (idempotent)
5. **Delete from Storage**: Call BlobStorageService to delete blob
6. **Update Player**: Clear player.PhotoUrl and PhotoBlobName properties
7. **Save Changes**: Call repository.UpdateAsync to persist changes
8. **Return Success**: Return ServiceResult indicating success

#### Authorization Rules

- **Upload**: User must be the owner of the player record (UserId match)
- **Delete**: User must be the owner of the player record (UserId match)
- **View**: No authorization check (URLs use SAS tokens for security)

#### Error Scenarios

| Scenario | Result |
|----------|--------|
| Player not found | ServiceResult.Failure("Player not found") |
| Unauthorized user | ServiceResult.Failure("You do not have permission...") |
| Validation failure | ServiceResult.Failure with ValidationErrors dictionary |
| Blob upload fails | ServiceResult.Failure("Failed to upload picture...") |
| Repository fails | ServiceResult.Failure("Failed to update player record") |

### 3. PlayerPictureValidator

**Location:** `src/GhcSamplePs.Core/Validation/PlayerPictureValidator.cs`

**Purpose:** Validates picture upload data according to business rules.

#### Validation Rules

```csharp
public static ValidationResult ValidateUpload(UploadPlayerPictureDto uploadDto)
{
    // 1. PlayerId: Must be > 0
    // 2. FileContent: Must not be null or empty
    // 3. FileName: Must not be null/empty, must have extension
    // 4. ContentType: Must be image/jpeg, image/png, image/gif, or image/webp
    // 5. FileSizeBytes: Must not exceed MaxUploadSizeBytes (5 MB)
    // 6. Extension matches ContentType
}
```

#### Validation Error Messages

| Rule | Error Message |
|------|--------------|
| Invalid PlayerId | "Player ID must be greater than 0" |
| Empty file content | "The uploaded file appears to be empty" |
| File too large | "The file size of {size} MB exceeds the maximum allowed size of 5 MB" |
| No extension | "File must have an extension" |
| Invalid content type | "Invalid content type. Allowed types are: image/jpeg, image/png, image/gif, image/webp" |
| Unsupported extension | "Unsupported file extension: {ext}" |

#### Constants

```csharp
public const long MaxUploadSizeBytes = 5 * 1024 * 1024; // 5 MB

private static readonly HashSet<string> AllowedContentTypes = new()
{
    "image/jpeg",
    "image/png",
    "image/gif",
    "image/webp"
};

private static readonly Dictionary<string, string> ContentTypeExtensions = new()
{
    { "image/jpeg", ".jpg" },
    { "image/png", ".png" },
    { "image/gif", ".gif" },
    { "image/webp", ".webp" }
};
```

### 4. Data Transfer Objects

**Location:** `src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/`

#### UploadPlayerPictureDto

```csharp
public class UploadPlayerPictureDto
{
    public int PlayerId { get; set; }
    public byte[] FileContent { get; set; } = [];
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
```

**Purpose:** Transfer picture upload data from UI to service layer.

**Usage:**
```csharp
var uploadDto = new UploadPlayerPictureDto
{
    PlayerId = 123,
    FileContent = await ReadFileAsync(file),
    FileName = file.FileName,
    ContentType = file.ContentType,
    FileSizeBytes = file.Length
};

var result = await playerService.UploadPlayerPictureAsync(uploadDto, currentUserId);
```

#### UploadPlayerPictureResultDto

```csharp
public class UploadPlayerPictureResultDto
{
    public bool Success { get; set; }
    public string? PictureUrl { get; set; }
    public string? ErrorMessage { get; set; }
}
```

**Purpose:** Return upload result with URL or error to UI.

**Usage:**
```csharp
if (result.Success && result.Data != null)
{
    var pictureUrl = result.Data.PictureUrl;
    // Display picture at pictureUrl
}
else if (result.Data != null)
{
    var errorMessage = result.Data.ErrorMessage;
    // Show error to user
}
```

### 5. Player Entity Updates

**Location:** `src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs`

#### Properties Added

```csharp
public class Player
{
    // Existing properties...
    
    [MaxLength(500)]
    public string? PhotoUrl { get; set; }
    
    [MaxLength(255)]
    public string? PhotoBlobName { get; set; }
}
```

**PhotoUrl**: Full URL to access the picture (with SAS token when needed)  
**PhotoBlobName**: Blob storage name for management operations (e.g., deletion)

#### Methods

```csharp
public bool HasPicture() => !string.IsNullOrWhiteSpace(PhotoUrl);

public void UpdatePicture(string blobUrl, string blobName, string userId)
{
    PhotoUrl = blobUrl;
    PhotoBlobName = blobName;
    UpdatedBy = userId;
    UpdatedAt = DateTime.UtcNow;
}

public void RemovePicture(string userId)
{
    PhotoUrl = null;
    PhotoBlobName = null;
    UpdatedBy = userId;
    UpdatedAt = DateTime.UtcNow;
}
```

## Testing

### Unit Tests

**Location:** `tests/GhcSamplePs.Core.Tests/`

#### Test Coverage

| Test Suite | Tests | Coverage |
|-----------|-------|----------|
| BlobStorageServiceTests | 21 | Blob operations, validation, error handling |
| PlayerServicePictureTests | 18 | Upload/delete workflows, authorization |
| PlayerPictureValidatorTests | 18 | All validation rules and edge cases |
| **Total** | **57** | **Comprehensive coverage** |

#### Key Test Scenarios

**BlobStorageServiceTests:**
- ✅ Generate unique blob names
- ✅ Validate file size and format
- ✅ Handle Azure Storage exceptions
- ✅ Generate SAS URLs with correct expiration
- ✅ Delete blobs (existing and non-existing)

**PlayerServicePictureTests:**
- ✅ Upload picture with valid data
- ✅ Replace existing picture
- ✅ Delete picture successfully
- ✅ Reject unauthorized upload attempts
- ✅ Handle player not found
- ✅ Handle blob storage failures
- ✅ Validate input parameters

**PlayerPictureValidatorTests:**
- ✅ Validate all supported formats (JPEG, PNG, GIF, WebP)
- ✅ Reject oversized files (>5 MB)
- ✅ Reject invalid content types
- ✅ Reject mismatched extensions
- ✅ Validate file name requirements

#### Running Tests

```bash
# Run all picture-related tests
dotnet test --filter "FullyQualifiedName~Picture"

# Run specific test class
dotnet test --filter "FullyQualifiedName~BlobStorageServiceTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Mocking Strategy

**IBlobStorageService** is mocked in PlayerService tests:

```csharp
var mockBlobStorage = new Mock<IBlobStorageService>();

mockBlobStorage
    .Setup(b => b.UploadPlayerPictureAsync(
        It.IsAny<byte[]>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<int>(),
        It.IsAny<CancellationToken>()))
    .ReturnsAsync(ServiceResult<(string, string)>.Success(
        ("https://blob.url/test.jpg", "player-1-test.jpg")));
```

**IPlayerRepository** is mocked for all service tests:

```csharp
var mockRepository = new Mock<IPlayerRepository>();

mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(player);
```

## Integration with Azure

### Azure Blob Storage Configuration

#### Container Setup

**Container Name:** `player-pictures`  
**Public Access:** None (Private)  
**Purpose:** Store all player profile pictures

#### Blob Naming Convention

Format: `player-{playerId}-{timestamp}.{extension}`

Examples:
- `player-123-20241229143045.jpg`
- `player-456-20241229150230.png`
- `player-789-20241229162145.webp`

#### SAS Token Configuration

- **Permissions:** Read only
- **Expiration:** 60 minutes (configurable)
- **Protocol:** HTTPS only
- **IP Restriction:** None (accessible from any IP)

#### Security Best Practices

1. **Private Container**: Never enable public blob access
2. **SAS Tokens**: Always use time-limited SAS tokens for access
3. **Managed Identity**: Use Azure Managed Identity in production (avoid connection strings)
4. **CORS**: Configure CORS rules if accessing from client-side JavaScript
5. **Lifecycle Policies**: Implement policies to delete old orphaned blobs

### Infrastructure as Code

**Location:** `infra/modules/storage.bicep`

The player-pictures container is provisioned via Bicep:

```bicep
resource playerPicturesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: 'player-pictures'
  properties: {
    publicAccess: 'None'
  }
}
```

## Extending the Feature

### Adding New Validation Rules

To add new validation rules, edit `PlayerPictureValidator.cs`:

```csharp
public static ValidationResult ValidateUpload(UploadPlayerPictureDto uploadDto)
{
    var result = new ValidationResult();

    // Existing validations...

    // New validation: Check image dimensions
    if (!ValidateImageDimensions(uploadDto.FileContent))
    {
        result.AddError(nameof(uploadDto.FileContent), 
            "Image dimensions must be at least 300x300 pixels");
    }

    return result;
}

private static bool ValidateImageDimensions(byte[] fileContent)
{
    // Implement dimension check using System.Drawing or ImageSharp
    return true;
}
```

### Supporting Additional Formats

To add support for new image formats:

1. Update `AllowedContentTypes` in `PlayerPictureValidator.cs`
2. Update `ContentTypeExtensions` dictionary
3. Add validation tests for the new format
4. Update user documentation

Example:
```csharp
private static readonly HashSet<string> AllowedContentTypes = new()
{
    "image/jpeg",
    "image/png",
    "image/gif",
    "image/webp",
    "image/svg+xml" // New format
};

private static readonly Dictionary<string, string> ContentTypeExtensions = new()
{
    { "image/jpeg", ".jpg" },
    { "image/png", ".png" },
    { "image/gif", ".gif" },
    { "image/webp", ".webp" },
    { "image/svg+xml", ".svg" } // New format
};
```

### Implementing Thumbnail Generation

To generate thumbnails for list views:

1. Create `IThumbnailService` interface in `Services/Interfaces/`
2. Implement thumbnail generation using ImageSharp or System.Drawing
3. Store thumbnails in a separate container: `player-picture-thumbnails`
4. Update `BlobStorageService` to generate thumbnails on upload

Example interface:
```csharp
public interface IThumbnailService
{
    Task<byte[]> GenerateThumbnailAsync(
        byte[] originalImage,
        int width,
        int height,
        CancellationToken cancellationToken = default);
}
```

### Adding Image Optimization

To automatically optimize uploaded images:

1. Install ImageSharp NuGet package
2. Create `IImageOptimizationService` interface
3. Implement compression and format conversion
4. Call optimization service before uploading to blob storage

Example:
```csharp
public interface IImageOptimizationService
{
    Task<byte[]> OptimizeImageAsync(
        byte[] originalImage,
        string contentType,
        int maxWidth = 1024,
        int maxHeight = 1024,
        int quality = 85,
        CancellationToken cancellationToken = default);
}
```

## Performance Considerations

### Upload Performance

- **Target**: < 5 seconds for files up to 5 MB
- **Factors**: Network speed, file size, Azure region proximity
- **Optimization**: Use Azure CDN for frequently accessed images

### Display Performance

- **Target**: < 1 second to load and display picture
- **Implementation**: SAS URLs enable direct browser access to Azure Storage
- **Caching**: Browser caches images based on URL (cache busting via timestamp in blob name)

### Scalability

- **Concurrent Uploads**: Azure Blob Storage handles multiple simultaneous uploads
- **Storage Capacity**: Virtually unlimited (Azure scales automatically)
- **Cost**: Pay per GB stored and per transaction

## Security Audit Checklist

- [x] **Authentication**: Only authenticated users can upload/delete
- [x] **Authorization**: Only owner can modify player's picture
- [x] **Private Storage**: Container access is private (not public)
- [x] **SAS Tokens**: Time-limited tokens (60 minutes)
- [x] **HTTPS Only**: All uploads and downloads use HTTPS
- [x] **Input Validation**: File size, format, and content type validated
- [x] **Error Messages**: No sensitive information leaked in errors
- [x] **Audit Logging**: All operations logged with user ID
- [x] **SQL Injection**: Parameterized queries via EF Core
- [x] **XSS Prevention**: URLs properly encoded in UI

## Troubleshooting

### Common Issues

**Issue:** "Blob Storage connection error"  
**Cause:** Invalid connection string or network issue  
**Solution:** Verify `AzureStorage:ConnectionString` in configuration

---

**Issue:** "Request failed with status code 403 (Forbidden)"  
**Cause:** Insufficient permissions on storage account  
**Solution:** Ensure the connection string has Blob Data Contributor role

---

**Issue:** "SAS token expired"  
**Cause:** Generated SAS token has expired  
**Solution:** Increase `SasExpirationMinutes` in configuration or regenerate URL

---

**Issue:** "Old picture not deleted during replacement"  
**Cause:** Deletion failure is logged but doesn't block upload  
**Solution:** Check logs and manually clean up orphaned blobs if needed

### Debugging Tips

1. **Enable Detailed Logging**: Set log level to `Debug` in development
2. **Check Azure Portal**: Verify blobs in Azure Storage Explorer
3. **Test with Azurite**: Use Azure Storage Emulator locally
4. **Review Application Insights**: Check telemetry for errors and performance

## Additional Resources

### Internal Documentation

- [Player Picture Upload Specification](../specs/PlayerPictureUpload_Feature_Specification.md)
- [Blob Storage Integration Specification](../specs/Blob_Storage_Integration_Specification.md)
- [Player Picture Upload User Guide](Player_Picture_Upload_User_Guide.md)
- [Core Project README](../../src/GhcSamplePs.Core/README.md)

### External Resources

- [Azure Blob Storage .NET SDK](https://docs.microsoft.com/azure/storage/blobs/storage-quickstart-blobs-dotnet)
- [SAS Tokens Best Practices](https://docs.microsoft.com/azure/storage/common/storage-sas-overview)
- [Azure Storage Security Guide](https://docs.microsoft.com/azure/storage/common/storage-security-guide)
- [Clean Architecture in .NET](https://docs.microsoft.com/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

**Last Updated:** December 29, 2024  
**Version:** 1.0.0  
**Maintainer:** Development Team
