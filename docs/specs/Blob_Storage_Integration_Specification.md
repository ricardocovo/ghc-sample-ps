# Blob Storage Integration Specification

**Version**: 1.0 | **Date**: December 24, 2025 | **Status**: Implemented

---

## Overview

This specification details the integration of Azure Blob Storage for player picture management in the GhcSamplePs application. The implementation provides a secure, scalable, and maintainable solution for storing and retrieving player profile pictures.

## Architecture

### Components

#### 1. BlobStorageService
**Location**: `src/GhcSamplePs.Core/Services/Implementations/BlobStorageService.cs`

**Responsibilities**:
- Upload player pictures to Azure Blob Storage
- Delete player pictures from storage
- Generate time-limited SAS URLs for secure picture access
- Generate unique blob names to prevent conflicts
- Ensure blob container exists at runtime

**Key Methods**:
- `UploadPlayerPictureAsync` - Uploads picture and returns blob URL
- `DeletePlayerPictureAsync` - Deletes picture from storage
- `GetPictureUrlWithSasAsync` - Generates SAS URL with expiration
- `GeneratePlayerBlobName` - Creates unique blob names (format: `player-{id}-{timestamp}.{ext}`)
- `EnsureContainerExistsAsync` - Ensures container exists, creates if needed

#### 2. BlobStorageInitializationService
**Location**: `src/GhcSamplePs.Core/Services/Implementations/BlobStorageInitializationService.cs`

**Purpose**: IHostedService that ensures blob containers exist when the application starts.

**Behavior**:
- Runs during application startup (before accepting requests)
- Calls `EnsureContainerExistsAsync` to create container if needed
- Logs success/failure but doesn't prevent application startup
- Gracefully handles missing configuration (logs warning)

#### 3. PlayerService Extensions
**Location**: `src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs`

**New Methods**:
- `UploadPlayerPictureAsync` - Coordinates picture upload with player record update
- `DeletePlayerPictureAsync` - Coordinates picture deletion with player record update

**Business Logic**:
- Validates user authorization (user must own the player)
- Validates file using PlayerPictureValidator
- Handles replacement of existing pictures (deletes old, uploads new)
- Updates Player.PhotoUrl with blob URL
- Maintains audit trail (UpdatedBy, UpdatedAt)

## Container Initialization

### Startup Process

1. **Application Startup**
   - `BlobStorageInitializationService` starts as IHostedService
   - Service is registered in DI container with `AddHostedService`

2. **Container Creation**
   - Service calls `IBlobStorageService.EnsureContainerExistsAsync()`
   - Azure SDK method `CreateIfNotExistsAsync` is invoked
   - Container created with `PublicAccessType.None` (private)
   - Operation is idempotent (safe to call multiple times)

3. **Error Handling**
   - Logs warning if blob storage not configured
   - Logs error if container creation fails
   - Application continues startup even if initialization fails
   - Upload operations will fail gracefully if container doesn't exist

### Configuration

Required configuration in `appsettings.json`:

```json
{
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "PlayerPicturesContainer": "player-pictures",
    "SasExpirationMinutes": 60,
    "MaxUploadSizeBytes": 5242880
  }
}
```

**Configuration Keys**:
- `AzureStorage:ConnectionString` - Azure Storage connection string (required)
- `AzureStorage:PlayerPicturesContainer` - Container name (default: "player-pictures")
- `AzureStorage:SasExpirationMinutes` - SAS token expiration (default: 60 minutes)
- `AzureStorage:MaxUploadSizeBytes` - Maximum file size (5 MB = 5,242,880 bytes)

### Dependency Injection Registration

**Core Services** (in `Program.cs` of Web project):

```csharp
// Register Blob Storage Service
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

// Register Container Initialization Service
builder.Services.AddHostedService<BlobStorageInitializationService>();
```

**Note**: BlobStorageService is optional in PlayerService constructor to support scenarios where blob storage is not configured.

## Validation

### PlayerPictureValidator

**Location**: `src/GhcSamplePs.Core/Validation/PlayerPictureValidator.cs`

**Validation Rules**:

