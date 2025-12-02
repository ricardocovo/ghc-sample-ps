// Main Bicep Orchestration Template
// Purpose: Deploy all infrastructure modules and configure RBAC assignments for GhcSamplePs
// Orchestrates: monitoring, storage, keyvault, sql, containerregistry, containerapp

targetScope = 'resourceGroup'

// =============================================================================
// PARAMETERS
// =============================================================================

@description('Azure region for all resources')
param location string = 'canadacentral'

@description('Environment name for resource tagging and naming')
@allowed([
  'dev'
  'prod'
])
param environment string = 'dev'

@description('Application name used in resource naming convention')
@minLength(3)
@maxLength(15)
param appName string = 'ghcsampleps'

@description('Entra ID admin email address for SQL Server')
param sqlAdminEntraId string

@description('Entra ID admin object ID for SQL Server')
param sqlAdminObjectId string

@description('Entra ID client ID for authentication')
param entraIdClientId string

@description('Entra ID tenant ID for authentication')
param entraIdTenantId string

@description('Docker image to deploy (optional for initial deployment)')
param containerImage string = ''

@description('Developer IP ranges for SQL firewall access')
param allowedIpRanges array = []

// =============================================================================
// VARIABLES - Resource Naming Convention: {appName}-{resource}-{environment}
// =============================================================================

// Monitoring resources
var logAnalyticsName = '${appName}-logs-${environment}'
var appInsightsName = '${appName}-ai-${environment}'

// Storage - must be lowercase alphanumeric, 3-24 chars
var storageAccountName = toLower(replace('${appName}st${environment}', '-', ''))

// Key Vault - alphanumeric and hyphens, 3-24 chars
var keyVaultName = '${appName}-kv-${environment}'

// SQL Server - lowercase alphanumeric and hyphens
var sqlServerName = toLower('${appName}-sql-${environment}')
var databaseName = '${appName}db'

// Container Registry - alphanumeric only, 5-50 chars
var registryName = toLower(replace('${appName}acr${environment}', '-', ''))

// Container Apps
var containerAppsEnvironmentName = '${appName}-env-${environment}'
var containerAppName = '${appName}-app-${environment}'

// Default container image when none provided
var defaultContainerImage = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
var deployImage = empty(containerImage) ? defaultContainerImage : containerImage

// ASP.NET Core environment mapping
var aspNetEnvironment = environment == 'prod' ? 'Production' : 'Development'

// =============================================================================
// BUILT-IN ROLE DEFINITIONS
// =============================================================================

// Key Vault Secrets User - Read secret contents
var keyVaultSecretsUserRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')

// Key Vault Crypto User - Use keys for cryptographic operations
var keyVaultCryptoUserRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '12338af0-0e69-4776-bea7-57ae8d297424')

// Storage Blob Data Contributor - Read, write, delete blob data
var storageBlobDataContributorRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')

// AcrPull - Pull images from container registry
var acrPullRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')

// =============================================================================
// MODULE DEPLOYMENTS
// =============================================================================

// 1. Monitoring - Log Analytics Workspace and Application Insights
module monitoring 'modules/monitoring.bicep' = {
  name: 'monitoringDeployment'
  params: {
    location: location
    logAnalyticsName: logAnalyticsName
    appInsightsName: appInsightsName
    environment: environment
  }
}

// 2. Storage Account - For Data Protection keys
module storage 'modules/storage.bicep' = {
  name: 'storageDeployment'
  params: {
    location: location
    storageAccountName: storageAccountName
    environment: environment
  }
}

// 3. Key Vault - For secrets and Data Protection key encryption
module keyVault 'modules/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    location: location
    keyVaultName: keyVaultName
    environment: environment
    tenantId: entraIdTenantId
  }
}

