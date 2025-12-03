// Monitoring Alerts Module - Alert rules for authentication, authorization, and Entra ID connectivity
// Purpose: Configure Azure Monitor alerts for security and authentication events

@description('Azure region for all resources')
param location string

@description('Resource ID of the Application Insights resource')
param appInsightsId string

@description('Name prefix for alert rules')
@minLength(1)
@maxLength(50)
param alertNamePrefix string

@description('Email address for alert notifications')
param alertEmailAddress string

@description('Environment tag for resource organization')
@allowed([
  'dev'
  'prod'
])
param environment string

// Action group for alert notifications
resource actionGroup 'Microsoft.Insights/actionGroups@2023-01-01' = {
  name: '${alertNamePrefix}-auth-alerts-ag'
  location: 'global'
  tags: {
    environment: environment
  }
  properties: {
    groupShortName: 'AuthAlerts'
    enabled: true
    emailReceivers: [
      {
        name: 'AdminEmail'
        emailAddress: alertEmailAddress
        useCommonAlertSchema: true
      }
    ]
  }
}

// Alert rule for authentication failures
// Triggers when AuthenticationFailure event count exceeds threshold within time window
resource authFailureAlert 'Microsoft.Insights/scheduledQueryRules@2023-03-15-preview' = {
  name: '${alertNamePrefix}-auth-failure-alert'
  location: location
  tags: {
    environment: environment
  }
  properties: {
    displayName: 'Authentication Failures Alert'
    description: 'Alert when authentication failures exceed threshold. Indicates potential unauthorized access attempts or Entra ID configuration issues.'
    severity: 2 // Warning
    enabled: true
    evaluationFrequency: 'PT5M' // Every 5 minutes
    scopes: [
      appInsightsId
    ]
    targetResourceTypes: [
      'Microsoft.Insights/components'
    ]
    windowSize: 'PT15M' // 15 minute window
    criteria: {
      allOf: [
        {
          query: '''
customEvents
| where name == "AuthenticationFailure"
| summarize FailureCount = count() by bin(timestamp, 5m)
| where FailureCount > 5
'''
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    autoMitigate: true
    actions: {
      actionGroups: [
        actionGroup.id
      ]
    }
  }
}

// Alert rule for authorization failures
// Triggers when AuthorizationFailure event count exceeds threshold
resource authzFailureAlert 'Microsoft.Insights/scheduledQueryRules@2023-03-15-preview' = {
  name: '${alertNamePrefix}-authz-failure-alert'
  location: location
  tags: {
    environment: environment
  }
  properties: {
    displayName: 'Authorization Failures Alert'
    description: 'Alert when authorization failures exceed threshold. Indicates potential privilege escalation attempts or misconfigured permissions.'
    severity: 2 // Warning
    enabled: true
    evaluationFrequency: 'PT5M'
    scopes: [
      appInsightsId
    ]
    targetResourceTypes: [
      'Microsoft.Insights/components'
    ]
    windowSize: 'PT15M'
    criteria: {
      allOf: [
        {
          query: '''
customEvents
| where name == "AuthorizationFailure"
| summarize FailureCount = count() by bin(timestamp, 5m)
| where FailureCount > 10
'''
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    autoMitigate: true
    actions: {
      actionGroups: [
        actionGroup.id
      ]
    }
  }
}

// Alert rule for Entra ID connectivity failures
// Triggers on any Entra ID connectivity failure (critical)
resource entraIdConnectivityAlert 'Microsoft.Insights/scheduledQueryRules@2023-03-15-preview' = {
  name: '${alertNamePrefix}-entraid-connectivity-alert'
  location: location
  tags: {
    environment: environment
  }
  properties: {
    displayName: 'Entra ID Connectivity Failures Alert'
    description: 'Critical alert when Entra ID connectivity failures occur. Indicates authentication infrastructure issues.'
    severity: 1 // Error
    enabled: true
    evaluationFrequency: 'PT1M' // Every minute for critical alerts
    scopes: [
      appInsightsId
    ]
    targetResourceTypes: [
      'Microsoft.Insights/components'
    ]
    windowSize: 'PT5M' // 5 minute window
    criteria: {
      allOf: [
        {
          query: '''
customEvents
| where name == "EntraIdConnectivityFailure"
| summarize FailureCount = count() by bin(timestamp, 1m)
| where FailureCount >= 1
'''
          timeAggregation: 'Count'
          dimensions: []
          operator: 'GreaterThan'
          threshold: 0
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
        }
      ]
    }
    autoMitigate: true
    actions: {
      actionGroups: [
        actionGroup.id
      ]
    }
  }
}

// Outputs for reference
@description('Resource ID of the action group')
output actionGroupId string = actionGroup.id

@description('Resource ID of the authentication failure alert')
output authFailureAlertId string = authFailureAlert.id

@description('Resource ID of the authorization failure alert')
output authzFailureAlertId string = authzFailureAlert.id

@description('Resource ID of the Entra ID connectivity alert')
output entraIdConnectivityAlertId string = entraIdConnectivityAlert.id
