// ACR RBAC Module - Assigns AcrPull role to a principal for Container Registry access
// Purpose: Allow Container App managed identity to pull Docker images from ACR

@description('Name of the Container Registry')
param registryName string

@description('Principal ID to grant AcrPull access (e.g., managed identity)')
param principalId string

@description('Role Definition ID (use AcrPull role ID)')
param roleDefinitionId string

// Reference the existing Container Registry
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: registryName
}

// Assign AcrPull role to the principal
resource roleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(containerRegistry.id, principalId, roleDefinitionId)
  scope: containerRegistry
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleDefinitionId)
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}

// Outputs
@description('Role assignment ID')
output roleAssignmentId string = roleAssignment.id

@description('Role assignment name')
output roleAssignmentName string = roleAssignment.name
