# Feature Specification: Player Picture Upload and Management

**Version**: 1.0 MVP | **Date**: December 23, 2025

---

## Executive Summary

### Brief Description

This feature enables users to upload, display, and manage player profile pictures using Azure Blob Storage. Users can upload images directly from their device or paste an image URL, view the picture on the Edit Player screen, and delete/re-upload pictures as needed.

### Business Value

- **Enhanced Player Identification**: Profile pictures help quickly identify players in lists and team rosters
- **Professional Appearance**: Provides a more polished, modern user experience
- **Data Management**: Centralized storage in Azure ensures reliability, scalability, and security
- **User Control**: Players can update their pictures at any time, keeping profiles current

### Key Stakeholders

- **Primary Users**: Coaches, Team Administrators, Parents
- **Secondary Users**: Players (viewing their own profiles)
- **Technical Owner**: Development Team
- **Business Owner**: Product Owner
- **Infrastructure**: Azure Blob Storage service

---

## Requirements

### Functional Requirements

#### FR1: Upload Player Picture

- **Upload Methods**:
  - File upload from device (browse and select)
  - Direct image URL paste/entry
- **Supported Formats**: JPEG, PNG, GIF, WebP
- **File Size Limit**: Maximum 5 MB per image
- **Validation**:
  - Verify file type before upload
  - Validate file size before upload
  - Display clear error messages for invalid files
- **Upload Feedback**:
  - Show progress indicator during upload
  - Display success confirmation with preview
  - Show error details if upload fails

#### FR2: Display Player Picture

- **Edit Player Screen**:
  - Display current picture in dedicated section at top of Player Information tab
  - Show placeholder/avatar icon when no picture is set
  - Display picture in circular or rounded square container (consistent with wireframe)
- **List View Enhancement** (Future Phase):
  - Small thumbnail in player list
  - Avatar/initials when no picture available

#### FR3: Delete and Re-upload Picture

- **Delete Functionality**:
  - Display delete button when picture exists
  - Show confirmation dialog before deletion
  - Remove picture from both Azure Storage and database
  - Revert to placeholder/avatar after deletion
- **Re-upload Functionality**:
  - Allow immediate re-upload after deletion
  - Replace existing picture without requiring deletion first (overwrite)

#### FR4: Picture Storage and Retrieval

- **Azure Blob Storage**:
  - Store pictures in dedicated container: `player-pictures`
  - Use consistent naming convention: `player-{playerId}-{timestamp}.{extension}`
  - Generate SAS URLs with time-limited access for secure retrieval
  - Clean up orphaned files during player deletion
- **Database Reference**:
  - Update Player entity with blob storage reference/URL
  - Store blob name and container reference for management

#### FR5: Validation and Error Handling

- **Client-Side Validation**:
  - Check file size before upload (max 5 MB)
  - Verify file type/extension
  - Validate URL format if using URL method
- **Server-Side Validation**:
  - Re-validate file size and type
  - Check for malicious content (basic validation)
  - Verify blob storage availability
  - Handle upload failures gracefully
- **Error Messages**:
  - "File size exceeds 5 MB limit"
  - "Invalid file format. Please upload JPEG, PNG, GIF, or WebP images"
  - "Upload failed. Please try again"
  - "Invalid URL format"

### Non-Functional Requirements

#### NFR1: Performance

- **Upload Speed**: Complete upload within 5 seconds for files up to 5 MB (dependent on network)
- **Display Speed**: Load and display picture within 1 second
- **Blob Storage Access**: Use CDN or caching where appropriate for frequently accessed images
- **Thumbnail Generation** (Future): Generate and cache optimized thumbnails for list views

#### NFR2: Security

- **Access Control**:
  - Only authenticated users can upload/delete pictures
  - Users can only manage pictures for players they own (authorization check via UserId)
- **Storage Security**:
  - Use SAS tokens with short expiration for blob access (e.g., 1 hour)
  - Block public anonymous access to blob container
  - Validate file content, not just extension (prevent malicious uploads)
- **URL Security**:
  - Sanitize and validate externally provided URLs
  - Consider proxy for external URLs to prevent SSRF attacks

#### NFR3: Scalability

- **Storage Capacity**: Azure Blob Storage scales automatically
- **Concurrent Uploads**: Support multiple users uploading simultaneously
- **Blob Naming**: Use unique identifiers (player ID + timestamp) to avoid collisions

#### NFR4: Maintainability

- **Clean Architecture**: Follow existing separation between Core and Web layers
- **Service Abstraction**: Create `IBlobStorageService` interface for storage operations
- **Configuration**: Store Azure Storage connection strings and container names in configuration
- **Logging**: Log all upload, delete, and error events for troubleshooting

#### NFR5: Usability