| Rule | Constraint | Error Message |
|------|-----------|---------------|
| File Size | Must not exceed 5 MB (5,242,880 bytes) | "File size exceeds the maximum allowed size of 5 MB." |
| Content Type | Must be image/jpeg, image/png, image/gif, or image/webp | "Unsupported content type '{type}'. Supported types: ..." |
| File Extension | Must be .jpg, .jpeg, .png, .gif, or .webp | "Unsupported file extension '{ext}'. Supported extensions: ..." |
| Extension Match | File extension must match content type | "File extension '{ext}' does not match content type '{type}'." |
| Player ID | Must be greater than 0 | "Player ID must be greater than 0." |
| File Content | Must not be empty | "File content cannot be empty." |

**Example**:
```csharp
var uploadDto = new UploadPlayerPictureDto
{
    PlayerId = 1,
    FileContent = imageBytes,
    FileName = "player.jpg",
    ContentType = "image/jpeg",
    FileSizeBytes = imageBytes.Length
};

var result = PlayerPictureValidator.ValidateUpload(uploadDto);
if (!result.IsValid)
{
    // Handle validation errors
}
```

## Security

### Access Control

1. **Container Access**
   - Container created with `PublicAccessType.None` (private)
   - No anonymous public read access
   - All access requires authentication

2. **User Authorization**
   - Users can only upload/delete pictures for players they own
   - Authorization checked via `Player.UserId == currentUserId`
   - Enforced in PlayerService before blob operations

3. **SAS Tokens**
   - Generated with read-only permissions
   - Time-limited (default: 60 minutes)
   - Includes 5-minute clock skew buffer
   - Format: `https://account.blob.core.windows.net/container/blob?sas-token`

### Data Protection

- **In Transit**: HTTPS enforced for all blob operations
- **At Rest**: Azure Storage encryption (automatic)
- **Audit Trail**: All operations logged with user ID, player ID, timestamps

## Error Handling

### Exception Types

| Exception | Cause | Handling |
|-----------|-------|----------|
| `RequestFailedException` | Azure Storage API errors | Wrap in ServiceResult.Fail(), log details |
| `InvalidOperationException` | Missing configuration | Throw with helpful message |
| `ArgumentException` | Invalid parameters | Throw immediately (parameter validation) |
| `ArgumentNullException` | Null required parameters | Throw immediately (parameter validation) |

### Error Messages

**User-Facing**:
- "Picture upload is not available. Blob storage service is not configured."
- "Failed to upload picture to storage. Please try again."
- "Picture deleted successfully."
- "You do not have permission to upload a picture for this player."

**Technical Logging**:
- All exceptions logged with full context (player ID, user ID, blob name)
- Azure error codes captured for troubleshooting
- Stack traces preserved for debugging

## Testing

### Unit Tests

**Test Coverage**: 53 tests covering:

1. **BlobStorageService Tests** (21 tests)
   - Constructor validation
   - GeneratePlayerBlobName logic and uniqueness
   - Parameter validation for all methods
   - Error handling scenarios

2. **PlayerPictureValidator Tests** (19 tests)
   - File size validation (under, at, over limit)
   - Content type validation (all supported types)
   - File extension validation and matching
   - Edge cases (empty files, no extension, etc.)

3. **PlayerService Picture Tests** (13 tests)
   - Upload success and failure scenarios
   - Authorization checks
   - Picture replacement logic
   - Delete success and failure scenarios
   - Blob storage not configured handling

**Test Commands**:
```bash
# Run all picture-related tests
dotnet test --filter "FullyQualifiedName~PlayerPicture"

# Run with coverage
dotnet test --filter "FullyQualifiedName~PlayerPicture" --collect:"XPlat Code Coverage"
```

## Deployment

### Azure Infrastructure

**Bicep Module**: `infra/modules/storage.bicep`

**Required Resources**:
1. **Storage Account**
   - SKU: Standard_LRS (or higher for production)
   - Access tier: Hot
   - Minimum TLS version: 1.2
   - HTTPS required: true

2. **Blob Container**
   - Name: `player-pictures`
   - Public access: None (private)
   - Created automatically by application

**Connection Methods**:
- **Development**: Connection string from appsettings
- **Production**: Managed Identity (recommended) or connection string from Key Vault

### Environment Variables

**Development** (`appsettings.Development.json`):
```json
{
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true"
  }
}
```

**Production** (Azure Container App environment variables):
- `AzureStorage__ConnectionString` - From Key Vault or Managed Identity
- `AzureStorage__PlayerPicturesContainer` - "player-pictures"

## Monitoring

### Logging Events

