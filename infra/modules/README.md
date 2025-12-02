# Infrastructure Modules

This directory contains reusable Bicep modules for deploying Azure infrastructure resources.

## Modules

### storage.bicep

Storage Account and Blob Container for ASP.NET Core Data Protection.

**Purpose**: Persistent storage for Data Protection keys shared across Container App instances.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `storageAccountName` | string | Storage Account name (3-24 chars, globally unique) |
| `environment` | string | Environment tag (dev/prod) |

| Output | Type | Description |
|--------|------|-------------|
| `storageAccountName` | string | Name of the Storage Account |
| `blobEndpoint` | string | Blob service endpoint URL |
| `storageAccountId` | string | Resource ID for RBAC assignments |

### keyvault.bicep

Azure Key Vault for secrets management and Data Protection key encryption.

**Purpose**: Encrypt Data Protection keys, store secrets for Azure Container Apps.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `keyVaultName` | string | Key Vault name (3-24 chars, globally unique) |
| `environment` | string | Environment tag (dev/prod) |
| `tenantId` | string | Azure AD tenant ID for Key Vault access |

| Output | Type | Description |
|--------|------|-------------|
| `keyVaultName` | string | Name of the Key Vault |
| `keyVaultUri` | string | Vault URI for Data Protection configuration |
| `keyVaultId` | string | Resource ID for RBAC assignments |

**Security Configuration**:
- SKU: Standard
- Soft delete: Enabled (90 days retention)
- Purge protection: Enabled
- Authorization: RBAC mode (not access policies)
- Minimum TLS: 1.2

**Required RBAC Roles** for Container App Managed Identity:
- `Key Vault Secrets User` - Read secrets
- `Key Vault Crypto User` - Use keys for encryption/decryption

## Usage

### Build Modules

```bash
# Build all modules
bicep build infra/modules/storage.bicep
bicep build infra/modules/keyvault.bicep

# Lint all modules
bicep lint infra/modules/storage.bicep
bicep lint infra/modules/keyvault.bicep
```

### Example: Reference in Main Bicep

```bicep
module keyVault 'modules/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    location: location
    keyVaultName: 'kv-${appName}-${environment}'
    environment: environment
    tenantId: tenantId
  }
}

module storage 'modules/storage.bicep' = {
  name: 'storageDeployment'
  params: {
    location: location
    storageAccountName: 'st${appName}${environment}'
    environment: environment
  }
}
```