- **Intuitive Interface**: Clear upload area with drag-and-drop support (if feasible)
- **Visual Feedback**: Show loading spinners, progress indicators, success/error messages
- **Responsive Design**: Work seamlessly on desktop, tablet, and mobile devices
- **Accessibility**: Ensure alt text and keyboard navigation support

#### NFR6: Reliability

- **Retry Logic**: Implement automatic retry for transient Azure Storage failures
- **Error Recovery**: Allow users to retry failed uploads without losing form data
- **Orphan Cleanup**: Scheduled job to remove orphaned blobs (pictures without associated players)

### User Stories

**US1**: As a coach, I want to upload a player's picture so that I can easily identify them in the roster.

**US2**: As a parent, I want to update my child's profile picture when they get a new team photo.

**US3**: As a team administrator, I want to delete an outdated player picture and upload a new one.

**US4**: As a user, I want to see a preview of the picture immediately after uploading to confirm it uploaded correctly.

**US5**: As a user, I want clear error messages if my upload fails so I know what went wrong and how to fix it.

### Acceptance Criteria

- ✅ User can upload a picture from their device (JPEG, PNG, GIF, WebP up to 5 MB)
- ✅ User can paste/enter an image URL and have it saved
- ✅ Picture displays in Edit Player screen immediately after upload
- ✅ User can delete existing picture with confirmation dialog
- ✅ User can re-upload/replace picture without deleting first
- ✅ File size and format validation works on client and server
- ✅ Clear error messages display for all validation failures
- ✅ Pictures are stored securely in Azure Blob Storage
- ✅ Only authorized users can upload/delete pictures
- ✅ Orphaned pictures are cleaned up when player is deleted
- ✅ Loading states and progress indicators work correctly
- ✅ All business logic is in Core project (UI-agnostic)
- ✅ Unit tests cover all Core services (85%+ coverage)

---

## Technical Design

### Architecture Impact

**Components Affected**:
- **GhcSamplePs.Core**: New blob storage service, updated Player entity/DTOs, new validation rules
- **GhcSamplePs.Web**: Updated EditPlayer.razor component, new upload UI components
- **Infrastructure**: New blob storage container in Azure, updated Bicep configuration

**New Components Required**:
- Core: `IBlobStorageService` and implementation
- Core: `PlayerPictureValidator` for upload validation
- Web: Picture upload UI section in EditPlayer.razor
- Web: Optional reusable `PictureUploadComponent.razor`
- Infra: Updated `storage.bicep` module

**Data Flow**:
1. User selects picture file or enters URL in UI
2. Client validates file size and format
3. Blazor component calls PlayerService with picture data
4. PlayerService calls BlobStorageService to upload to Azure
5. BlobStorageService returns blob URL/reference
6. PlayerService updates Player entity with picture reference
7. Repository persists updated Player to database
8. UI displays uploaded picture

### Implementation Details

#### Data Layer

**Player Entity Updates** (reference: [src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs](src/GhcSamplePs.Core/Models/PlayerManagement/Player.cs))

The existing `Player` entity already has a `PhotoUrl` property (string, nullable, max 500 chars). This will be repurposed to store the blob storage reference:

**Updated Property Purpose**:
- `PhotoUrl`: Will store either a full Azure Blob Storage URL (with SAS token when needed) or a relative blob reference (container/blob-name)

**No schema changes required** - existing property is sufficient.

**New Properties to Consider** (optional, for enhanced management):
- `PhotoBlobName`: The blob file name in storage (e.g., "player-123-20251223.jpg")
- `PhotoContainerName`: The container name (e.g., "player-pictures")
- `PhotoUploadedAt`: Timestamp of last picture upload

**Navigation Properties**: None required for this feature.

**Business Logic Methods** (add to Player entity):
- `UpdatePicture(blobUrl, blobName, userId)`: Updates picture reference and audit fields
- `RemovePicture(userId)`: Clears picture reference and updates audit fields
- `HasPicture()`: Returns true if PhotoUrl is not null/empty

---

**Player DTOs Updates**

**PlayerDto** (reference: [src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/PlayerDto.cs](src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/PlayerDto.cs))

Already includes `PhotoUrl` property - no changes required.

**CreatePlayerDto** (reference: [src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/CreatePlayerDto.cs](src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/CreatePlayerDto.cs))

Already includes `PhotoUrl` property - can be left as is for URL-based uploads, or make nullable/optional since upload will typically happen after player creation.

**UpdatePlayerDto** (reference: [src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/UpdatePlayerDto.cs](src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/UpdatePlayerDto.cs))

Already includes `PhotoUrl` property - no changes required.

**New DTOs Required**:

**UploadPlayerPictureDto**:
- `PlayerId`: The player's ID (int, required)
- `FileContent`: Byte array of the image file (byte[], required for file upload)
- `FileName`: Original file name (string, required)
- `ContentType`: MIME type (string, required, e.g., "image/jpeg")
- `FileSizeBytes`: Size in bytes (long, required)

