// Monitoring Module - Log Analytics Workspace and Application Insights
// Purpose: Centralized logging and application performance monitoring for Container Apps

@description('Azure region for all resources')
param location string

@description('Name of the Log Analytics Workspace')
@minLength(4)
@maxLength(63)
param logAnalyticsName string

@description('Name of the Application Insights instance')
@minLength(1)
@maxLength(260)
param appInsightsName string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

// Log Analytics Workspace - Free tier configuration for development
// SKU: PerGB2018 with 500MB/day ingestion (free tier)
// Retention: 30 days (free tier limit)
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
    workspaceCapping: {
      dailyQuotaGb: -1 // No daily cap for free tier (500MB/day is automatic)
    }
  }
}

// Application Insights - Linked to Log Analytics workspace
// Sampling enabled at 10% for development to reduce costs
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
    SamplingPercentage: 10 // 10% sampling for dev to reduce costs
  }
}

// Outputs for consumption by other modules
@description('Resource ID of the Log Analytics Workspace')
output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id

@description('Instrumentation key for Application Insights')
output appInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey

@description('Connection string for Application Insights')
output appInsightsConnectionString string = applicationInsights.properties.ConnectionString
