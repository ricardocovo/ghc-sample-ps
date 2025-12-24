# Blob Storage Integration Specification

**Version**: 1.0
**Date**: December 24, 2025
**Related Feature**: [Player Picture Upload and Management](PlayerPictureUpload_Feature_Specification.md)

---

## Executive Summary

This specification provides step-by-step guidance for integrating Azure Blob Storage into the GhcSamplePs application for both **local development** (using Azurite emulator) and **Azure production** environments. This integration enables the Player Picture Upload feature and establishes the foundation for storing binary assets (images, files, documents) in the application.

### Purpose

- Enable developers to work with blob storage locally without Azure costs
- Provide consistent development experience matching production behavior
- Document production Azure Blob Storage configuration
- Establish security and connectivity best practices

### Key Technologies

- **Local Development**: Azurite (Azure Storage Emulator)
- **Production**: Azure Storage Account with Blob Service
- **SDK**: Azure.Storage.Blobs NuGet package (v12.x)
- **Authentication**: Connection strings (local), Managed Identity (production)

---

## Table of Contents

1. [Local Development Setup (Azurite)](#local-development-setup-azurite)
2. [Azure Production Setup](#azure-production-setup)
3. [Application Configuration](#application-configuration)
4. [Testing and Verification](#testing-and-verification)
5. [Troubleshooting](#troubleshooting)
6. [Security Considerations](#security-considerations)

---

## Local Development Setup (Azurite)

### Overview

Azurite is the official Azure Storage emulator that runs locally on your development machine. It provides blob, queue, and table storage emulation with full fidelity to the Azure Storage service.

### Prerequisites

- **Node.js** (version 14 or higher) for npm-based installation, OR
- **Docker Desktop** for container-based installation (recommended)
- **Azure Storage Explorer** (optional but recommended for visual inspection)
- **Visual Studio Code** with Azure Storage extension (optional)

---

### Step 1: Install Azurite

Choose one of the following installation methods:

#### Option A: Docker Installation (Recommended)

**Advantages**: Isolated, easy to start/stop, no global installation

**Steps**:

1. **Pull Azurite Docker Image**
   ```
   docker pull mcr.microsoft.com/azure-storage/azurite
   ```

2. **Create Local Data Directory** (optional, for persistent storage)
   ```
   mkdir C:\azurite-data
   ```

3. **Run Azurite Container**
   ```
   docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 -v C:\azurite-data:/data mcr.microsoft.com/azure-storage/azurite
   ```

   **Port Mappings**:
   - `10000`: Blob service
   - `10001`: Queue service
   - `10002`: Table service

4. **Verify Container Running**
   ```
   docker ps
   ```

   You should see the Azurite container in the list.

#### Option B: NPM Installation

**Advantages**: Runs as local Node.js process, easier for some workflows

**Steps**:

1. **Install Azurite Globally via NPM**
   ```
   npm install -g azurite
   ```

2. **Start Azurite**
   ```
   azurite --silent --location C:\azurite-data --debug C:\azurite-debug.log
   ```

   **Flags**:
   - `--silent`: Suppress console output
   - `--location`: Data persistence directory
   - `--debug`: Enable debug logging to file

3. **Verify Azurite Running**

   Open browser to `http://127.0.0.1:10000` - you should see a blank response (blob service is running).

---

### Step 2: Configure Azurite Connection in Application

#### Default Azurite Connection String

Azurite provides a well-known connection string for local development:

```
UseDevelopmentStorage=true
```

Or the full connection string:

```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;
```

#### Update Application Configuration Files

**File**: [src/GhcSamplePs.Web/appsettings.Development.json](../../src/GhcSamplePs.Web/appsettings.Development.json)

Add the `AzureStorage` section:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "PlayerPicturesContainer": "player-pictures",
    "SasExpirationMinutes": 60,
    "MaxUploadSizeBytes": 5242880
  },
  "Logging": {
    ...
  }
}
```

**Configuration Properties**:

| Property | Value | Description |
|----------|-------|-------------|
| ConnectionString | `UseDevelopmentStorage=true` | Azurite emulator connection |
| PlayerPicturesContainer | `player-pictures` | Container name for player images |
| SasExpirationMinutes | `60` | SAS token validity duration (1 hour) |
| MaxUploadSizeBytes | `5242880` | Maximum file size (5 MB) |

---

### Step 3: Install Required NuGet Package

Add Azure Storage Blobs SDK to the Core project (where business logic resides):

**Command**:
```
dotnet add src/GhcSamplePs.Core/GhcSamplePs.Core.csproj package Azure.Storage.Blobs
```

**Expected Version**: 12.x (latest stable)

**Verification**: Check [src/GhcSamplePs.Core/GhcSamplePs.Core.csproj](../../src/GhcSamplePs.Core/GhcSamplePs.Core.csproj) for package reference:

```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.x.x" />
```

---

### Step 4: Create Blob Container in Azurite

Azurite starts with no containers by default. You must create the `player-pictures` container before use.

#### Option A: Azure Storage Explorer (GUI)

1. **Download and Install** [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/)

2. **Connect to Local Emulator**:
   - Open Storage Explorer
   - Expand "Local & Attached" → "Storage Accounts" → "(Emulator - Default Ports)"
   - Right-click "Blob Containers" → "Create Blob Container"
   - Name: `player-pictures`
   - Public access level: "Private (no anonymous access)"

3. **Verify Container Created**:
   - You should see `player-pictures` under Blob Containers

#### Option B: Azure CLI

1. **Ensure Azure CLI Installed**
   ```
   az --version
   ```

2. **Create Container**
   ```
   az storage container create --name player-pictures --connection-string "UseDevelopmentStorage=true"
   ```

3. **Verify Container**
   ```
   az storage container list --connection-string "UseDevelopmentStorage=true"
   ```

#### Option C: Application Startup Code (Automatic Creation)

Implement container creation logic in application startup to ensure container exists:

**Location**: Update [src/GhcSamplePs.Web/Program.cs](../../src/GhcSamplePs.Web/Program.cs)

**Logic** (described in natural language):
- During application startup, after services are registered
- Retrieve the BlobStorageService from DI container
- Call a method like `EnsureContainersExistAsync()`
- BlobStorageService checks if `player-pictures` container exists
- If not, create it with private access level
- Log the result (container created or already exists)

---

### Step 5: Verify Local Setup

#### Test Blob Upload

Create a simple test to verify Azurite connectivity:

**Location**: [tests/GhcSamplePs.Core.Tests/](../../tests/GhcSamplePs.Core.Tests/) (integration test)

**Test Scenario**:
1. Initialize BlobServiceClient with Azurite connection string
2. Get reference to `player-pictures` container
3. Upload a test blob (small text file or image)
4. Verify blob exists
5. Download blob and verify content
6. Delete test blob
7. Assert all operations succeeded

**Verification Steps**:
1. Start Azurite (Docker or NPM)
2. Run test suite: `dotnet test`
3. Check test output for success
4. Optional: Open Storage Explorer and verify test blob created/deleted

#### Manual Verification with Storage Explorer

1. Start Azurite
2. Run the application: `dotnet run --project src/GhcSamplePs.Web`
3. Navigate to Edit Player screen
4. Upload a test image
5. Open Storage Explorer
6. Navigate to Local Emulator → Blob Containers → player-pictures
7. Verify uploaded blob appears with correct naming convention (e.g., `player-123-20251224.jpg`)

---

### Step 6: Running Azurite During Development

#### Docker Approach

**Start Azurite**:
```
docker run -d --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 -v C:\azurite-data:/data mcr.microsoft.com/azure-storage/azurite
```

**Stop Azurite**:
```
docker stop azurite
```

**Remove Azurite Container**:
```
docker rm azurite
```

**Start Existing Container**:
```
docker start azurite
```

#### NPM Approach

**Start in Background** (Windows):
```
start /b azurite --silent --location C:\azurite-data
```

**Stop**:
- Find process: `tasklist | findstr node`
- Kill process: `taskkill /PID <process-id> /F`

#### VS Code Task (Optional)

Create a VS Code task to start/stop Azurite automatically.

**Location**: [.vscode/tasks.json](../../.vscode/tasks.json) (add new task)

**Task Definition**:
- Label: "start-azurite"
- Type: shell
- Command: `docker start azurite` (or start container if not exists)
- IsBackground: true
- Problem matcher: none

**Usage**: Run task from Command Palette (`Ctrl+Shift+P` → "Run Task" → "start-azurite")

---

### Step 7: Development Workflow Integration

#### Recommended Workflow

1. **Start Azurite** before running application
2. **Verify Azurite running** (check Docker Desktop or process list)
3. **Run application**: `dotnet run --project src/GhcSamplePs.Web` or `dotnet watch`
4. **Develop and test** picture upload features
5. **Stop Azurite** when done (optional, can leave running)

#### Data Persistence

**With Docker Volume** (`-v` flag): Data persists across container restarts

**Without Volume**: Data lost when container removed (useful for clean state testing)

**Clear Data**:
- Stop Azurite
- Delete volume directory (`C:\azurite-data`)
- Restart Azurite

---

## Azure Production Setup

### Overview

This section covers configuring Azure Blob Storage in the Azure cloud for production deployment.

### Prerequisites

- **Azure Subscription** with appropriate permissions
- **Azure CLI** installed and authenticated (`az login`)
- **Contributor or Owner role** on resource group
- **Existing Infrastructure** deployed via Bicep (reference: [infra/](../../infra/))

---

### Step 1: Update Bicep Infrastructure

The existing [infra/modules/storage.bicep](../../infra/modules/storage.bicep) already defines a storage account. Update it to include the `player-pictures` container.

#### Add Player Pictures Container

**File**: [infra/modules/storage.bicep](../../infra/modules/storage.bicep)

**Changes Required**:

Add a new blob container resource after the existing `dataProtectionContainer`:

**Resource Definition** (described in natural language):
- Resource type: `Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01`
- Parent: `blobServices` (same as data protection container)
- Name: `player-pictures`
- Properties:
  - publicAccess: `None` (private, no anonymous access)
- Comments: Container for player profile pictures, accessed via Managed Identity with RBAC

**Existing Pattern Reference**: Follow the same pattern as the `dataProtectionContainer` resource already defined in the file.

#### Add CORS Configuration (Optional)

If implementing client-side direct upload to blob storage (future enhancement), add CORS rules:

**Resource**: Modify `blobServices` resource

**CORS Properties** (described):
- Allowed origins: Application URL(s)
- Allowed methods: GET, PUT, POST, OPTIONS
- Allowed headers: Content-Type, x-ms-blob-type, x-ms-blob-content-type
- Exposed headers: x-ms-request-id
- Max age: 3600 seconds (1 hour)

---

### Step 2: Deploy Updated Infrastructure

#### Validate Bicep Changes

**Command**:
```
az bicep build --file infra/main.bicep
```

**Expected Output**: No errors, JSON template generated at `infra/main.json`

#### Deploy Infrastructure

**Option A: GitHub Actions** (Recommended)

1. Push changes to main branch
2. GitHub Actions workflow automatically triggers
3. Infrastructure deployment runs via [.github/workflows/infrastructure.yml](../../.github/workflows/)
4. Monitor workflow progress in GitHub Actions tab
5. Verify deployment succeeded

**Option B: Manual Deployment**

**Command**:
```
az deployment group create --resource-group <your-rg-name> --template-file infra/main.bicep --parameters infra/main.bicepparam
```

**Parameters**: Update [infra/main.bicepparam](../../infra/main.bicepparam) with environment-specific values

**Verification**:
```
az deployment group show --resource-group <your-rg-name> --name main --query properties.provisioningState
```

Expected output: `Succeeded`

---

### Step 3: Verify Container Created in Azure

#### Azure Portal

1. Navigate to [Azure Portal](https://portal.azure.com)
2. Open the Storage Account (name from Bicep output)
3. Go to "Containers" under "Data storage"
4. Verify `player-pictures` container exists
5. Check "Public access level" = Private (no anonymous access)

#### Azure CLI

**List Containers**:
```
az storage container list --account-name <storage-account-name> --auth-mode login
```

**Check Specific Container**:
```
az storage container show --name player-pictures --account-name <storage-account-name> --auth-mode login
```

#### Azure Storage Explorer

1. Connect to Azure account in Storage Explorer
2. Navigate to subscription → Storage Accounts → [your-storage-account]
3. Expand "Blob Containers"
4. Verify `player-pictures` listed
5. Right-click → Properties → Verify access level

---

### Step 4: Configure Managed Identity Access (RBAC)

For security best practices, the Container App should access Blob Storage using Managed Identity (not connection strings).

#### Enable System-Assigned Managed Identity on Container App

**Already Configured**: The existing Bicep infrastructure at [infra/modules/containerapp.bicep](../../infra/modules/containerapp.bicep) should already enable Managed Identity.

**Verification**:
```
az containerapp show --name <container-app-name> --resource-group <rg-name> --query identity.type
```

Expected output: `SystemAssigned`

#### Assign Storage Blob Data Contributor Role

**Role**: `Storage Blob Data Contributor` (allows read, write, delete blobs)

**Assignment** (via Azure CLI):
```
az role assignment create --assignee <container-app-managed-identity-principal-id> --role "Storage Blob Data Contributor" --scope /subscriptions/<subscription-id>/resourceGroups/<rg-name>/providers/Microsoft.Storage/storageAccounts/<storage-account-name>
```

**Get Managed Identity Principal ID**:
```
az containerapp show --name <container-app-name> --resource-group <rg-name> --query identity.principalId -o tsv
```

**Verification**:
```
az role assignment list --assignee <principal-id> --scope <storage-account-resource-id>
```

#### Update Bicep for Automatic RBAC Assignment

**File**: [infra/modules/acr-rbac.bicep](../../infra/modules/acr-rbac.bicep) or create new `storage-rbac.bicep`

**Resource** (described):
- Resource type: `Microsoft.Authorization/roleAssignments@2022-04-01`
- Role definition ID: Storage Blob Data Contributor (built-in role)
- Principal ID: Container App Managed Identity principal ID
- Scope: Storage Account resource ID
- Principal type: ServicePrincipal

**Reference**: Follow pattern in existing [infra/modules/acr-rbac.bicep](../../infra/modules/acr-rbac.bicep)

---

### Step 5: Configure Application Settings in Container App

Update Container App environment variables to use Azure Blob Storage.

#### Required Environment Variables

| Variable Name | Value | Source |
|---------------|-------|--------|
| `AzureStorage__BlobEndpoint` | `https://<account>.blob.core.windows.net/` | Storage account primary blob endpoint |
| `AzureStorage__PlayerPicturesContainer` | `player-pictures` | Container name |
| `AzureStorage__SasExpirationMinutes` | `60` | SAS token expiration (if using SAS) |
| `AzureStorage__MaxUploadSizeBytes` | `5242880` | Max upload size (5 MB) |
| `AzureStorage__UseManagedIdentity` | `true` | Enable Managed Identity authentication |

**Note**: Do NOT set `ConnectionString` in production - use Managed Identity instead.

#### Set Environment Variables via Azure CLI

**Command** (set multiple variables):
```
az containerapp update --name <container-app-name> --resource-group <rg-name> --set-env-vars AzureStorage__BlobEndpoint=https://<account>.blob.core.windows.net/ AzureStorage__PlayerPicturesContainer=player-pictures AzureStorage__UseManagedIdentity=true AzureStorage__SasExpirationMinutes=60 AzureStorage__MaxUploadSizeBytes=5242880
```

#### Set via Bicep (Recommended)

**File**: [infra/modules/containerapp.bicep](../../infra/modules/containerapp.bicep)

**Changes**: Add environment variables to container app definition

**Environment Variable Array** (described):
- Add new entries to the `env` array in the container configuration
- Name: `AzureStorage__BlobEndpoint`
- Value: Reference storage account blob endpoint from storage module output
- Repeat for other variables
- Use parameter or variable for dynamic values
- Use Key Vault reference for sensitive values (if any)

**Reference Output**: Use storage module's `blobEndpoint` output value

---

### Step 6: Update Application Code for Managed Identity

The BlobStorageService implementation must support both connection string (local) and Managed Identity (production) authentication.

#### Authentication Logic (Described)

**Location**: [src/GhcSamplePs.Core/Services/Implementations/BlobStorageService.cs](../../src/GhcSamplePs.Core/Services/Implementations/)

**Implementation Pattern**:

1. **Constructor**: Inject IConfiguration and ILogger
2. **Initialize BlobServiceClient**:
   - Check configuration for `UseManagedIdentity` flag
   - **If true** (production):
     - Create DefaultAzureCredential instance
     - Initialize BlobServiceClient with blob endpoint URL and credential
   - **If false** (local development):
     - Get connection string from configuration
     - Initialize BlobServiceClient with connection string
3. **Container Access**: Get BlobContainerClient for `player-pictures` container
4. **All methods**: Use the initialized client for blob operations

**Reference Pattern**: See [src/GhcSamplePs.Web/Program.cs](../../src/GhcSamplePs.Web/Program.cs) lines 35-50 for DefaultAzureCredential usage example

**Example Configuration Check**:
- Read `AzureStorage:UseManagedIdentity` from configuration
- Read `AzureStorage:ConnectionString` for local development
- Read `AzureStorage:BlobEndpoint` for production
- Determine authentication method based on these values

---

### Step 7: Deploy Application with Blob Storage Support

#### Build and Push Docker Image

**Reference**: [infra/scripts/build-push-image.ps1](../../infra/scripts/build-push-image.ps1)

**Commands**:
```powershell
cd infra/scripts
./build-push-image.ps1 -RegistryName <your-acr-name> -ImageTag v1.0.0
```

#### Update Container App with New Image

**Command**:
```
az containerapp update --name <container-app-name> --resource-group <rg-name> --image <acr-name>.azurecr.io/ghcsampleps-web:v1.0.0
```

**Verification**:
```
az containerapp revision list --name <container-app-name> --resource-group <rg-name> --query [0].properties.provisioningState
```

Expected: `Provisioned` and revision active

---

### Step 8: Verify Production Setup

#### Test Upload in Production

1. Navigate to application URL (Container App FQDN)
2. Authenticate via Entra ID
3. Navigate to Edit Player screen
4. Upload a test player picture
5. Verify success message displays
6. Verify picture displays correctly

#### Verify Blob in Azure Storage

**Azure Portal**:
1. Open Storage Account
2. Navigate to Containers → player-pictures
3. Verify blob uploaded with correct naming convention
4. Check blob properties (content type, size)

**Azure CLI**:
```
az storage blob list --container-name player-pictures --account-name <storage-account-name> --auth-mode login
```

#### Check Application Logs

**Container App Logs**:
```
az containerapp logs show --name <container-app-name> --resource-group <rg-name> --follow
```

**Look for**:
- Successful blob upload log entries
- No authentication errors
- No permission denied errors
- Correct blob URLs in logs

#### Test Delete Operation

1. In application, delete the uploaded picture
2. Verify confirmation dialog appears
3. Confirm deletion
4. Verify picture removed from UI
5. Check Azure Storage - blob should be deleted

---

## Application Configuration

### Configuration Structure

The application uses hierarchical configuration with environment-specific overrides:

1. **appsettings.json**: Base configuration (minimal, no secrets)
2. **appsettings.Development.json**: Local development overrides (Azurite)
3. **Environment Variables**: Production overrides (Azure Container App)

### Configuration Class Binding (Described)

**Location**: [src/GhcSamplePs.Core/Common/](../../src/GhcSamplePs.Core/Common/) (create new)

**Class Name**: `AzureStorageOptions`

**Properties**:
- ConnectionString (string, nullable): Connection string for local development
- BlobEndpoint (string, nullable): Blob service endpoint URL
- PlayerPicturesContainer (string, required): Container name for player pictures
- SasExpirationMinutes (int, default 60): SAS token expiration time
- MaxUploadSizeBytes (long, default 5242880): Maximum file upload size
- UseManagedIdentity (bool, default false): Enable Managed Identity authentication

**Registration** (described):
- In [Program.cs](../../src/GhcSamplePs.Web/Program.cs), bind configuration section "AzureStorage" to AzureStorageOptions
- Register as IOptions<AzureStorageOptions> in DI container
- Inject into BlobStorageService constructor

### Environment-Specific Configuration

#### Development (Azurite)

**File**: [src/GhcSamplePs.Web/appsettings.Development.json](../../src/GhcSamplePs.Web/appsettings.Development.json)

```json
{
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "PlayerPicturesContainer": "player-pictures",
    "SasExpirationMinutes": 60,
    "MaxUploadSizeBytes": 5242880,
    "UseManagedIdentity": false
  }
}
```

#### Production (Azure)

**Environment Variables** (set in Container App):
- `AzureStorage__BlobEndpoint`: `https://stghcsamplepsdev.blob.core.windows.net/` (example)
- `AzureStorage__PlayerPicturesContainer`: `player-pictures`
- `AzureStorage__SasExpirationMinutes`: `60`
- `AzureStorage__MaxUploadSizeBytes`: `5242880`
- `AzureStorage__UseManagedIdentity`: `true`

**No Connection String**: Security best practice - use Managed Identity

---

## Testing and Verification

### Unit Tests

**Location**: [tests/GhcSamplePs.Core.Tests/Services/BlobStorageServiceTests.cs](../../tests/GhcSamplePs.Core.Tests/)

**Test Scenarios**:

1. **Constructor Initialization**:
   - Given connection string configuration, initializes client correctly
   - Given Managed Identity configuration, initializes client correctly
   - Given missing configuration, throws appropriate exception

2. **Upload Operations** (mocked Azure SDK):
   - UploadPlayerPictureAsync succeeds with valid input
   - UploadPlayerPictureAsync fails with oversized file
   - UploadPlayerPictureAsync generates unique blob names
   - UploadPlayerPictureAsync returns correct blob URL

3. **Delete Operations**:
   - DeletePlayerPictureAsync succeeds when blob exists
   - DeletePlayerPictureAsync handles non-existent blob gracefully

4. **SAS Token Generation**:
   - GetPictureUrlWithSasAsync generates valid SAS URL
   - SAS token has correct expiration time
   - SAS token has read-only permissions

**Mocking Strategy**: Mock BlobServiceClient, BlobContainerClient, and BlobClient using test doubles or mocking framework (Moq)

### Integration Tests (Local)

**Location**: [tests/GhcSamplePs.Core.Tests/Integration/](../../tests/GhcSamplePs.Core.Tests/)

**Prerequisites**: Azurite running before test execution

**Test Scenarios**:

1. **End-to-End Upload**:
   - Start Azurite
   - Create BlobStorageService with Azurite connection
   - Upload real test image file
   - Verify blob exists in Azurite
   - Verify blob content matches uploaded file
   - Clean up test blob

2. **Container Creation**:
   - Verify container auto-creation if not exists
   - Verify idempotency (multiple calls don't fail)

3. **Error Handling**:
   - Stop Azurite
   - Attempt upload
   - Verify appropriate exception handling and error message

**Test Fixtures**: Create small test image files (JPEG, PNG) in test project

### Manual Testing Checklist

#### Local Development

- [ ] Azurite running and accessible
- [ ] Application starts without errors
- [ ] Navigate to Edit Player screen
- [ ] Upload JPEG image (< 5 MB) → Success
- [ ] Upload PNG image → Success
- [ ] Upload oversized file (> 5 MB) → Error message
- [ ] Upload invalid file type → Error message
- [ ] Uploaded picture displays correctly
- [ ] Delete picture → Confirmation dialog → Picture removed
- [ ] Verify blob in Storage Explorer
- [ ] Stop Azurite → Upload fails gracefully with error message

#### Azure Production

- [ ] Application deployed successfully
- [ ] Navigate to Edit Player screen (authenticated)
- [ ] Upload JPEG image → Success
- [ ] Picture displays correctly
- [ ] Verify blob in Azure Storage via Portal
- [ ] Delete picture → Success
- [ ] Verify blob deleted from Azure Storage
- [ ] Check application logs for errors
- [ ] Test from different users (authorization)
- [ ] Test on mobile device (responsive design)

---

## Troubleshooting

### Local Development Issues

#### Azurite Not Starting

**Symptoms**: Application fails with "Unable to connect to storage emulator"

**Solutions**:
1. **Check if Azurite running**:
   - Docker: `docker ps | findstr azurite`
   - NPM: `tasklist | findstr node`
2. **Check port conflicts**:
   - Verify nothing else using ports 10000, 10001, 10002
   - Windows: `netstat -ano | findstr :10000`
3. **Restart Azurite**:
   - Docker: `docker restart azurite`
   - NPM: Kill process and restart
4. **Check Docker Desktop running** (if using Docker)
5. **Review Azurite logs**:
   - Docker: `docker logs azurite`
   - NPM: Check debug log file

#### Container Not Found

**Symptoms**: "Container 'player-pictures' not found"

**Solutions**:
1. **Create container** using Storage Explorer or Azure CLI (see Step 4 above)
2. **Enable auto-create** in BlobStorageService
3. **Verify connection string correct** in configuration
4. **Check container name spelling** (case-sensitive)

#### Connection String Invalid

**Symptoms**: "Invalid connection string format"

**Solutions**:
1. **Verify using standard Azurite connection**: `UseDevelopmentStorage=true`
2. **Check no extra spaces or quotes** in appsettings.json
3. **Ensure JSON syntax valid** (no trailing commas)
4. **Restart application** after configuration changes

#### Slow Upload Performance

**Symptoms**: Uploads taking longer than expected locally

**Solutions**:
1. **Use Docker Azurite instead of NPM** (better performance)
2. **Allocate more resources to Docker Desktop**
3. **Check antivirus not scanning Azurite data directory**
4. **Use SSD for Azurite data directory**

---

### Azure Production Issues

#### Authentication Failed

**Symptoms**: "Authentication failed" or "403 Forbidden" errors

**Solutions**:
1. **Verify Managed Identity enabled** on Container App:
   ```
   az containerapp show --name <app-name> --resource-group <rg> --query identity
   ```
2. **Verify RBAC role assigned**:
   ```
   az role assignment list --assignee <principal-id> --scope <storage-account-id>
   ```
3. **Check role is "Storage Blob Data Contributor"** (not Reader)
4. **Wait for RBAC propagation** (can take 5-10 minutes)
5. **Verify BlobEndpoint URL correct** in environment variables
6. **Check application using DefaultAzureCredential** correctly

#### Container Not Found in Production

**Symptoms**: "Container 'player-pictures' does not exist"

**Solutions**:
1. **Verify Bicep deployment succeeded**:
   ```
   az deployment group show --resource-group <rg> --name main --query properties.provisioningState
   ```
2. **Check container exists** in Azure Portal or CLI
3. **Verify environment variable correct**: `AzureStorage__PlayerPicturesContainer`
4. **Check spelling and case** (container names lowercase)

#### Slow Upload Performance in Azure

**Symptoms**: Uploads taking longer than 5 seconds

**Solutions**:
1. **Check Container App region** matches Storage Account region (minimize latency)
2. **Review Storage Account performance tier** (upgrade to Premium if needed)
3. **Check network restrictions** on Storage Account
4. **Verify no bandwidth throttling** at container app level
5. **Review application logs** for retries or timeouts
6. **Check storage account metrics** in Azure Portal

#### Managed Identity Not Working

**Symptoms**: "DefaultAzureCredential failed to retrieve token"

**Solutions**:
1. **Verify System-Assigned Identity enabled**:
   ```
   az containerapp show --name <app> --resource-group <rg> --query identity.principalId
   ```
2. **Check Container App has latest revision deployed**
3. **Verify environment variable** `AzureStorage__UseManagedIdentity` set to `true`
4. **Test RBAC assignment**:
   ```
   az role assignment list --assignee <principal-id>
   ```
5. **Check Azure AD token retrieval** in application logs
6. **Ensure no connection string in production config** (conflict)

#### Blob Not Deleting

**Symptoms**: Delete operation succeeds but blob remains

**Solutions**:
1. **Check RBAC permissions include delete** (Storage Blob Data Contributor required)
2. **Verify blob name correct** in delete request
3. **Check for soft delete enabled** on storage account (blob in recycle bin)
4. **Review application logs** for actual delete operation
5. **Verify no blob leases active** (locks deletion)

---

## Security Considerations

### Local Development Security

**Azurite Security Characteristics**:
- **Well-known credentials**: Everyone knows the Azurite account key (by design)
- **Not suitable for production**: Only for local development
- **No encryption**: Data stored in plain text locally
- **No authentication**: Anyone with network access can connect

**Best Practices**:
1. **Never commit connection strings** to source control (already in .gitignore)
2. **Use appsettings.Development.json** for local configuration (not tracked)
3. **Don't expose Azurite ports publicly** (bind to localhost only)
4. **Clear test data regularly** (avoid accumulating sensitive test images)
5. **Use representative but not real data** for testing

### Azure Production Security

#### Authentication Best Practices

**Managed Identity** (Recommended):
- ✅ No credentials in code or configuration
- ✅ Automatic credential rotation by Azure
- ✅ Fine-grained RBAC permissions
- ✅ Audit trail in Azure AD logs

**Connection Strings** (Avoid in production):
- ❌ Credentials in configuration or Key Vault
- ❌ Manual rotation required
- ❌ Full account access (over-privileged)
- ❌ Risk of exposure in logs or errors

#### Storage Account Security

**Network Security**:
- Configure firewall rules to restrict access to specific IPs or VNets (if needed)
- Enable "Allow Azure services" to permit Container App access
- Consider Private Endpoints for enhanced security (advanced)

**Data Security**:
- **Encryption at rest**: Enabled by default on Azure Storage
- **Encryption in transit**: HTTPS only (enforced in Bicep)
- **TLS version**: Minimum TLS 1.2 (enforced in Bicep)
- **Public access**: Disabled at account level (enforced in Bicep)

**Access Control**:
- Use RBAC (Role-Based Access Control) over account keys
- Grant minimum necessary permissions (Blob Data Contributor, not Owner)
- Audit access logs regularly via Azure Monitor
- Enable storage account logging (read, write, delete operations)

#### Application Security

**File Validation**:
- **Client-side**: Validate file size and type before upload (user experience)
- **Server-side**: Re-validate all input (security enforcement)
- **Content validation**: Check file headers, not just extension
- **Malware scanning**: Consider Azure Defender for Storage (optional)

**SAS Tokens** (if used):
- **Short expiration**: 1 hour or less
- **Read-only**: Don't grant write permissions in SAS URLs
- **IP restrictions**: Limit SAS token to specific IPs (if feasible)
- **HTTPS only**: Ensure SAS URLs use HTTPS

**Authorization**:
- Verify user owns the player before upload/delete operations
- Implement authorization checks in PlayerService (not UI)
- Log all upload/delete operations with user ID for audit trail
- Rate limit uploads to prevent abuse

---

## Appendix

### Quick Reference: Common Commands

#### Azurite (Docker)

```powershell
# Start Azurite
docker run -d --name azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite

# Stop Azurite
docker stop azurite

# Remove Azurite
docker rm azurite

# View logs
docker logs azurite -f
```

#### Azure Storage (Azure CLI)

```powershell
# List containers
az storage container list --account-name <name> --auth-mode login

# Create container
az storage container create --name player-pictures --account-name <name> --auth-mode login

# List blobs
az storage blob list --container-name player-pictures --account-name <name> --auth-mode login

# Delete blob
az storage blob delete --container-name player-pictures --name <blob-name> --account-name <name> --auth-mode login
```

#### Container App Configuration

```powershell
# Set environment variable
az containerapp update --name <app> --resource-group <rg> --set-env-vars KEY=VALUE

# View environment variables
az containerapp show --name <app> --resource-group <rg> --query properties.configuration.activeRevisionsMode

# View logs
az containerapp logs show --name <app> --resource-group <rg> --follow
```

### Related Documentation

- [Azure Storage Documentation](https://learn.microsoft.com/azure/storage/)
- [Azurite Emulator Documentation](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)
- [Azure.Storage.Blobs SDK Documentation](https://learn.microsoft.com/dotnet/api/azure.storage.blobs)
- [Player Picture Upload Feature Specification](PlayerPictureUpload_Feature_Specification.md)
- [Infrastructure Setup Guide](../infra/implementation-plan.md)
- [Development Environment Setup](../Development_Environment_Setup.md)

### Useful Tools

- **Azure Storage Explorer**: [Download](https://azure.microsoft.com/features/storage-explorer/)
- **VS Code Azure Storage Extension**: [Marketplace](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage)
- **Postman**: For API testing with blob uploads
- **Azure CLI**: [Installation Guide](https://learn.microsoft.com/cli/azure/install-azure-cli)

---

**Document Version**: 1.0
**Last Updated**: December 24, 2025
**Author**: Development Team
**Review Date**: Q1 2026