Purpose: Transfer picture upload data from UI to service layer.

**UploadPlayerPictureResultDto**:
- `Success`: Boolean indicating success
- `PictureUrl`: The URL to access the uploaded picture (string, nullable)
- `ErrorMessage`: Error details if upload failed (string, nullable)

Purpose: Return upload result to UI with URL or error information.

---

**Database Schema**

Existing `Players` table already has `PhotoUrl` column:
- `PhotoUrl` (nvarchar(500), nullable)

**No migration required** unless adding optional tracking columns:
- `PhotoBlobName` (nvarchar(255), nullable)
- `PhotoContainerName` (nvarchar(100), nullable)
- `PhotoUploadedAt` (datetime2, nullable)

**Indexes**: No new indexes required for this feature.

---

#### Business Logic Layer

**New Service: IBlobStorageService** (create in [src/GhcSamplePs.Core/Services/Interfaces/](src/GhcSamplePs.Core/Services/Interfaces/))

**Interface Definition**:

| Method | Description | Parameters | Return Type |
|--------|-------------|------------|-------------|
| UploadPlayerPictureAsync | Uploads a player picture to blob storage | file content, file name, content type, player ID, cancellation token | ServiceResult with blob URL |
| DeletePlayerPictureAsync | Deletes a player picture from blob storage | blob name or player ID, cancellation token | ServiceResult indicating success |
| GetPictureUrlWithSasAsync | Generates a time-limited SAS URL for picture access | blob name, expiration duration, cancellation token | ServiceResult with SAS URL |
| GeneratePlayerBlobNameAsync | Creates a unique blob name for a player picture | player ID, file extension | string blob name |

**Responsibilities**:
- Handle all Azure Blob Storage interactions
- Generate unique blob names (e.g., "player-{id}-{timestamp}.jpg")
- Create SAS tokens for secure access
- Validate file content and size
- Handle Azure Storage exceptions
- Log all operations

**Implementation Location**: [src/GhcSamplePs.Core/Services/Implementations/BlobStorageService.cs](src/GhcSamplePs.Core/Services/Implementations/)

**Dependencies**:
- Azure.Storage.Blobs NuGet package
- ILogger for logging
- IConfiguration for connection strings and container names

**Error Handling**:
- Wrap Azure exceptions in domain-specific exceptions
- Return ServiceResult with clear error messages
- Log all exceptions with context

---

**Updated Service: IPlayerService** (reference: [src/GhcSamplePs.Core/Services/Interfaces/IPlayerService.cs](src/GhcSamplePs.Core/Services/Interfaces/IPlayerService.cs))

**New Methods to Add**:

| Method | Description | Parameters | Return Type |
|--------|-------------|------------|-------------|
| UploadPlayerPictureAsync | Uploads picture and updates player record | player ID, upload DTO, current user ID, cancellation token | ServiceResult<UploadPlayerPictureResultDto> |
| DeletePlayerPictureAsync | Deletes picture from storage and player record | player ID, current user ID, cancellation token | ServiceResult indicating success |
| UpdatePlayerPictureUrlAsync | Updates player with externally hosted picture URL | player ID, picture URL, current user ID, cancellation token | ServiceResult<PlayerDto> |

**Business Logic**:
1. **UploadPlayerPictureAsync**:
   - Validate user authorization (user owns the player)
   - Validate file size and format using PlayerPictureValidator
   - Call BlobStorageService to upload file
   - Update Player entity with blob URL
   - Save changes via repository
   - Return updated player with picture URL

2. **DeletePlayerPictureAsync**:
   - Validate user authorization
   - Retrieve player's current picture reference
   - Call BlobStorageService to delete blob
   - Update Player entity to remove picture reference
   - Save changes via repository
   - Return success result

3. **UpdatePlayerPictureUrlAsync**:
   - Validate user authorization
   - Validate URL format
   - Update Player entity with URL (for external images)
   - Save changes via repository
   - Return updated player

**Implementation Location**: [src/GhcSamplePs.Core/Services/Implementations/PlayerService.cs](src/GhcSamplePs.Core/Services/Implementations/)

---

**New Validator: PlayerPictureValidator** (create in [src/GhcSamplePs.Core/Validation/](src/GhcSamplePs.Core/Validation/))

**Validation Rules**:

| Rule | Description |
|------|-------------|
| File Size | Must not exceed 5 MB (5,242,880 bytes) |
| File Format | Must be JPEG, PNG, GIF, or WebP based on content type |
| Content Type | Must match: image/jpeg, image/png, image/gif, image/webp |
| File Extension | Must match content type (prevent mismatches) |
| File Content | Optional: Validate file header bytes to ensure actual image format |
| Player Exists | Player ID must reference existing player |
| User Authorization | Current user must own the player (UserId match) |

