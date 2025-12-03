# Application Insights Monitoring Guide

> **Production Monitoring Setup for Authentication and Authorization Events**

This document describes the Application Insights monitoring configuration for the GhcSamplePs application, including authentication event tracking, custom metrics, and alerting.

---

## Table of Contents

- [Overview](#overview)
- [Application Insights Configuration](#application-insights-configuration)
- [Authentication Event Tracking](#authentication-event-tracking)
- [Custom Metrics](#custom-metrics)
- [Alert Rules](#alert-rules)
- [Monitoring Dashboard](#monitoring-dashboard)
- [Kusto Queries](#kusto-queries)
- [Troubleshooting](#troubleshooting)

---

## Overview

The monitoring system tracks:

- **Authentication Events**: Login successes, failures, and sign-outs
- **Authorization Events**: Permission checks and access denials
- **Entra ID Connectivity**: Connection failures to Microsoft Entra ID (Azure AD)
- **Custom Metrics**: Counters for tracking authentication patterns

### Architecture

```
┌─────────────────┐     ┌────────────────────┐     ┌─────────────────┐
│   Blazor App    │────▶│ Authentication     │────▶│  Application    │
│                 │     │ Telemetry Service  │     │   Insights      │
└─────────────────┘     └────────────────────┘     └─────────────────┘
                                                           │
                                                           ▼
                                                   ┌─────────────────┐
                                                   │  Alert Rules    │
                                                   │  ─────────────  │
                                                   │ • Auth Failures │
                                                   │ • Authz Errors  │
                                                   │ • Entra ID Conn │
                                                   └─────────────────┘
                                                           │
                                                           ▼
                                                   ┌─────────────────┐
                                                   │  Action Group   │
                                                   │  (Email Alerts) │
                                                   └─────────────────┘
```

---

## Application Insights Configuration

### Web Application Setup

Application Insights is configured in `Program.cs`:

```csharp
// Configure Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.EnableAdaptiveSampling = true;
    options.EnableQuickPulseMetricStream = true;
});

// Register authentication telemetry service
builder.Services.AddScoped<IAuthenticationTelemetryService, AuthenticationTelemetryService>();
```

### Environment Variables

| Variable | Description | Source |
|----------|-------------|--------|
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | App Insights connection string | Container Apps secret |
| `ApplicationInsights:ConnectionString` | Alternative config key | appsettings.json |

### Infrastructure Setup

Application Insights is deployed via Bicep:

```bicep
// In main.bicep
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    IngestionMode: 'LogAnalytics'
    SamplingPercentage: 10  // 10% sampling for dev
  }
}
```

---

## Authentication Event Tracking

### Event Types

| Event Name | Description | Severity |
|------------|-------------|----------|
| `AuthenticationSuccess` | User successfully authenticated | Info |
| `AuthenticationFailure` | Authentication attempt failed | Warning |
| `AuthorizationSuccess` | Authorization check passed | Debug |
| `AuthorizationFailure` | Authorization check failed | Warning |
| `EntraIdConnectivityFailure` | Entra ID connection error | Error |
| `UserSignOut` | User signed out | Info |

### Event Properties

#### AuthenticationSuccess
- `UserId`: The authenticated user's object ID
- `AuthenticationMethod`: e.g., "OpenIdConnect"
- `EventTime`: UTC timestamp

#### AuthenticationFailure
- `Reason`: Error message or failure reason
- `AuthenticationMethod`: e.g., "OpenIdConnect"
- `EventTime`: UTC timestamp

#### AuthorizationFailure
- `UserId`: The user attempting access
- `Resource`: The resource being accessed
- `Policy`: The authorization policy that failed
- `Reason`: Why authorization was denied
- `EventTime`: UTC timestamp

#### EntraIdConnectivityFailure
- `Operation`: The operation that failed
- `ErrorMessage`: The error details
- `ExceptionType`: (if applicable) Exception type
- `ExceptionMessage`: (if applicable) Exception message
- `EventTime`: UTC timestamp

### Using the Telemetry Service

```csharp
public class SomeComponent
{
    private readonly IAuthenticationTelemetryService _telemetryService;

    public SomeComponent(IAuthenticationTelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    public void TrackCustomAuthEvent()
    {
        // Track a successful authentication
        _telemetryService.TrackAuthenticationSuccess("user-id", "CustomMethod");

        // Track a failed authentication
        _telemetryService.TrackAuthenticationFailure("Invalid credentials", "CustomMethod");

        // Track an authorization failure
        _telemetryService.TrackAuthorizationFailure(
            "user-id",
            "/admin/settings",
            "RequireAdminRole",
            "User is not in Admin role");
    }
}
```

---

## Custom Metrics

The telemetry service tracks the following metrics:

| Metric Name | Description | Aggregation |
|-------------|-------------|-------------|
| `AuthenticationSuccessCount` | Count of successful authentications | Sum |
| `AuthenticationFailureCount` | Count of failed authentications | Sum |
| `AuthorizationFailureCount` | Count of authorization failures | Sum |
| `EntraIdConnectivityFailureCount` | Count of Entra ID connection failures | Sum |

### Viewing Metrics

1. Navigate to Application Insights in Azure Portal
2. Select **Metrics** from the left menu
3. Choose `Custom Metrics` as the metric namespace
4. Select the desired metric

---

## Alert Rules

### Deployed Alerts

Three alert rules are configured in the `alerts.bicep` module:

#### 1. Authentication Failures Alert
- **Severity**: Warning (2)
- **Condition**: More than 5 authentication failures in 15 minutes
- **Evaluation**: Every 5 minutes

#### 2. Authorization Failures Alert
- **Severity**: Warning (2)
- **Condition**: More than 10 authorization failures in 15 minutes
- **Evaluation**: Every 5 minutes

#### 3. Entra ID Connectivity Alert
- **Severity**: Error (1)
- **Condition**: Any Entra ID connectivity failure
- **Evaluation**: Every 1 minute

### Enabling Alerts

Alerts are enabled by providing an email address in the deployment parameters:

```bicep
// In main.bicepparam
param alertEmailAddress = 'admin@contoso.com'
```

If `alertEmailAddress` is empty, the alerts module is not deployed.

### Alert Actions

All alerts send email notifications to the configured email address via an Action Group.

---

## Monitoring Dashboard

### Creating a Dashboard

1. Navigate to Azure Portal → **Dashboards**
2. Click **New dashboard**
3. Add the following tiles:

#### Authentication Overview
- **Type**: Metric chart
- **Resource**: Application Insights
- **Metrics**: 
  - `AuthenticationSuccessCount`
  - `AuthenticationFailureCount`

#### Recent Authentication Failures
- **Type**: Log Analytics query
- **Query**:
```kusto
customEvents
| where name == "AuthenticationFailure"
| where timestamp > ago(24h)
| project timestamp, Reason = tostring(customDimensions.Reason)
| order by timestamp desc
| take 10
```

#### Authorization Failures by Resource
- **Type**: Log Analytics query
- **Query**:
```kusto
customEvents
| where name == "AuthorizationFailure"
| where timestamp > ago(24h)
| summarize Count = count() by Resource = tostring(customDimensions.Resource)
| order by Count desc
```

---

## Kusto Queries

### Authentication Success Rate

```kusto
customEvents
| where timestamp > ago(24h)
| where name in ("AuthenticationSuccess", "AuthenticationFailure")
| summarize 
    Total = count(),
    Success = countif(name == "AuthenticationSuccess"),
    Failure = countif(name == "AuthenticationFailure")
| extend SuccessRate = round(Success * 100.0 / Total, 2)
```

### Authentication Failures by Reason

```kusto
customEvents
| where name == "AuthenticationFailure"
| where timestamp > ago(24h)
| summarize Count = count() by Reason = tostring(customDimensions.Reason)
| order by Count desc
```

### Authorization Failures by User

```kusto
customEvents
| where name == "AuthorizationFailure"
| where timestamp > ago(24h)
| summarize 
    FailureCount = count(),
    Resources = make_set(tostring(customDimensions.Resource))
    by UserId = tostring(customDimensions.UserId)
| order by FailureCount desc
```

### Entra ID Connectivity Issues

```kusto
customEvents
| where name == "EntraIdConnectivityFailure"
| where timestamp > ago(7d)
| project 
    timestamp,
    Operation = tostring(customDimensions.Operation),
    ErrorMessage = tostring(customDimensions.ErrorMessage)
| order by timestamp desc
```

### User Activity Timeline

```kusto
customEvents
| where timestamp > ago(24h)
| where name in ("AuthenticationSuccess", "UserSignOut")
| extend UserId = tostring(customDimensions.UserId)
| project timestamp, name, UserId
| order by timestamp desc
```

---

## Troubleshooting

### No Telemetry Data

1. **Check Connection String**: Verify `APPLICATIONINSIGHTS_CONNECTION_STRING` is set
2. **Check Sampling**: Low sampling rates may reduce visible events
3. **Check Filters**: Ensure your query time range includes the events

### Alerts Not Firing

1. **Check Alert State**: Navigate to Alerts → Alert Rules, verify enabled
2. **Check Action Group**: Verify email address is correct
3. **Check Query**: Run the alert query manually to verify results

### High Telemetry Costs

1. **Reduce Sampling**: Lower `SamplingPercentage` in Application Insights
2. **Filter Events**: Consider filtering low-value telemetry
3. **Daily Cap**: Set a daily ingestion cap in Application Insights settings

### Debug Mode

For development, you can enable more verbose logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "GhcSamplePs.Web.Services.AuthenticationTelemetryService": "Debug"
    }
  }
}
```

---

## Related Documentation

- [Azure Application Insights Documentation](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview)
- [Kusto Query Language (KQL)](https://learn.microsoft.com/azure/data-explorer/kusto/query/)
- [Azure Monitor Alerts](https://learn.microsoft.com/azure/azure-monitor/alerts/alerts-overview)
- [Microsoft Entra ID Monitoring](https://learn.microsoft.com/entra/identity/monitoring-health/)

---

**Last Updated:** December 3, 2025  
**Version:** 1.0.0
