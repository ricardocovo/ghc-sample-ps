// Main orchestration template for GhcSamplePs infrastructure deployment
// Purpose: Deploys all Azure resources for the Blazor Server application
// Environment: Development (scale-to-zero, cost-optimized)

targetScope = 'resourceGroup'

// ==============================================================================
// Parameters
// ==============================================================================

@description('Azure region for all resources')
param location string = 'canadacentral'

@description('Environment name for resource tagging and naming')
@allowed([
  'dev'
  'prod'
])
param environment string = 'dev'

@description('Application name used for resource naming convention')
@minLength(1)
@maxLength(20)
param appName string = 'ghcsampleps'

@description('Entra ID admin email address for SQL Server authentication')
param sqlAdminEntraId string

@description('Entra ID admin object ID for SQL Server authentication')
param sqlAdminObjectId string

@description('Entra ID client ID for application authentication')
param entraIdClientId string

@description('Entra ID tenant ID for application authentication')
param entraIdTenantId string

// ==============================================================================
// Variables - Resource Naming Convention: {appName}-{resource}-{environment}
// ==============================================================================

var resourcePrefix = '${appName}-${environment}'
var logAnalyticsName = '${resourcePrefix}-log'
var appInsightsName = '${resourcePrefix}-ai'
var storageAccountName = replace('${appName}${environment}st', '-', '')
// Key Vault name must be 3-24 characters, globally unique
// Truncate appName to max 18 chars to ensure total length <24 with 'dev'/'prod' (3-4 chars) + 'kv' (2 chars)
var keyVaultName = '${take(appName, 18)}${environment}kv'
var sqlServerName = '${resourcePrefix}-sql'
var databaseName = '${appName}db'
var registryName = replace('${appName}${environment}acr', '-', '')
var containerAppEnvironmentName = '${resourcePrefix}-cae'
var containerAppName = '${resourcePrefix}-app'

// ==============================================================================
// Resources - Log Analytics Workspace (needed for listKeys at deploy time)
// ==============================================================================

// Log Analytics Workspace - deployed directly to enable listKeys for Container Apps
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  tags: {
    environment: environment
  }
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

// Application Insights - Linked to Log Analytics workspace
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  tags: {
    environment: environment
  }
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    SamplingPercentage: 10
  }
}

// ==============================================================================
// Modules - Infrastructure Components
// ==============================================================================

// Storage: Storage Account for Data Protection keys
module storage 'modules/storage.bicep' = {
  name: 'storage-deployment'
  params: {
    location: location
    storageAccountName: storageAccountName
    environment: environment
  }
}

// Key Vault: Secrets management and Data Protection key encryption
module keyVault 'modules/keyvault.bicep' = {
  name: 'keyvault-deployment'
  params: {
    location: location
    keyVaultName: keyVaultName
    environment: environment
    tenantId: entraIdTenantId
  }
}

// SQL: Azure SQL Server and Serverless Database
module sql 'modules/sql.bicep' = {
  name: 'sql-deployment'
  params: {
    location: location
    sqlServerName: sqlServerName
    databaseName: databaseName
    environment: environment
    sqlAdminEntraId: sqlAdminEntraId
    sqlAdminObjectId: sqlAdminObjectId
  }
}

// Container Registry: Docker image hosting
module containerRegistry 'modules/containerregistry.bicep' = {
  name: 'containerregistry-deployment'
  params: {
    location: location
    registryName: registryName
    environment: environment
  }
}

// Container App: Blazor Server application hosting
// Note: Using placeholder image initially. Deploy actual application image separately after infrastructure is ready.
module containerApp 'modules/containerapp.bicep' = {
  name: 'containerapp-deployment'
  params: {
    location: location
    environmentName: containerAppEnvironmentName
    appName: containerAppName
    logAnalyticsCustomerId: logAnalyticsWorkspace.properties.customerId
    logAnalyticsSharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
    containerImage: '${containerRegistry.outputs.loginServer}/${appName}-web:latest'
    registryServer: containerRegistry.outputs.loginServer
    sqlConnectionString: sql.outputs.connectionStringFormat
    entraIdTenantId: entraIdTenantId
    entraIdClientId: entraIdClientId
    keyVaultUri: keyVault.outputs.keyVaultUri
    blobEndpoint: storage.outputs.blobEndpoint
    environment: 'Development'
  }
}

// ==============================================================================
// RBAC Assignments - Grant Managed Identity access to resources
// Note: Role assignments use deterministic names based on resource names
// ==============================================================================

// Storage Blob Data Contributor role for Container App Managed Identity
var storageBlobDataContributorRoleId = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'

resource storageRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, storageAccountName, containerAppName, storageBlobDataContributorRoleId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      storageBlobDataContributorRoleId
    )
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Key Vault Secrets User role for Container App Managed Identity
var keyVaultSecretsUserRoleId = '4633458b-17de-408a-b874-0445c86b69e6'

resource keyVaultSecretsRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, keyVaultName, containerAppName, keyVaultSecretsUserRoleId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultSecretsUserRoleId)
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Key Vault Crypto User role for Container App Managed Identity
var keyVaultCryptoUserRoleId = '12338af0-0e69-4776-bea7-57ae8d297424'

resource keyVaultCryptoRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, keyVaultName, containerAppName, keyVaultCryptoUserRoleId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', keyVaultCryptoUserRoleId)
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// ACR Pull role for Container App Managed Identity
var acrPullRoleId = '7f951dda-4ed3-4680-a7ca-43fe172d538d'

resource acrRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, registryName, containerAppName, acrPullRoleId)
  scope: resourceGroup()
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleId)
    principalId: containerApp.outputs.containerAppIdentityPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// ==============================================================================
// Outputs
// ==============================================================================

@description('Public URL for the Container App')
output containerAppUrl string = 'https://${containerApp.outputs.containerAppFqdn}'

@description('Fully qualified domain name of the SQL Server')
output sqlServerFqdn string = sql.outputs.sqlServerFqdn

@description('Database name')
output databaseName string = sql.outputs.databaseName

@description('Container Registry login server')
output registryLoginServer string = containerRegistry.outputs.loginServer

@description('Application Insights instrumentation key')
output appInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey

@description('Key Vault URI')
output keyVaultUri string = keyVault.outputs.keyVaultUri