**Methods**:
- `ValidateUpload(uploadDto, currentUserId)`: Returns validation result with errors if any
- `ValidateFileSize(sizeBytes)`: Checks file size limit
- `ValidateFileFormat(contentType, fileName)`: Checks format and extension
- `IsValidImageContentType(contentType)`: Returns true for supported types

**Implementation Location**: [src/GhcSamplePs.Core/Validation/PlayerPictureValidator.cs](src/GhcSamplePs.Core/Validation/)

---

#### API/Interface Layer

This feature primarily involves service layer changes as the existing architecture uses Blazor components directly calling services via dependency injection. No separate API endpoints are required unless exposing this functionality via REST API in the future.

**If API Endpoints Were Required** (future consideration):

| Method | Path | Description | Request Body | Response |
|--------|------|-------------|--------------|----------|
| POST | /api/players/{id}/picture | Upload player picture | Multipart form data with file | 200 OK with picture URL or 400/500 error |
| DELETE | /api/players/{id}/picture | Delete player picture | None | 204 No Content or error |
| PUT | /api/players/{id}/picture-url | Update with external URL | JSON with URL property | 200 OK with updated player or error |

---

#### UI/Presentation Layer

**EditPlayer.razor Component** (reference: [src/GhcSamplePs.Web/Components/Pages/PlayerManagement/EditPlayer.razor](src/GhcSamplePs.Web/Components/Pages/PlayerManagement/EditPlayer.razor))

**New UI Section to Add** (in Player Information Tab):

**Location**: Add picture upload section at the top of the Player Information tab, before the name field.

**UI Elements**:

1. **Picture Display Area**:
   - Circular or rounded square container (150x150px or similar)
   - Display current picture if available
   - Display placeholder avatar icon if no picture (use MudBlazor Icons.Material.Filled.Person)
   - Overlay hover effect with "Change Picture" text

2. **Upload Controls**:
   - **Upload Button**: "Upload Picture" or "Change Picture" (MudButton with file input)
   - **URL Input Option**: Text field for pasting image URL (collapsible/expandable)
   - **Delete Button**: Red delete button (visible only when picture exists)
   - File input (hidden, triggered by button click)

3. **Feedback Elements**:
   - Progress bar or spinner during upload (MudProgressLinear or MudProgressCircular)
   - Success message (MudAlert, Severity.Success, auto-dismiss after 3 seconds)
   - Error message (MudAlert, Severity.Error, with details)

4. **Layout Structure** (described in natural language):
   - Use MudStack for vertical layout
   - Center picture and controls within MudPaper or MudCard
   - Icons from MudBlazor: Icons.Material.Filled.CloudUpload, Icons.Material.Filled.Delete, Icons.Material.Filled.Link

**User Interaction Flow**:
1. User clicks "Upload Picture" button
2. File browser opens, user selects image file
3. Client validates file size and format
4. If valid, upload begins with progress indicator
5. On success, picture displays immediately with success message
6. On failure, error message displays with retry option
7. User can delete picture via delete button (with confirmation dialog)
8. User can paste URL in URL field and click "Use URL" button

**Blazor Component Code Patterns to Follow**:
- Use `@inject IPlayerService` for service injection
- Use `InputFile` component for file uploads (MudFileUpload wrapper)
- Use `OnChange` event handlers for file selection
- Use `async Task` methods for upload/delete operations
- Use `_isUploading`, `_uploadError`, `_uploadSuccess` fields for state management
- Use MudBlazor dialogs for delete confirmation (reference existing delete patterns in EditPlayer.razor)

**Reference Existing Patterns**:
- Button styling and layout: See action buttons at bottom of Player Information tab
- Loading states: See `_isSaving`, `_isDeleting` patterns in EditPlayer
- Error display: See `_errorMessage` pattern with MudAlert
- Dialog confirmation: See `HandleDelete` method for confirmation pattern

---

**Optional: Reusable Component** (future enhancement)

**PictureUploadComponent.razor** (create in [src/GhcSamplePs.Web/Components/Shared/](src/GhcSamplePs.Web/Components/Shared/))

**Purpose**: Reusable picture upload component for other entities (teams, coaches, etc.).

**Parameters**:
- `CurrentPictureUrl`: URL of current picture (string, nullable)
- `OnUpload`: Callback when upload completes (EventCallback)
- `OnDelete`: Callback when delete is clicked (EventCallback)
- `MaxSizeBytes`: Maximum file size (long, default 5 MB)
- `AllowUrl`: Enable URL input option (bool, default true)
- `ShowDeleteButton`: Show delete button (bool, default true)

---

#### Code Conventions to Follow

