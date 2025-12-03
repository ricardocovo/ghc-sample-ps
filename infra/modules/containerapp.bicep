// Container Apps Module - Container Apps Environment and Container App for Blazor Server
// Purpose: Hosting for Blazor Server application with scale-to-zero capability and session affinity

@description('Azure region for all resources')
param location string

@description('Name of the Container Apps Environment')
@minLength(1)
@maxLength(64)
param environmentName string

@description('Name of the Container App')
@minLength(1)
@maxLength(32)
param appName string

@description('Log Analytics Workspace customer ID (GUID)')
param logAnalyticsCustomerId string

@description('Log Analytics Workspace shared key')
@secure()
param logAnalyticsSharedKey string

@description('Docker image to deploy (full path including tag)')
param containerImage string

@description('Azure Container Registry login server (e.g., myregistry.azurecr.io)')
param registryServer string

@description('SQL Server connection string')
@secure()
param sqlConnectionString string

@description('Entra ID tenant ID for authentication')
@secure()
param entraIdTenantId string

@description('Entra ID client ID for authentication')
@secure()
param entraIdClientId string

@description('Key Vault URI for secrets and data protection')
@secure()
param keyVaultUri string

@description('Storage blob endpoint for data protection keys')
@secure()
param blobEndpoint string

@description('Application Insights connection string for telemetry')
@secure()
param appInsightsConnectionString string = ''

@description('Environment name for ASP.NET Core')
@allowed([
  'Development'
  'Staging'
  'Production'
  'Test'
])
param environment string = 'Development'

// Container Apps Environment - Managed environment for hosting container apps
// Zone redundancy: Disabled (single zone for dev)
// Internal load balancer: false (public ingress)
resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: environmentName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsCustomerId
        sharedKey: logAnalyticsSharedKey
      }
    }
    zoneRedundant: false
  }
}

// Container App - Blazor Server application
// - System-assigned Managed Identity
// - Scale-to-zero configuration
// - Session affinity for Blazor Server SignalR connections
resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: appName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
        // Session affinity: sticky - CRITICAL for Blazor Server
        // Blazor Server uses SignalR for real-time communication
        // All requests from a client must reach the same instance
        stickySessions: {
          affinity: 'sticky'
        }
      }
      registries: [
        {
          server: registryServer
          identity: 'system'
        }
      ]
      secrets: concat([
        {
          name: 'connection-strings-default'
          value: sqlConnectionString
        }
        {
          name: 'azure-ad-tenant-id'
          value: entraIdTenantId
        }
        {
          name: 'azure-ad-client-id'
          value: entraIdClientId
        }
        {
          name: 'keyvault-uri'
          value: keyVaultUri
        }
        {
          name: 'storage-blob-endpoint'
          value: blobEndpoint
        }
      ], !empty(appInsightsConnectionString) ? [
        {
          name: 'appinsights-connection-string'
          value: appInsightsConnectionString
        }
      ] : [])
    }
    template: {
      containers: [
        {
          name: '${appName}-container'
          image: containerImage
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
          env: concat([
            {
              name: 'ConnectionStrings__DefaultConnection'
              secretRef: 'connection-strings-default'
            }
            {
              name: 'AzureAd__TenantId'
              secretRef: 'azure-ad-tenant-id'
            }
            {
              name: 'AzureAd__ClientId'
              secretRef: 'azure-ad-client-id'
            }
            {
              name: 'KeyVault__VaultUri'
              secretRef: 'keyvault-uri'
            }
            {
              name: 'Storage__BlobEndpoint'
              secretRef: 'storage-blob-endpoint'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: environment
            }
          ], !empty(appInsightsConnectionString) ? [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              secretRef: 'appinsights-connection-string'
            }
          ] : [])
        }
      ]
      scale: {
        // Scale-to-zero for cost optimization in development
        minReplicas: 0
        maxReplicas: 1
        rules: [
          {
            name: 'http-rule'
            http: {
              metadata: {
                concurrentRequests: '10'
              }
            }
          }
        ]
      }
    }
  }
}

// Outputs for consumption by other modules
@description('Public FQDN for the Container App')
output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn

@description('Managed Identity Principal ID for RBAC assignments')
output containerAppIdentityPrincipalId string = containerApp.identity.principalId

@description('Resource ID of the Container App')
output containerAppId string = containerApp.id
