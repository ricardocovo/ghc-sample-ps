// Storage Module - Storage Account and Blob Container for ASP.NET Core Data Protection
// Purpose: Persistent storage for Data Protection keys shared across Container App instances

@description('Azure region for all resources')
param location string

@description('Name of the Storage Account (must be globally unique)')
@minLength(3)
@maxLength(24)
param storageAccountName string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

// Storage Account - Standard locally redundant storage for development
// SKU: Standard_LRS (locally redundant, lowest cost)
// Kind: StorageV2 (general-purpose v2)
// Security: HTTPS only, TLS 1.2 minimum
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  tags: {
    environment: environment
  }
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
    accessTier: 'Hot'
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

// Blob Services - Required for container creation
resource blobServices 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    deleteRetentionPolicy: {
      enabled: false
    }
  }
}

// Blob Container - Data Protection keys storage
// Name: dataprotection-keys
// Access: None (private, accessed via Managed Identity with RBAC)
resource dataProtectionContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobServices
  name: 'dataprotection-keys'
  properties: {
    publicAccess: 'None'
  }
}

// Outputs for consumption by other modules
@description('Name of the Storage Account')
output storageAccountName string = storageAccount.name

@description('Blob service endpoint URL')
output blobEndpoint string = storageAccount.properties.primaryEndpoints.blob

@description('Resource ID of the Storage Account for RBAC assignments')
output storageAccountId string = storageAccount.id