Reference project instruction files:
- [.github/instructions/blazor-architecture.instructions.md](.github/instructions/blazor-architecture.instructions.md): Maintain clean separation between Core and Web
- [.github/instructions/csharp.instructions.md](.github/instructions/csharp.instructions.md): C# 14 features, nullable reference types, XML documentation
- [.github/instructions/dotnet-architecture-good-practices.instructions.md](.github/instructions/dotnet-architecture-good-practices.instructions.md): DDD patterns, SOLID principles

**Specific Conventions**:
- **Naming**:
  - Service interface: `IBlobStorageService`
  - Implementation: `BlobStorageService`
  - Validator: `PlayerPictureValidator`
  - DTOs: `UploadPlayerPictureDto`, `UploadPlayerPictureResultDto`
- **File Organization**:
  - Services: [src/GhcSamplePs.Core/Services/Interfaces/](src/GhcSamplePs.Core/Services/Interfaces/) and [Implementations/](src/GhcSamplePs.Core/Services/Implementations/)
  - DTOs: [src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/](src/GhcSamplePs.Core/Models/PlayerManagement/DTOs/)
  - Validation: [src/GhcSamplePs.Core/Validation/](src/GhcSamplePs.Core/Validation/)
- **Error Handling**: Use `ServiceResult<T>` pattern (reference existing service methods)
- **Logging**: Use `ILogger<T>` for structured logging
- **Async**: All I/O operations must be async with CancellationToken support

---

#### Dependencies

**NuGet Packages**:

| Package | Version | Purpose | Target Project |
|---------|---------|---------|----------------|
| Azure.Storage.Blobs | Latest (12.x) | Azure Blob Storage client | GhcSamplePs.Core |
| Azure.Identity | Latest (1.x) | Azure authentication (if using Managed Identity) | GhcSamplePs.Core |

**Existing Dependencies** (already in project):
- MudBlazor: UI components
- Entity Framework Core: Database access
- Microsoft.AspNetCore.Components.Forms: File upload components

**Azure Resources**:
- Azure Storage Account (already exists - reference: [infra/modules/storage.bicep](infra/modules/storage.bicep))
- New Blob Container: `player-pictures`

---

#### Configuration Requirements

**appsettings.json** (reference: [src/GhcSamplePs.Web/appsettings.json](src/GhcSamplePs.Web/appsettings.json))

Add new section:

```
"AzureStorage": {
  "ConnectionString": "",
  "PlayerPicturesContainer": "player-pictures",
  "SasExpirationMinutes": 60,
  "MaxUploadSizeBytes": 5242880
}
```

**Environment Variables** (for Azure deployment):
- `AzureStorage__ConnectionString`: Storage account connection string (or use Managed Identity)
- `AzureStorage__PlayerPicturesContainer`: Container name (default: "player-pictures")

**Azure Key Vault** (production):
- Store connection string in Key Vault
- Reference via configuration: `@Microsoft.KeyVault(SecretUri=...)`

---

#### Security Considerations

**Authentication & Authorization**:
- **Requirement**: User must be authenticated to upload/delete pictures
- **Check**: Verify user owns the player (UserId match) in PlayerService before operations
- **Implementation**: Use `ICurrentUserProvider` (existing service) to get current user ID

**File Validation**:
- **Client-Side**: Validate file size and extension before upload
- **Server-Side**: Re-validate file size, content type, and optionally file content (header bytes)
- **Purpose**: Prevent malicious file uploads and oversized files

**Blob Storage Security**:
- **Private Container**: Set blob container public access to "None" (private)
- **SAS Tokens**: Generate time-limited SAS tokens for read access (e.g., 1 hour expiration)
- **Managed Identity**: Use Azure Managed Identity for authentication in production (avoid connection strings)
- **CORS**: Configure CORS rules if accessing blobs from client-side JavaScript

**URL Validation**:
- **External URLs**: If allowing external image URLs, validate URL format and consider using a proxy
- **SSRF Prevention**: Do not directly fetch external URLs from server without validation

**Data Privacy**:
- **Player Pictures**: Ensure pictures are only accessible to authorized users
- **Audit Trail**: Log all picture uploads/deletes with user ID and timestamp

---

#### Error Handling

**Expected Exceptions**:

| Exception Type | Cause | Handling Strategy |
|----------------|-------|-------------------|
| RequestFailedException (Azure) | Azure Storage errors (network, quota, etc.) | Wrap in domain exception, log details, return user-friendly message |
| InvalidOperationException | Player not found, unauthorized access | Return error in ServiceResult, display in UI |
| ArgumentException | Invalid file format, size, or URL | Return validation errors in ServiceResult |
| IOException | File read errors | Log error, return generic error message to user |

**Error Messages** (user-facing):

| Scenario | Message |
|----------|---------|
| File too large | "The selected file exceeds the 5 MB size limit. Please choose a smaller image." |
| Invalid format | "Invalid file format. Please upload a JPEG, PNG, GIF, or WebP image." |
| Upload failed | "Failed to upload picture. Please try again. If the problem persists, contact support." |
| Unauthorized | "You do not have permission to modify this player's picture." |
| Network error | "Upload failed due to network error. Please check your connection and try again." |
| Storage quota exceeded | "Unable to upload picture due to storage limitations. Please contact support." |