// 4. SQL Server and Database
module sql 'modules/sql.bicep' = {
  name: 'sqlDeployment'
  params: {
    location: location
    sqlServerName: sqlServerName
    databaseName: databaseName
    environment: environment
    sqlAdminEntraId: sqlAdminEntraId
    sqlAdminObjectId: sqlAdminObjectId
    allowedIpRanges: allowedIpRanges
  }
}

// 5. Container Registry
module containerRegistry 'modules/containerregistry.bicep' = {
  name: 'containerRegistryDeployment'
  params: {
    location: location
    registryName: registryName
    environment: environment
  }
}

// 6. Container Apps Environment and Container App
module containerApp 'modules/containerapp.bicep' = {
  name: 'containerAppDeployment'
  params: {
    location: location
    environmentName: containerAppsEnvironmentName
    appName: containerAppName
    logAnalyticsCustomerId: monitoring.outputs.logAnalyticsCustomerId
    logAnalyticsSharedKey: monitoring.outputs.logAnalyticsSharedKey
    containerImage: deployImage
    registryServer: containerRegistry.outputs.loginServer
    sqlConnectionString: sql.outputs.connectionStringFormat
    entraIdTenantId: entraIdTenantId
    entraIdClientId: entraIdClientId
    keyVaultUri: keyVault.outputs.keyVaultUri
    blobEndpoint: storage.outputs.blobEndpoint
    environment: aspNetEnvironment
  }
}

// =============================================================================
// RBAC ROLE ASSIGNMENTS - Container App Managed Identity
// Note: Using resource names for deterministic GUID generation
// =============================================================================

// Key Vault Secrets User - Read secrets
resource keyVaultSecretsUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, keyVaultName, containerAppName, 'KeyVaultSecretsUser')
  scope: keyVaultResource
  properties: {
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    roleDefinitionId: keyVaultSecretsUserRoleId
    principalType: 'ServicePrincipal'
  }
}

// Key Vault Crypto User - Use keys for Data Protection encryption
resource keyVaultCryptoUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, keyVaultName, containerAppName, 'KeyVaultCryptoUser')
  scope: keyVaultResource
  properties: {
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    roleDefinitionId: keyVaultCryptoUserRoleId
    principalType: 'ServicePrincipal'
  }
}

// Storage Blob Data Contributor - Access Data Protection keys blob
resource storageBlobContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, storageAccountName, containerAppName, 'StorageBlobDataContributor')
  scope: storageAccountResource
  properties: {
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    roleDefinitionId: storageBlobDataContributorRoleId
    principalType: 'ServicePrincipal'
  }
}

// ACR Pull - Pull container images
resource acrPullAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, registryName, containerAppName, 'AcrPull')
  scope: containerRegistryResource
  properties: {
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    roleDefinitionId: acrPullRoleId
    principalType: 'ServicePrincipal'
  }
}

// =============================================================================
// EXISTING RESOURCE REFERENCES FOR RBAC SCOPING
// =============================================================================

resource keyVaultResource 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
  dependsOn: [
    keyVault
  ]
}

resource storageAccountResource 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
  dependsOn: [
    storage
  ]
}

resource containerRegistryResource 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: registryName
  dependsOn: [
    containerRegistry
  ]
}

// =============================================================================
// OUTPUTS
// =============================================================================

@description('Public FQDN of the Container App')
output containerAppUrl string = containerApp.outputs.containerAppFqdn

@description('Fully qualified domain name of the SQL Server')
output sqlServerFqdn string = sql.outputs.sqlServerFqdn

@description('Name of the database')
output databaseName string = sql.outputs.databaseName

@description('URI of the Key Vault')
output keyVaultUri string = keyVault.outputs.keyVaultUri

@description('Name of the Storage Account')
output storageAccountName string = storage.outputs.storageAccountName

@description('Login server URL for the Container Registry')
output registryLoginServer string = containerRegistry.outputs.loginServer

@description('Managed Identity Principal ID for additional RBAC assignments')
output containerAppIdentityId string = containerApp.outputs.containerAppIdentityPrincipalId
