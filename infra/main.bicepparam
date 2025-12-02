using './main.bicep'

// Azure region for all resources
param location = 'canadacentral'

// Environment name (dev/prod)
param environment = 'dev'

// Application name used in resource naming
param appName = 'ghcsampleps'

// Entra ID admin email for SQL Server - REQUIRED
// Replace with your Entra ID admin email
param sqlAdminEntraId = 'your-email@domain.com'

// Entra ID admin object ID for SQL Server - REQUIRED
// Get from Azure Portal > Microsoft Entra ID > Users > Your User > Object ID
param sqlAdminObjectId = 'your-entra-object-id'

// Entra ID client ID for authentication - REQUIRED
// Get from Azure Portal > Microsoft Entra ID > App Registrations > Your App > Application (client) ID
param entraIdClientId = 'your-app-client-id'

// Entra ID tenant ID for authentication - REQUIRED
// Get from Azure Portal > Microsoft Entra ID > Overview > Tenant ID
param entraIdTenantId = 'your-tenant-id'

// Optional: Docker image to deploy
// Leave empty for initial deployment (uses hello-world placeholder)
// After building your image, update with: 'yourregistry.azurecr.io/ghcsampleps-web:latest'
param containerImage = ''

// Optional: Developer IP ranges for SQL firewall access
// Example: [{ startIpAddress: '192.168.1.1', endIpAddress: '192.168.1.255' }]
param allowedIpRanges = []