**Logging Strategy**:
- **Information**: Successful uploads/deletes with user ID, player ID, file size
- **Warning**: Validation failures, unauthorized attempts
- **Error**: Azure Storage exceptions, unexpected errors with full stack trace

---

## Testing Strategy

### Unit Tests

**Location**: [tests/GhcSamplePs.Core.Tests/](tests/GhcSamplePs.Core.Tests/)

**Test Classes**:

1. **BlobStorageServiceTests.cs**
   - `UploadPlayerPictureAsync_ValidFile_ReturnsSuccessWithUrl`
   - `UploadPlayerPictureAsync_FileExceedsSizeLimit_ReturnsError`
   - `UploadPlayerPictureAsync_AzureStorageException_ReturnsError`
   - `DeletePlayerPictureAsync_ExistingBlob_ReturnsSuccess`
   - `DeletePlayerPictureAsync_BlobNotFound_ReturnsSuccess`
   - `GetPictureUrlWithSasAsync_ValidBlob_ReturnsSasUrl`
   - `GeneratePlayerBlobNameAsync_ValidPlayerId_ReturnsUniqueName`

2. **PlayerServiceTests.cs** (add to existing test file)
   - `UploadPlayerPictureAsync_ValidUpload_UpdatesPlayerAndReturnsUrl`
   - `UploadPlayerPictureAsync_UnauthorizedUser_ReturnsError`
   - `UploadPlayerPictureAsync_PlayerNotFound_ReturnsError`
   - `UploadPlayerPictureAsync_BlobUploadFails_ReturnsError`
   - `DeletePlayerPictureAsync_ExistingPicture_DeletesBlobAndUpdatesPlayer`
   - `DeletePlayerPictureAsync_UnauthorizedUser_ReturnsError`
   - `UpdatePlayerPictureUrlAsync_ValidUrl_UpdatesPlayer`
   - `UpdatePlayerPictureUrlAsync_InvalidUrl_ReturnsError`

3. **PlayerPictureValidatorTests.cs**
   - `ValidateUpload_ValidFile_ReturnsSuccess`
   - `ValidateUpload_FileTooLarge_ReturnsError`
   - `ValidateUpload_InvalidFormat_ReturnsError`
   - `ValidateUpload_MismatchedExtension_ReturnsError`
   - `ValidateFileSize_ExceedsLimit_ReturnsFalse`
   - `ValidateFileFormat_UnsupportedType_ReturnsFalse`
   - `IsValidImageContentType_SupportedTypes_ReturnsTrue`

**Mocking Strategy**:
- Mock `ILogger<T>` for all tests
- Mock Azure Blob Storage client (BlobContainerClient, BlobClient) using interfaces
- Mock `IPlayerRepository` in PlayerService tests
- Mock `IBlobStorageService` in PlayerService tests
- Use test data fixtures for sample file content and DTOs

**Code Coverage Target**: 85% minimum for all new Core services and validators

---

### Integration Tests

**Scenarios** (manual or automated):

1. **End-to-End Upload Flow**:
   - User uploads valid 2 MB JPEG file
   - File uploads to Azure Blob Storage
   - Player record updates with blob URL
   - Picture displays in UI

2. **File Validation**:
   - Attempt to upload 6 MB file → Error displayed
   - Attempt to upload .txt file → Error displayed
   - Attempt to upload .exe renamed to .jpg → Error displayed

3. **Delete Flow**:
   - User deletes existing picture
   - Confirmation dialog appears
   - Picture removed from Azure and database
   - Placeholder displays in UI

4. **Authorization**:
   - User A attempts to upload picture for User B's player → Error
   - User A attempts to delete User B's player picture → Error

5. **Error Handling**:
   - Simulate Azure Storage unavailability → User-friendly error message
   - Simulate network timeout during upload → Retry option available

**Test Environment**:
- Use Azure Storage Emulator (Azurite) for local testing
- Use test storage account for integration testing
- Clean up test blobs after tests complete

---

### UI/UX Testing

**Test Cases**:

| Test Case | Expected Behavior |
|-----------|-------------------|
| No picture exists | Placeholder avatar displays, "Upload Picture" button visible |
| Upload valid file | Progress indicator shows, picture displays after upload, success message appears |
| Upload oversized file | Error message displays immediately, upload does not start |
| Upload invalid format | Error message displays, upload does not start |
| Delete picture | Confirmation dialog appears, picture removed after confirmation |
| Cancel delete | Picture remains unchanged |
| Upload during another operation | Upload button disabled, cannot start upload |
| Mobile device upload | File picker opens, upload works smoothly |