| Log Level | Event | Message Pattern |
|-----------|-------|----------------|
| Information | Container initialized | "Blob container '{ContainerName}' already exists" |
| Information | Picture uploaded | "Successfully uploaded player picture. PlayerId: {PlayerId}, BlobName: {BlobName}" |
| Information | Picture deleted | "Successfully deleted player picture. BlobName: {BlobName}" |
| Warning | Blob not found on delete | "Blob not found when attempting to delete. BlobName: {BlobName}" |
| Warning | Storage not configured | "Blob storage service is not configured. Skipping container initialization" |
| Error | Azure Storage error | "Azure Storage error while uploading player picture. Error: {ErrorCode}" |

### Metrics to Monitor

- **Upload Success Rate**: Percentage of successful uploads
- **Upload Duration**: Time to complete upload operation
- **Storage Usage**: Total size of player-pictures container
- **SAS Token Generation**: Frequency and success rate
- **Container Initialization**: Success/failure on startup

## Performance Considerations

### Optimization Strategies

1. **Blob Naming**
   - Timestamp-based names ensure uniqueness
   - No need for existence checks before upload
   - Format: `player-{id}-{yyyyMMddHHmmss}.{ext}`

2. **Upload Process**
   - Stream directly to blob storage (no temporary files)
   - Use `MemoryStream` for in-memory processing
   - Set Content-Type header for proper browser handling

3. **SAS Tokens**
   - Generated on-demand (not cached)
   - Short expiration (60 minutes) balances security and UX
   - Client can refresh token if needed

4. **Container Initialization**
   - Idempotent `CreateIfNotExistsAsync` call
   - Runs once at startup (not per request)
   - Non-blocking (doesn't prevent app startup)

### Scalability

- **Concurrent Uploads**: No locking, each upload independent
- **Storage Limits**: Azure Blob Storage scales to exabytes
- **Throughput**: Limited by Azure Storage Account limits (see Azure docs)
- **Geographic Distribution**: Use Azure CDN for global access (future enhancement)

## Future Enhancements

### Planned Improvements

1. **Image Processing**
   - Automatic thumbnail generation
   - Image compression to reduce storage costs
   - Format conversion (e.g., convert to WebP)

2. **CDN Integration**
   - Serve pictures through Azure CDN
   - Reduced latency for global users
   - Lower blob storage egress costs

3. **Content Moderation**
   - Azure Content Moderator integration
   - Automatic scanning for inappropriate content
   - Flagging system for manual review

4. **Blob Lifecycle Management**
   - Automatic deletion of orphaned blobs
   - Archival of old pictures to cool/archive tier
   - Scheduled cleanup jobs

5. **Enhanced Security**
   - Managed Identity authentication (instead of connection strings)
   - Azure Key Vault integration for connection strings
   - Customer-managed encryption keys

## Troubleshooting

### Common Issues

**Issue**: Container initialization fails
- **Cause**: Invalid connection string or insufficient permissions
- **Solution**: Verify connection string, check storage account access

**Issue**: Upload fails with "Container not found"
- **Cause**: Container initialization failed at startup
- **Solution**: Check application logs, manually create container, restart app

**Issue**: Pictures not displaying
- **Cause**: Expired SAS token or incorrect URL
- **Solution**: Verify SAS expiration, check blob URL format

**Issue**: "Blob storage service is not configured"
- **Cause**: IBlobStorageService not registered in DI or null
- **Solution**: Register service in Program.cs, check configuration

### Debug Commands

```bash
# Check if container exists (Azure CLI)
az storage container show --name player-pictures --connection-string "..."

# List blobs in container
az storage blob list --container-name player-pictures --connection-string "..."

# Upload test file
az storage blob upload --container-name player-pictures --file test.jpg --name test.jpg --connection-string "..."
```

## References

- **Azure Blob Storage Documentation**: https://learn.microsoft.com/azure/storage/blobs/
- **Azure Storage .NET SDK**: https://learn.microsoft.com/dotnet/api/overview/azure/storage.blobs-readme
- **SAS Tokens**: https://learn.microsoft.com/azure/storage/common/storage-sas-overview
- **IHostedService**: https://learn.microsoft.com/aspnet/core/fundamentals/host/hosted-services

---

**Document History**:
- v1.0 (2025-12-24): Initial specification based on implementation

**Related Documents**:
- `PlayerPictureUpload_Feature_Specification.md` - Overall feature specification
- `blazor-architecture.instructions.md` - Architecture guidelines
- `csharp.instructions.md` - C# coding standards
