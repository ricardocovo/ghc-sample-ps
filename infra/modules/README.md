# Infrastructure Modules

This directory contains reusable Bicep modules for deploying Azure infrastructure resources.

## Modules

### monitoring.bicep

Log Analytics Workspace and Application Insights for centralized logging and monitoring.

**Purpose**: Centralized logging and application performance monitoring for Container Apps.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `logAnalyticsName` | string | Log Analytics Workspace name (4-63 chars) |
| `appInsightsName` | string | Application Insights name (1-260 chars) |
| `environment` | string | Environment tag (dev/prod) |

| Output | Type | Description |
|--------|------|-------------|
| `logAnalyticsWorkspaceId` | string | Resource ID of the Log Analytics Workspace |
| `logAnalyticsCustomerId` | string | Customer ID (Workspace ID) for Log Analytics |
| `logAnalyticsSharedKey` | string | Shared key for Container Apps Environment |
| `appInsightsInstrumentationKey` | string | Instrumentation key for Application Insights |
| `appInsightsConnectionString` | string | Connection string for Application Insights |

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

### sql.bicep

Azure SQL Server and Serverless Database with Managed Identity authentication.

**Purpose**: Database infrastructure for the application with Entra ID authentication.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `sqlServerName` | string | SQL Server name (1-63 chars, globally unique) |
| `databaseName` | string | Database name (1-128 chars) |
| `environment` | string | Environment tag (dev/prod) |
| `sqlAdminEntraId` | string | Entra ID admin email address |
| `sqlAdminObjectId` | string | Entra ID admin object ID |
| `allowedIpRanges` | array | Developer IP ranges for firewall access |

| Output | Type | Description |
|--------|------|-------------|
| `sqlServerFqdn` | string | Fully qualified domain name of the SQL Server |
| `databaseName` | string | Name of the database |
| `sqlServerId` | string | Resource ID of the SQL Server |
| `connectionStringFormat` | string | Connection string format for Managed Identity |

### containerregistry.bicep

Azure Container Registry for Docker images.

**Purpose**: Host Docker images for the Blazor Server application in Container Apps.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `registryName` | string | Registry name (5-50 chars, alphanumeric, globally unique) |
| `environment` | string | Environment tag (dev/prod) |

| Output | Type | Description |
|--------|------|-------------|
| `registryName` | string | Name of the Container Registry |
| `loginServer` | string | Login server URL (e.g., myregistry.azurecr.io) |
| `registryId` | string | Resource ID for RBAC assignments |

### containerapp.bicep

Container Apps Environment and Container App for Blazor Server hosting.

**Purpose**: Hosting for Blazor Server application with scale-to-zero capability and session affinity.

| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | string | Azure region for all resources |
| `environmentName` | string | Container Apps Environment name (1-64 chars) |
| `appName` | string | Container App name (1-32 chars) |
| `logAnalyticsCustomerId` | string | Log Analytics Workspace customer ID |
| `logAnalyticsSharedKey` | string (secure) | Log Analytics shared key |
| `containerImage` | string | Docker image to deploy (full path with tag) |
| `registryServer` | string | ACR login server |
| `sqlConnectionString` | string (secure) | SQL Server connection string |
| `entraIdTenantId` | string | Entra ID tenant ID |
| `entraIdClientId` | string | Entra ID client ID |
| `keyVaultUri` | string | Key Vault URI |
| `blobEndpoint` | string | Storage blob endpoint |
| `environment` | string | ASP.NET Core environment (Development/Production) |

| Output | Type | Description |
|--------|------|-------------|
| `containerAppFqdn` | string | Public FQDN for the Container App |
| `containerAppIdentityPrincipalId` | string | Managed Identity Principal ID |
| `containerAppId` | string | Resource ID of the Container App |

## Usage

### Build Modules

```bash
# Build all modules
bicep build infra/modules/monitoring.bicep
bicep build infra/modules/storage.bicep
bicep build infra/modules/keyvault.bicep
bicep build infra/modules/sql.bicep
bicep build infra/modules/containerregistry.bicep
bicep build infra/modules/containerapp.bicep

# Lint all modules
bicep lint infra/modules/monitoring.bicep
bicep lint infra/modules/storage.bicep
```

### Deploy with Main Orchestration Template

```bash
# Deploy using main.bicep with parameters file
az deployment group create \
  --resource-group <resource-group-name> \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

### Example: Reference in Main Bicep

```bicep
module monitoring 'modules/monitoring.bicep' = {
  name: 'monitoringDeployment'
  params: {
    location: location
    logAnalyticsName: '${appName}-logs-${environment}'
    appInsightsName: '${appName}-ai-${environment}'
    environment: environment
  }
}

module keyVault 'modules/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    location: location
    keyVaultName: '${appName}-kv-${environment}'
    environment: environment
    tenantId: tenantId
  }
}

module storage 'modules/storage.bicep' = {
  name: 'storageDeployment'
  params: {
    location: location
    storageAccountName: '${appName}st${environment}'
    environment: environment
  }
}
```