**Browser Testing**: Test on Chrome, Firefox, Safari, Edge

**Device Testing**: Test on desktop, tablet, mobile (iOS and Android)

---

## Implementation Phases

### Phase 1: MVP (Core Functionality)

**Scope**:
- Upload picture from device (file selection)
- Display picture in Edit Player screen
- Delete picture with confirmation
- Store pictures in Azure Blob Storage
- Basic validation (size and format)
- Unit tests for Core services

**Timeline**: 2-3 sprints

**Deliverables**:
- `IBlobStorageService` and implementation
- Updated `PlayerService` with upload/delete methods
- `PlayerPictureValidator`
- Updated `EditPlayer.razor` with picture upload UI
- Unit tests (85%+ coverage)
- Updated Bicep infrastructure (player-pictures container)
- Documentation

**Success Criteria**:
- Users can upload, display, and delete player pictures
- Pictures stored securely in Azure Blob Storage
- All validation rules enforced
- No regressions in existing player management functionality

---

### Phase 2: Enhanced Features (Future)

**Scope**:
- Image URL paste/link option
- Thumbnail generation for list views
- Picture cropping/editing in UI
- Drag-and-drop upload support
- Multiple picture gallery per player
- Bulk upload for multiple players
- Picture caching and CDN integration
- Advanced image optimization (compression, format conversion)

**Timeline**: Future releases (as needed)

**Dependencies**: Phase 1 completion

---

## Migration & Deployment Considerations

### Database Migration

**No migration required** if using existing `PhotoUrl` column.

**Optional migration** if adding tracking columns:

**Migration Name**: `AddPlayerPictureTrackingFields`

**Changes**:
- Add `PhotoBlobName` (nvarchar(255), nullable)
- Add `PhotoContainerName` (nvarchar(100), nullable)
- Add `PhotoUploadedAt` (datetime2, nullable)

**Rollback Strategy**: Drop columns if needed (data loss acceptable for these tracking fields)

---

### Azure Infrastructure Deployment

**Bicep Changes** (reference: [infra/modules/storage.bicep](infra/modules/storage.bicep))

**Updates Required**:

1. **Add Player Pictures Blob Container**:
   - Container name: `player-pictures`
   - Public access: None (private)
   - Create resource in storage.bicep module

2. **CORS Configuration** (if needed for client-side uploads):
   - Add CORS rules to blob service
   - Allow origins: application domain
   - Allow methods: GET, PUT, POST, DELETE
   - Allow headers: Content-Type, x-ms-blob-type

3. **Lifecycle Management** (optional, for cleanup):
   - Define policy to delete blobs older than X days if not referenced
   - Schedule cleanup job for orphaned blobs

**Deployment Steps**:
1. Update [infra/modules/storage.bicep](infra/modules/storage.bicep) with new container
2. Run `az bicep build` to validate
3. Deploy infrastructure via GitHub Actions or manual deployment
4. Verify container created in Azure Portal
5. Test connectivity from application

---

### Configuration Changes

**Steps**:
1. Add `AzureStorage` section to [appsettings.json](src/GhcSamplePs.Web/appsettings.json)
2. Store connection string in Azure Key Vault (production)
3. Configure environment variables in Azure Container App
4. Update [Program.cs](src/GhcSamplePs.Web/Program.cs) to register `IBlobStorageService`

**Service Registration** (add to Program.cs):

Register BlobStorageService in the dependency injection container using AddScoped or AddSingleton lifetime, along with configuration binding for AzureStorage settings.

---

### Deployment Steps

**Pre-Deployment**:
1. Complete all unit tests
2. Update documentation
3. Review security checklist
4. Test in development environment

**Deployment**:
1. Deploy infrastructure changes (Bicep)
2. Deploy application code
3. Run smoke tests
4. Monitor logs for errors

**Post-Deployment**:
1. Verify picture upload/delete works in production
2. Check Azure Blob Storage metrics
3. Monitor application logs for exceptions
4. Notify users of new feature

---

### Rollback Strategy

**If Issues Arise**:
1. Disable picture upload feature via feature flag (optional)
2. Revert application code to previous version
3. Leave Azure Blob Storage intact (no data loss)
4. Pictures uploaded before rollback remain accessible

**Data Preservation**:
- Blobs in Azure Storage are not affected by application rollback
- Player records retain PhotoUrl values
- Can re-enable feature once issues resolved

---

## Success Metrics

### Functional Metrics

| Metric | Target | Measurement Method |
|--------|--------|-------------------|
| Upload Success Rate | > 95% | Log successful vs. failed uploads |
| Upload Time (< 5 MB) | < 5 seconds | Measure time from upload start to completion |
| Picture Display Time | < 1 second | Measure time to load and render picture |
| Validation Error Rate | Accurate rejection of invalid files | Log validation failures and types |
| User Satisfaction | Positive feedback | User surveys, feedback forms |

