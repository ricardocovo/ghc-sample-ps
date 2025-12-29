# Storage Account Deployment Update Plan for Player Photo Feature

**Version**: 1.0 | **Date**: December 29, 2025 | **Status**: Planning

---

## Executive Summary

The player photo feature requires an additional blob container (`player-pictures`) in the existing Azure Storage Account. The current infrastructure already provisions a storage account for ASP.NET Core Data Protection keys with a `dataprotection-keys` container. This plan outlines the minimal changes needed to support the player photo feature while maintaining the existing Data Protection functionality.

### Current State
- ✅ Storage account provisioned via `infra/modules/storage.bicep`
- ✅ `dataprotection-keys` container exists for Data Protection
- ✅ Managed Identity RBAC assignments configured (Storage Blob Data Contributor)
- ✅ Storage account outputs available (blobEndpoint, storageAccountName)

### Required Changes
- ➕ Add `player-pictures` blob container to storage module
- ➕ Configure container for private access (no anonymous access)
- ➕ Update main.bicep outputs to expose player pictures container name
- ➕ Document deployment process and verification steps

---

## Architecture Analysis

### Current Storage Implementation

**Module**: `infra/modules/storage.bicep`

**Existing Components**:
1. **Storage Account** (`Microsoft.Storage/storageAccounts@2023-05-01`)
   - Name: `{appName}{environment}st` (e.g., `ghcsamplespsdevst`)
   - SKU: `Standard_LRS` (Locally Redundant Storage)
   - Kind: `StorageV2` (General-purpose v2)
   - Security: HTTPS only, TLS 1.2 minimum
   - Public blob access: Disabled
   - Access tier: Hot

2. **Blob Services** (`Microsoft.Storage/storageAccounts/blobServices@2023-05-01`)
   - Name: `default`
   - Delete retention: Disabled (development configuration)

3. **Existing Container** (`Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01`)
   - Name: `dataprotection-keys`
   - Public access: None (private)
   - Purpose: ASP.NET Core Data Protection keys persistence

**Existing RBAC**:
- Container App Managed Identity has `Storage Blob Data Contributor` role
- Scope: Resource Group level (applies to all storage accounts)
- Allows read/write/delete operations on all containers

### Player Photo Requirements

**From Blob_Storage_Integration_Specification.md**:

1. **Container Configuration**
   - Name: `player-pictures`
   - Public access: None (private, accessed via SAS tokens)
   - Automatic creation: Handled by `BlobStorageInitializationService` at runtime

2. **Access Pattern**
   - Upload/Delete: Via Managed Identity (Storage Blob Data Contributor)
   - Read (User Access): Via time-limited SAS tokens (60 minutes default)
   - Authorization: User must own the player record

3. **Security**
   - Container created with `PublicAccessType.None`
   - All access requires authentication
   - SAS tokens generated on-demand with read-only permissions

4. **Application Configuration** (handled by app, not infrastructure):
   - `AzureStorage:ConnectionString` - Managed Identity or connection string
   - `AzureStorage:PlayerPicturesContainer` - "player-pictures"
   - `AzureStorage:SasExpirationMinutes` - 60
   - `AzureStorage:MaxUploadSizeBytes` - 5,242,880 (5 MB)

---

## Implementation Plan

### Phase 1: Update Storage Module

**File**: `infra/modules/storage.bicep`

**Changes Required**:

1. **Add Player Pictures Container Resource**
   ```bicep
   // Blob Container - Player profile pictures storage
   // Name: player-pictures
   // Access: None (private, accessed via SAS tokens or Managed Identity)
   resource playerPicturesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
     parent: blobServices
     name: 'player-pictures'
     properties: {
       publicAccess: 'None'
     }
   }
   ```

2. **Add Output for Player Pictures Container**
   ```bicep
   @description('Name of the player pictures blob container')
   output playerPicturesContainerName string = playerPicturesContainer.name
   ```

**Validation**:
- ✅ No changes to existing resources (storage account, blob services, dataprotection-keys)
- ✅ New container follows same security pattern (private access)
- ✅ Idempotent deployment (safe to re-run)
- ✅ Minimal change principle (only add required resources)

### Phase 2: Update Main Template (Optional)

**File**: `infra/main.bicep`

**Optional Output Addition** (for documentation/visibility):
```bicep
@description('Name of the player pictures blob container')
output playerPicturesContainerName string = storage.outputs.playerPicturesContainerName
```

**Rationale**:
- Container name is static (`player-pictures`), so output is informational only
- Application configuration uses hardcoded container name from appsettings
- Output useful for documentation and Azure Portal verification

**Decision**: **Add output for completeness** ✅

### Phase 3: Deployment Process

