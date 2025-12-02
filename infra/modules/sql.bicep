// SQL Module - Azure SQL Server and Serverless Database
// Purpose: Database infrastructure with Managed Identity authentication for Container Apps

@description('Azure region for all resources')
param location string

@description('Name of the Azure SQL Server (must be globally unique)')
@minLength(1)
@maxLength(63)
param sqlServerName string

@description('Name of the database')
@minLength(1)
@maxLength(128)
param databaseName string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

@description('Entra ID admin email address')
param sqlAdminEntraId string

@description('Entra ID admin object ID')
param sqlAdminObjectId string

@description('Developer IP ranges for firewall access')
param allowedIpRanges array = []

// Azure SQL Server - Managed Identity enabled with Entra ID admin
// Public network access enabled with firewall rules
// Minimum TLS 1.2 for security
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: {
    environment: environment
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: sqlAdminEntraId
      sid: sqlAdminObjectId
      tenantId: subscription().tenantId
      principalType: 'User'
    }
  }
}

// Azure SQL Database - Serverless General Purpose
// SKU: GP_S_Gen5 (General Purpose Serverless)
// Auto-pause delay: 60 minutes
// Min capacity: 0.5 vCores, Max capacity: 2 vCores
// Storage: 32GB
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: {
    environment: environment
  }
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 2
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368 // 32GB
    autoPauseDelay: 60 // 60 minutes
    minCapacity: json('0.5') // 0.5 vCores minimum
    zoneRedundant: false
    requestedBackupStorageRedundancy: 'Local'
  }
}

// Firewall Rule - Allow Azure services access
// Required for Managed Identity connections from Container Apps
resource firewallRuleAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Firewall Rules - Developer IP ranges for access
// Created dynamically based on allowedIpRanges parameter
resource firewallRulesDevIps 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = [
  for (ipRange, index) in allowedIpRanges: {
    parent: sqlServer
    name: 'AllowedIpRange-${index}'
    properties: {
      startIpAddress: ipRange.startIpAddress
      endIpAddress: ipRange.endIpAddress
    }
  }
]

// Outputs for consumption by other modules
@description('Fully qualified domain name of the SQL Server')
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName

@description('Name of the database')
output databaseName string = sqlDatabase.name

@description('Resource ID of the SQL Server')
output sqlServerId string = sqlServer.id

@description('Connection string format for Managed Identity authentication')
output connectionStringFormat string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;'
