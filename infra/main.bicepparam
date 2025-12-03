// =============================================================================
// GhcSamplePs Development Environment Parameters
// =============================================================================
// This file contains the Bicep parameters for deploying the development
// environment infrastructure to Azure.
//
// BEFORE DEPLOYING: Update the placeholder values below with your specific
// Azure Entra ID and tenant configuration.
// =============================================================================

using './main.bicep'

// -----------------------------------------------------------------------------
// Location Configuration
// -----------------------------------------------------------------------------
// Azure region for all resources. Canada Central is used for development.
param location = 'canadacentral'

// -----------------------------------------------------------------------------
// Environment Configuration
// -----------------------------------------------------------------------------
// Environment identifier used in resource naming convention.
// Use 'dev' for development, 'prod' for production.
param environment = 'dev'

// -----------------------------------------------------------------------------
// Application Name
// -----------------------------------------------------------------------------
// Application identifier used in resource naming convention: {appName}-{resource}-{environment}
// This creates resources like: ghcsampleps-dev-sql, ghcsampleps-dev-app, etc.
param appName = 'ghcsampleps'

// -----------------------------------------------------------------------------
// SQL Server Entra ID Admin Configuration
// -----------------------------------------------------------------------------
// These parameters configure the Entra ID administrator for Azure SQL Server.
// This account will be the only admin for SQL Server (Entra ID-only authentication).
//
// HOW TO GET YOUR ENTRA ID EMAIL:
//   - Use your Azure portal login email (e.g., your-name@domain.com)
//   - For Microsoft Learn sandbox: your Microsoft account email
//
// HOW TO GET YOUR ENTRA ID OBJECT ID:
//   1. Go to Azure Portal (https://portal.azure.com)
//   2. Search for "Microsoft Entra ID" (or "Azure Active Directory")
//   3. Click "Users" in the left menu
//   4. Search for your user account
//   5. Click on your user to view details
//   6. Copy the "Object ID" value (a GUID like: 12345678-1234-1234-1234-123456789012)
//
//   Alternative method using Azure CLI:
//   az ad signed-in-user show --query id -o tsv
//
// PLACEHOLDER: Replace with your actual values
param sqlAdminEntraId = '<your-email@domain.com>'
param sqlAdminObjectId = '<your-entra-object-id>'

// -----------------------------------------------------------------------------
// Entra ID Application Registration Configuration
// -----------------------------------------------------------------------------
// These parameters configure the Entra ID authentication for the application.
// The values can be found in the existing app registration configured in appsettings.json.
//
// REFERENCE: See src/GhcSamplePs.Web/appsettings.json for existing configuration.
//
// HOW TO GET THE CLIENT ID:
//   1. Go to Azure Portal (https://portal.azure.com)
//   2. Search for "App registrations"
//   3. Find your application (e.g., GhcSamplePs)
//   4. Copy the "Application (client) ID" value
//
//   From appsettings.json - AzureAd:ClientId value
//
// HOW TO GET THE TENANT ID:
//   1. In the same App registration page
//   2. Copy the "Directory (tenant) ID" value
//
//   From appsettings.json - AzureAd:TenantId value
//
// EXISTING VALUES FROM appsettings.json:
//   Domain: MngEnvMCAP091123.onmicrosoft.com
//   TenantId: 66d8e3ac-39f0-4505-b4d8-2d91327ff764
//   ClientId: 3acf99ae-f2ed-49a6-b7eb-0e4067ba23e5
//
// PLACEHOLDER: Replace with your actual values or use the existing ones from appsettings.json
param entraIdClientId = '<your-app-client-id>'
param entraIdTenantId = '<your-tenant-id>'