### Technical Metrics

| Metric | Target | Measurement Method |
|--------|--------|-------------------|
| Unit Test Coverage | > 85% | Code coverage report |
| Blob Storage Availability | > 99.9% | Azure metrics |
| Average Blob Size | Track over time | Azure Storage analytics |
| Orphaned Blobs | < 1% of total blobs | Scheduled cleanup job reporting |
| Error Rate | < 1% of operations | Application Insights monitoring |

### User Adoption

| Metric | Target | Measurement Method |
|--------|--------|-------------------|
| % of Players with Pictures | Track growth over time | Database query count |
| Daily Picture Uploads | Monitor trends | Application logs |
| Picture Delete/Re-upload Rate | < 5% of players | Track delete operations |

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Azure Storage quota exceeded | High - prevents uploads | Low | Monitor storage usage, implement alerts, provision adequate storage |
| Large file uploads degrade performance | Medium - slow UI | Medium | Enforce strict 5 MB limit, show progress indicators, compress images |
| Malicious file uploads | High - security breach | Low | Validate file content (not just extension), scan for malware (future) |
| Users upload copyrighted/inappropriate images | Medium - legal issues | Medium | Implement content moderation (future), terms of service, reporting mechanism |
| Blob storage costs exceed budget | Medium - financial | Low | Monitor costs, set alerts, implement lifecycle policies to delete old blobs |
| Network failures during upload | Medium - poor UX | Medium | Implement retry logic, save form state, provide clear error messages |
| Orphaned blobs accumulate | Low - storage waste | Medium | Implement scheduled cleanup job, cascade delete on player removal |
| SAS token expiration issues | Medium - pictures not loading | Low | Use appropriate expiration times (1 hour+), implement token refresh logic |

---

## Open Questions

- [ ] **Image Optimization**: Should we automatically compress/resize uploaded images to reduce storage and improve load times?
- [ ] **CDN Integration**: Should we use Azure CDN for serving pictures to improve global performance?
- [ ] **Multiple Pictures**: Will players need multiple pictures in the future (e.g., gallery, action shots)?
- [ ] **Thumbnail Generation**: Should we generate thumbnails for list views, or use full-size images?
- [ ] **Public Sharing**: Will there be a requirement to share player profiles publicly (affecting access control)?
- [ ] **Backup Strategy**: What is the backup/disaster recovery plan for blob storage?
- [ ] **Content Moderation**: Do we need automated content moderation for uploaded images?
- [ ] **GDPR Compliance**: Are there specific GDPR requirements for storing player pictures (consent, right to deletion)?

---

## Appendix

### Related Documentation

- [Player Management Feature Specification](ManagePlayers_Feature_Specification.md)
- [Azure Storage Bicep Module](../infra/modules/storage.bicep)
- [Blazor Architecture Guidelines](../.github/instructions/blazor-architecture.instructions.md)
- [C# Development Standards](../.github/instructions/csharp.instructions.md)
- [Azure Cosmos DB Instructions](vscode-userdata:/c%3A/Users/covoricardo/AppData/Roaming/Code/User/prompts/azurecosmosdb.instructions.md) (reference for service patterns)

### Azure Blob Storage Resources

- [Azure Blob Storage Documentation](https://learn.microsoft.com/azure/storage/blobs/)
- [Azure Storage .NET SDK](https://learn.microsoft.com/dotnet/api/overview/azure/storage.blobs-readme)
- [SAS Tokens Best Practices](https://learn.microsoft.com/azure/storage/common/storage-sas-overview)
- [Azure Storage Security](https://learn.microsoft.com/azure/storage/common/storage-security-guide)

### Wireframe Reference

See attached wireframe image showing picture placement in Edit Player screen:
- Profile picture displayed at top of Player Information section
- Circular or rounded square display
- Upload and delete controls positioned near picture

### Similar Implementations

- Team Management already uses similar patterns for CRUD operations (reference: [TeamManagement_Feature_Specification_Concise.md](TeamManagement_Feature_Specification_Concise.md))
- Player Statistics uses similar service patterns (reference: IPlayerStatisticService)
- Follow existing error handling and ServiceResult patterns from PlayerService

---

## Glossary

| Term | Definition |
|------|------------|
| Blob | Binary Large Object - a file stored in Azure Blob Storage |
| SAS Token | Shared Access Signature - a URI that grants time-limited access to Azure Storage resources |
| Blob Container | A logical grouping of blobs in Azure Storage (similar to a folder) |
| Content Type | MIME type indicating file format (e.g., "image/jpeg") |
| Managed Identity | Azure AD identity for Azure resources to authenticate without storing credentials |
| Orphaned Blob | A blob in storage that is no longer referenced by any database record |

---

**END OF SPECIFICATION**