**Prerequisites**:
- Azure CLI installed and authenticated
- Bicep CLI installed (included with Azure CLI 2.20.0+)
- Subscription and resource group selected
- Bicep parameter file configured (`infra/main.bicepparam`)

**Deployment Steps**:

1. **Pre-flight Check**
   ```powershell
   # Verify Bicep installation
   bicep --version

   # Validate template syntax
   bicep build infra/main.bicep --stdout --no-restore

   # Lint template
   bicep lint infra/main.bicep

   # Format template (optional)
   bicep format infra/main.bicep
   ```

2. **Review Changes (What-If)**
   ```powershell
   # Preview deployment changes
   az deployment group what-if `
     --resource-group ghcsampleps-dev-rg `
     --template-file infra/main.bicep `
     --parameters infra/main.bicepparam
   ```

   **Expected What-If Output**:
   ```
   Resource changes: 1 to create, 0 to modify, 0 to delete

   + Microsoft.Storage/storageAccounts/blobServices/containers/player-pictures
       name: "player-pictures"
       properties.publicAccess: "None"
   ```

3. **Deploy Infrastructure**
   ```powershell
   # Deploy updated template
   az deployment group create `
     --resource-group ghcsampleps-dev-rg `
     --template-file infra/main.bicep `
     --parameters infra/main.bicepparam `
     --mode Incremental
   ```

   **Expected Output**:
   ```
   Deployment succeeded

   Outputs:
   - playerPicturesContainerName: "player-pictures"
   - storageAccountName: "ghcsamplespsdevst"
   - blobEndpoint: "https://ghcsamplespsdevst.blob.core.windows.net/"
   ```

4. **Post-Deployment Verification**
   ```powershell
   # Verify container exists
   az storage container show `
     --name player-pictures `
     --account-name ghcsamplespsdevst `
     --auth-mode login

   # List all containers
   az storage container list `
     --account-name ghcsamplespsdevst `
     --auth-mode login `
     --output table
   ```

   **Expected Container List**:
   ```
   Name                   PublicAccess
   ---------------------  --------------
   dataprotection-keys    None
   player-pictures        None
   ```

### Phase 4: Application Configuration

**No infrastructure changes required** - Configuration handled in application code:

1. **Connection String** (already configured in main.bicep)
   - Passed to Container App via `blobEndpoint` output
   - Application uses Managed Identity or connection string from Key Vault

2. **Container Name** (hardcoded in application)
   - Defined in `appsettings.json`: `"PlayerPicturesContainer": "player-pictures"`
   - Application calls `EnsureContainerExistsAsync()` at startup (idempotent)

3. **Runtime Initialization**
   - `BlobStorageInitializationService` runs at app startup
   - Creates container if not exists (redundant after this deployment)
   - No manual intervention required

---

## Deployment Validation Checklist

### Pre-Deployment
- [ ] Bicep templates pass linting (`bicep lint`)
- [ ] Bicep templates compile successfully (`bicep build`)
- [ ] What-if analysis reviewed and approved
- [ ] Backup of existing infrastructure state (ARM template export)
- [ ] Team notification sent (if applicable)

### Post-Deployment
- [ ] Deployment completed successfully (no errors)
- [ ] `player-pictures` container exists in storage account
- [ ] Container has `publicAccess: None` configuration
- [ ] Managed Identity RBAC still applies (Storage Blob Data Contributor)
- [ ] `dataprotection-keys` container unaffected
- [ ] Application logs show successful container initialization
- [ ] Upload test picture via application UI
- [ ] Verify SAS token generation works
- [ ] Delete test picture via application UI
- [ ] Monitor Application Insights for errors (24 hours)

---

## Rollback Plan

### Scenario 1: Deployment Fails

**Action**: No rollback needed
- Incremental deployment mode prevents resource deletion
- Existing resources remain unchanged
- Fix Bicep template and re-deploy

### Scenario 2: Container Misconfigured

**Action**: Update and re-deploy
```powershell
# Fix configuration in storage.bicep
# Re-deploy with corrected template
az deployment group create \
  --resource-group ghcsampleps-dev-rg \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam \
  --mode Incremental
```

### Scenario 3: Application Errors After Deployment

**Action**: Manual container deletion (infrastructure unchanged)
```powershell
# Delete container if causing issues
az storage container delete \
  --name player-pictures \
  --account-name ghcsamplespsdevst \
  --auth-mode login
