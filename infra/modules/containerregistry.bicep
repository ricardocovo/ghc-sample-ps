// Container Registry Module - Azure Container Registry for Docker images
// Purpose: Host Docker images for the Blazor Server application in Container Apps

@description('Azure region for all resources')
param location string

@description('Name of the Container Registry (must be globally unique, alphanumeric only)')
@minLength(5)
@maxLength(50)
param registryName string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

// Azure Container Registry - Basic SKU configuration for development
// SKU: Basic (~$5/month, sufficient for dev workloads)
// Admin: Disabled (use Managed Identity for authentication)
// Public network access: Enabled (required for Container Apps pull)
// Anonymous pull: Disabled (authentication required)
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: registryName
  location: location
  tags: {
    environment: environment
  }
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
    publicNetworkAccess: 'Enabled'
    // Note: Anonymous pull is disabled by default for Basic SKU
    // Trust policy and retention policy are only available in Standard/Premium SKUs
  }
}

// Outputs for consumption by other modules
@description('Name of the Container Registry')
output registryName string = containerRegistry.name

@description('Login server URL for the Container Registry (e.g., myregistry.azurecr.io)')
output loginServer string = containerRegistry.properties.loginServer

@description('Resource ID of the Container Registry for RBAC assignments')
output registryId string = containerRegistry.id
