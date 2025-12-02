// Key Vault Module - Azure Key Vault for secrets management and Data Protection key encryption
// Purpose: Encrypt Data Protection keys, store secrets for Azure Container Apps

@description('Azure region for all resources')
param location string

@description('Name of the Key Vault (must be globally unique)')
@minLength(3)
@maxLength(24)
param keyVaultName string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

@description('Azure AD tenant ID for Key Vault access')
param tenantId string

// Key Vault - Standard tier with RBAC authorization
// SKU: Standard
// Security: Soft delete enabled, purge protection enabled, TLS 1.2 minimum
// Authorization: RBAC mode (not access policies)
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: {
    environment: environment
  }
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    publicNetworkAccess: 'Enabled'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Outputs for consumption by other modules and Program.cs configuration
@description('Name of the Key Vault')
output keyVaultName string = keyVault.name

@description('URI of the Key Vault for Data Protection configuration')
output keyVaultUri string = keyVault.properties.vaultUri

@description('Resource ID of the Key Vault for RBAC assignments')
output keyVaultId string = keyVault.id