```

**Note**: Deleting the container does not affect other infrastructure. Application will recreate it on next startup.

---

## Cost Impact Analysis

### Additional Storage Costs

**New Resource**: One additional blob container (`player-pictures`)

**Storage Account Costs** (Canada Central, Standard LRS):
- **Storage Capacity**: $0.0208 per GB/month
  - Estimated usage: 1 GB (200 players × 5 MB/picture) = **$0.02/month**
  - Growth estimate: 10 GB/year = **$0.21/month** at end of year

- **Write Operations**: $0.0065 per 10,000 operations
  - Estimated: 1,000 uploads/month = **$0.0007/month**

- **Read Operations**: $0.0005 per 10,000 operations
  - Estimated: 10,000 views/month (via SAS) = **$0.0005/month**

- **Data Egress**: $0.087 per GB (first 100 GB free)
  - Estimated: < 1 GB/month = **$0** (within free tier)

**Total Additional Cost**: **~$0.02 - $0.25/month** (depending on usage)

**Existing Costs** (unchanged):
- Storage account base cost: Included in existing infrastructure budget
- Data Protection keys storage: Negligible (< 1 MB)

### Comparison to Alternatives

| Option | Monthly Cost | Pros | Cons |
|--------|-------------|------|------|
| **Azure Blob Storage** (chosen) | $0.02-$0.25 | - Integrated with existing account<br>- Managed Identity support<br>- Scalable<br>- SAS token support | - Requires infrastructure change |
| New Storage Account | $0.05-$0.30 | - Isolated from Data Protection storage | - Separate RBAC configuration<br>- Additional management overhead |
| Azure CDN + Blob | $0.10-$1.00 | - Global distribution<br>- Faster access | - Overkill for development<br>- Higher cost |
| Database (varbinary) | $0-$5.00 | - No infrastructure change | - Poor performance<br>- 5 MB pictures bloat database<br>- Expensive SQL storage |

**Recommendation**: ✅ **Use existing storage account** (minimal cost, best practices)

---

## Security Considerations

### Access Control

**No changes to existing security model**:
- ✅ Managed Identity already has `Storage Blob Data Contributor` role (resource group scope)
- ✅ Role applies to all containers in the storage account
- ✅ No additional RBAC assignments required

**Container Security**:
- ✅ `publicAccess: None` prevents anonymous access
- ✅ SAS tokens generated on-demand with minimal permissions (read-only)
- ✅ SAS token expiration enforced (60 minutes default)
- ✅ Authorization enforced at application layer (user must own player)

### Compliance

**Data Protection (GDPR)**:
- ✅ Encryption at rest (Azure Storage default)
- ✅ Encryption in transit (HTTPS enforced)
- ✅ Data residency (Canada Central)
- ✅ Audit logging (Azure Storage Analytics)
- ⚠️ **Action Required**: Implement player data deletion workflow
  - When player is deleted, blob must also be deleted
  - Already implemented in `PlayerService.DeletePlayerPictureAsync`
  - Consider orphaned blob cleanup job (future enhancement)

**Personal Data**:
- Player pictures are considered personal data (GDPR Article 4)
- Right to erasure (GDPR Article 17) requires blob deletion when player is deleted
- Current implementation supports this via `DeletePlayerPictureAsync`

---

## Testing Strategy

### Unit Tests (No Changes Required)

**Existing Tests**: 53 tests covering blob storage functionality
- ✅ `BlobStorageServiceTests.cs` - 21 tests
- ✅ `PlayerPictureValidatorTests.cs` - 19 tests
- ✅ `PlayerServicePictureTests.cs` - 13 tests

**Coverage**: All tests use mocked `BlobContainerClient`, so infrastructure changes don't affect unit tests.

### Integration Tests (Manual)

**Test Plan**:

1. **Container Existence**
   ```powershell
   # Verify container exists after deployment
   az storage container show \
     --name player-pictures \
     --account-name ghcsamplespsdevst \
     --auth-mode login
   ```

2. **Managed Identity Access**
   ```powershell
   # Test upload via Azure CLI (simulates Managed Identity)
   az storage blob upload \
     --container-name player-pictures \
     --file test-image.jpg \
     --name test-upload.jpg \
     --account-name ghcsamplespsdevst \
     --auth-mode login
   ```

3. **Application Upload Test**
   - Navigate to player profile page
   - Upload test picture (< 5 MB, supported format)
   - Verify picture displays with SAS URL
   - Check Application Insights for successful upload event

4. **SAS Token Generation**
   - Upload picture via application
   - Copy generated SAS URL from browser network tab
   - Verify URL is accessible in browser
   - Wait 60 minutes and verify URL expires (optional)

5. **Picture Deletion**
   - Delete picture via application
   - Verify picture removed from blob storage
   - Verify Player.PhotoUrl is null in database

### Load Testing (Optional)

**Scenario**: 100 concurrent picture uploads
```powershell
# Use Azure Load Testing or Apache JMeter
# Target: Player picture upload endpoint
# Expected: All uploads succeed with < 5s latency
```

---

## Monitoring & Alerts

### Application Insights Queries

**Picture Upload Success Rate**:
```kusto
traces
| where message contains "Successfully uploaded player picture"
| summarize SuccessfulUploads = count() by bin(timestamp, 1h)
```

**Picture Upload Failures**:
```kusto
traces
| where message contains "Failed to upload picture"
| project timestamp, message, customDimensions
```

**Storage Account Operations**:
```kusto
dependencies
| where type == "Azure blob"
| summarize Count = count(), AvgDuration = avg(duration) by name, resultCode
| order by Count desc
```

### Recommended Alerts

1. **High Upload Failure Rate**
   - Condition: > 10% of uploads fail in 15 minutes
   - Action: Email notification to DevOps team

2. **Storage Account Throttling**
   - Condition: HTTP 503 responses from storage account
   - Action: Scale up storage account tier (if needed)

3. **Orphaned Blobs**
   - Condition: Blobs exist but not referenced in database
   - Action: Scheduled cleanup job (future enhancement)

---

## Timeline & Resources

### Estimated Effort

| Phase | Task | Time | Owner |
|-------|------|------|-------|
| 1 | Update storage.bicep | 15 min | Infrastructure Team |
| 1 | Update main.bicep outputs | 5 min | Infrastructure Team |
| 1 | Test Bicep compilation | 5 min | Infrastructure Team |
| 2 | Review what-if analysis | 10 min | Team Lead |
| 3 | Deploy infrastructure | 15 min | Infrastructure Team |
| 3 | Verify deployment | 15 min | Infrastructure Team |
| 4 | Application smoke tests | 30 min | QA Team |
| 4 | Monitoring setup | 15 min | DevOps Team |

**Total Time**: ~2 hours (including review and verification)

### Maintenance Window

**Deployment Type**: Zero-downtime deployment
- Adding a blob container does not require application restart
- Existing Data Protection functionality unaffected
- Application continues serving requests during deployment

**Recommended Window**: Anytime (no downtime required)

---

## Success Criteria

### Deployment Success
- ✅ Bicep deployment completes without errors
- ✅ `player-pictures` container exists in storage account
- ✅ Container configuration matches requirements (publicAccess: None)
- ✅ Existing `dataprotection-keys` container unaffected

### Application Success
- ✅ Application starts without errors
- ✅ `BlobStorageInitializationService` logs successful initialization
- ✅ Users can upload player pictures
- ✅ Uploaded pictures display in UI
- ✅ Users can delete player pictures
- ✅ SAS tokens generate correctly

### Operational Success
- ✅ No increase in error rate (Application Insights)
- ✅ Storage costs within expected range ($0.02-$0.25/month)
- ✅ Monitoring and alerts configured
- ✅ Documentation updated

---

## References

### Related Documents
- **Blob Storage Integration Specification**: `docs/specs/Blob_Storage_Integration_Specification.md`
- **Player Picture Upload Feature Spec**: `docs/specs/PlayerPictureUpload_Feature_Specification.md`
- **Infrastructure High-Level Design**: `docs/infra/high-level.md`
- **Infrastructure Implementation Plan**: `docs/infra/implementation-plan.md`
- **Bicep Best Practices**: `.github/instructions/bicep-code-best-practices.instructions.md`

### Azure Documentation
- **Azure Blob Storage**: https://learn.microsoft.com/azure/storage/blobs/
- **Bicep Storage Account**: https://learn.microsoft.com/azure/templates/microsoft.storage/storageaccounts
- **Bicep Blob Containers**: https://learn.microsoft.com/azure/templates/microsoft.storage/storageaccounts/blobservices/containers
- **Storage Account Pricing**: https://azure.microsoft.com/pricing/details/storage/blobs/
- **Managed Identity with Storage**: https://learn.microsoft.com/azure/storage/common/storage-auth-aad-msi

### Internal Resources
- **Current Infrastructure**: `infra/main.bicep`, `infra/modules/storage.bicep`
- **Deployment Scripts**: `infra/scripts/deploy-infra.ps1`
- **Application Configuration**: `src/GhcSamplePs.Web/appsettings.json`

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-12-29 | GitHub Copilot | Initial plan for player photo storage deployment |

---

## Approval & Sign-off

- [ ] **Technical Review**: Infrastructure changes reviewed and approved
- [ ] **Security Review**: Security implications assessed and approved
- [ ] **Cost Review**: Budget impact approved
- [ ] **Stakeholder Sign-off**: Product owner approves deployment

---

**Next Steps**:
1. Review this plan with team
2. Approve budget for additional storage costs
3. Schedule deployment (anytime - zero downtime)
4. Execute Phase 1-4 as outlined above
5. Monitor application for 24 hours post-deployment
6. Update this document with actual results and lessons learned
