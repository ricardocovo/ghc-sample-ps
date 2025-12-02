# Azure Deployment Implementation Plan

## Overview

This document outlines the detailed implementation plan for deploying GhcSamplePs to Azure Container Apps with SQL Serverless in a **Development environment** configuration.

---

## Architecture Summary

Based on `high-level.md` decisions:

- **Environment**: Development only
- **Region**: Canada Central
- **SQL**: Serverless (0.5-2 vCores, auto-pause after 1hr)
- **Container Apps**: Scale-to-zero (Min: 0, Max: 1, 0.25 vCPU, 0.5GB RAM)
- **Authentication**: Managed Identity (passwordless)
- **Monitoring**: Basic (Log Analytics free tier, App Insights sampling)
- **Deployment**: Manual with Bicep IaC
- **Estimated Cost**: $7-35/month

---

## Implementation Phases

### Phase 1: Infrastructure

### 1. monitoring.bicep Module

**Status: ✅ Code Ready - Awaiting File Creation**

The monitoring module provides centralized logging and application performance monitoring for Azure Container Apps.

#### Quick Setup Commands:

```bash
# Create directory structure
mkdir -p infra/modules

# Create the monitoring.bicep file with the code below
# File: infra/modules/monitoring.bicep
```

#### Complete Bicep Implementation:

```bicep
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
```

#### Verification Commands:

```bash
# Build the module (validates syntax)
bicep build infra/modules/monitoring.bicep

# Lint the module (checks best practices)
bicep lint infra/modules/monitoring.bicep
```

#### Module Specifications:

| Component | Configuration | Purpose |
|-----------|--------------|---------|
| Log Analytics Workspace | PerGB2018 SKU, 30-day retention | Centralized logging for Container Apps |
| Application Insights | 10% sampling, LogAnalytics ingestion | Application performance monitoring |

#### Parameters:

| Name | Type | Description |
|------|------|-------------|
| `location` | string | Azure region for all resources |
| `logAnalyticsName` | string | Workspace name (4-63 chars) |
| `appInsightsName` | string | Application Insights name (1-260 chars) |
| `environment` | string | Environment tag (dev/prod) |

#### Outputs:

| Name | Type | Description |
|------|------|-------------|
| `logAnalyticsWorkspaceId` | string | Workspace resource ID |
| `appInsightsInstrumentationKey` | string | Instrumentation key |
| `appInsightsConnectionString` | string | Connection string |

### Phase 1 Deliverables as Code (Bicep)

#### Directory Structure
```
infra/
├── main.bicep                    # Main orchestration template
├── main.bicepparam               # Development parameters
├── modules/
│   ├── monitoring.bicep          # Log Analytics + App Insights
│   ├── storage.bicep             # Storage Account for Data Protection
│   ├── keyvault.bicep            # Key Vault for secrets
│   ├── sql.bicep                 # Azure SQL Serverless
│   ├── containerregistry.bicep   # Container Registry
│   └── containerapp.bicep        # Container Apps Environment + App
└── scripts/
    ├── deploy-infra.ps1          # Deploy infrastructure
    ├── build-push-image.ps1      # Build and push Docker image
    └── configure-permissions.ps1  # Grant Managed Identity access
```

#### Module Breakdown

**1. monitoring.bicep**
- Log Analytics Workspace
  - SKU: PerGB2018 (free tier: 500MB/day)
  - Location: Canada Central
  - Retention: 30 days (free tier)
- Application Insights
  - Connected to Log Analytics
  - Sampling enabled (10% for dev to reduce costs)
  - IngestionMode: LogAnalytics

**2. storage.bicep**
- Storage Account
  - SKU: Standard_LRS (locally redundant)
  - Kind: StorageV2
  - HTTPS only: true
  - Minimum TLS: 1.2
- Blob Container
  - Name: dataprotection-keys
  - Public access: None
- Outputs: Storage account name, blob endpoint

**3. keyvault.bicep**
- Azure Key Vault
  - SKU: Standard
  - Soft delete enabled (90 days)
  - Purge protection: enabled
  - RBAC authorization mode (not access policies)
- Outputs: Key Vault name, vault URI

**4. sql.bicep**
- Azure SQL Server
  - Managed Identity enabled (system-assigned)
  - Entra ID admin configured
  - Public network access: enabled (with firewall rules)
  - Minimum TLS: 1.2
- Azure SQL Database
  - SKU: GP_S_Gen5 (Serverless)
  - Min capacity: 0.5 vCores
  - Max capacity: 2 vCores
  - Auto-pause delay: 60 minutes
  - Storage: 32GB
- Firewall Rules
  - Azure services access: enabled
  - Specific IP ranges: configured via parameters
- Outputs: Server FQDN, database name, connection string format

**5. containerregistry.bicep**
- Azure Container Registry
  - SKU: Basic (sufficient for dev, $5/month)
  - Admin enabled: false (use Managed Identity)
  - Public network access: enabled
  - Anonymous pull: disabled
- Outputs: Registry name, login server

**6. containerapp.bicep**
- Container Apps Environment
  - Log Analytics integration
  - Zone redundancy: disabled (single zone)
  - Internal load balancer: false (public ingress)
- Container App
  - System-assigned Managed Identity
  - Scale rules:
    - Min replicas: 0 (scale-to-zero)
    - Max replicas: 1
    - Scale rule: HTTP concurrent requests (trigger: 10)
  - Resources:
    - CPU: 0.25 cores
    - Memory: 0.5Gi
  - Ingress:
    - External: true
    - Target port: 8080
    - Transport: http (Container Apps terminates TLS)
    - Session affinity: sticky
  - Environment variables (secrets):
    - ConnectionStrings__DefaultConnection
    - AzureAd__TenantId
    - AzureAd__ClientId
    - KeyVault__VaultUri
    - Storage__BlobEndpoint
- Outputs: Container App FQDN, identity principal ID

**7. main.bicep**
- Orchestrates all modules
- Parameters:
  - `location` (default: canadacentral)
  - `environment` (default: dev)
  - `appName` (default: ghcsampleps)
  - `sqlAdminEntraId` (required)
  - `sqlAdminObjectId` (required)
  - `entraIdClientId` (required)
  - `entraIdTenantId` (required)
- Resource naming convention: `{appName}-{resource}-{environment}`
- RBAC assignments after all resources created:
  - Container App Identity → SQL Database (db_datareader, db_datawriter, db_ddladmin)
  - Container App Identity → Key Vault Secrets User
  - Container App Identity → Key Vault Crypto User
  - Container App Identity → Storage Blob Data Contributor
  - Container App Identity → Container Registry (AcrPull)

**8. main.bicepparam**
```bicep
using './main.bicep'

param location = 'canadacentral'
param environment = 'dev'
param appName = 'ghcsampleps'
param sqlAdminEntraId = 'your-email@domain.com'
param sqlAdminObjectId = 'your-entra-object-id'
param entraIdClientId = 'your-app-client-id'
param entraIdTenantId = 'your-tenant-id'
```

---

### Phase 2: Application Changes

#### 1. Dockerfile
Location: `src/GhcSamplePs.Web/Dockerfile`

```dockerfile
# Multi-stage build for .NET 10 Blazor Server
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["GhcSamplePs.sln", "./"]
COPY ["src/GhcSamplePs.Web/GhcSamplePs.Web.csproj", "src/GhcSamplePs.Web/"]
COPY ["src/GhcSamplePs.Core/GhcSamplePs.Core.csproj", "src/GhcSamplePs.Core/"]

# Restore dependencies
RUN dotnet restore "src/GhcSamplePs.Web/GhcSamplePs.Web.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/GhcSamplePs.Web"
RUN dotnet build "GhcSamplePs.Web.csproj" -c Release -o /app/build
RUN dotnet publish "GhcSamplePs.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app

# Copy published app
COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

# Expose port 8080 (Container Apps standard)
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GhcSamplePs.Web.dll"]
```

#### 2. .dockerignore
Location: `src/GhcSamplePs.Web/.dockerignore`

```
**/.git/
**/.vs/
**/.vscode/
**/bin/
**/obj/
**/out/
**/*.user
**/*.suo
**/node_modules/
**/TestResults/
**/.DS_Store
**/appsettings.Development.json
```

#### 3. Program.cs Updates
Location: `src/GhcSamplePs.Web/Program.cs`

**Required NuGet Packages**:
```xml
<PackageReference Include="Azure.Extensions.AspNetCore.DataProtection.Blobs" Version="1.3.4" />
<PackageReference Include="Azure.Extensions.AspNetCore.DataProtection.Keys" Version="1.2.4" />
<PackageReference Include="Azure.Identity" Version="1.14.0" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" />
```

**Code Changes**:
```csharp
// Add after builder creation
var builder = WebApplication.CreateBuilder(args);

// Configure Azure credential for Managed Identity
var azureCredential = new DefaultAzureCredential();

// Configure Data Protection with Azure Blob + Key Vault
if (!builder.Environment.IsDevelopment())
{
    var blobEndpoint = builder.Configuration["Storage:BlobEndpoint"];
    var vaultUri = builder.Configuration["KeyVault:VaultUri"];
    
    if (!string.IsNullOrEmpty(blobEndpoint) && !string.IsNullOrEmpty(vaultUri))
    {
        builder.Services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(
                new Uri($"{blobEndpoint}/dataprotection-keys/keys.xml"),
                azureCredential)
            .ProtectKeysWithAzureKeyVault(
                new Uri($"{vaultUri}/keys/dataprotection"),
                azureCredential);
    }
}

// Configure SQL connection with Managed Identity
if (!builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        // Connection string should use Authentication=Active Directory Default
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            }));
    }
}
```

---

### Phase 3: Deployment Scripts

#### 1. deploy-infra.ps1
```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "canadacentral",
    
    [Parameter(Mandatory=$false)]
    [string]$ParameterFile = "./infra/main.bicepparam"
)

# Login check
$context = Get-AzContext
if (-not $context) {
    Write-Host "Please login to Azure..." -ForegroundColor Yellow
    Connect-AzAccount
}

# Create resource group if not exists
$rg = Get-AzResourceGroup -Name $ResourceGroupName -ErrorAction SilentlyContinue
if (-not $rg) {
    Write-Host "Creating resource group: $ResourceGroupName" -ForegroundColor Green
    New-AzResourceGroup -Name $ResourceGroupName -Location $Location
}

# Validate deployment
Write-Host "Validating Bicep deployment..." -ForegroundColor Cyan
$validation = Test-AzResourceGroupDeployment `
    -ResourceGroupName $ResourceGroupName `
    -TemplateParameterFile $ParameterFile `
    -TemplateFile "./infra/main.bicep"

if ($validation) {
    Write-Host "Validation failed:" -ForegroundColor Red
    $validation | Format-List
    exit 1
}

# Deploy infrastructure
Write-Host "Deploying infrastructure to Azure..." -ForegroundColor Green
$deployment = New-AzResourceGroupDeployment `
    -Name "ghcsampleps-$(Get-Date -Format 'yyyyMMddHHmmss')" `
    -ResourceGroupName $ResourceGroupName `
    -TemplateParameterFile $ParameterFile `
    -TemplateFile "./infra/main.bicep" `
    -Verbose

if ($deployment.ProvisioningState -eq 'Succeeded') {
    Write-Host "`nDeployment successful!" -ForegroundColor Green
    Write-Host "Container App URL: $($deployment.Outputs.containerAppUrl.Value)" -ForegroundColor Cyan
    Write-Host "SQL Server: $($deployment.Outputs.sqlServerFqdn.Value)" -ForegroundColor Cyan
} else {
    Write-Host "Deployment failed!" -ForegroundColor Red
    exit 1
}
```

#### 2. build-push-image.ps1
```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$RegistryName,
    
    [Parameter(Mandatory=$false)]
    [string]$ImageTag = "latest"
)

$imageName = "ghcsampleps-web"

# Build Docker image
Write-Host "Building Docker image..." -ForegroundColor Green
docker build -t "$imageName:$ImageTag" -f ./src/GhcSamplePs.Web/Dockerfile .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker build failed!" -ForegroundColor Red
    exit 1
}

# Login to Azure Container Registry
Write-Host "Logging into Azure Container Registry..." -ForegroundColor Cyan
az acr login --name $RegistryName

# Tag image for ACR
$acrLoginServer = "$RegistryName.azurecr.io"
docker tag "$imageName:$ImageTag" "$acrLoginServer/$imageName:$ImageTag"

# Push to ACR
Write-Host "Pushing image to ACR..." -ForegroundColor Green
docker push "$acrLoginServer/$imageName:$ImageTag"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nImage pushed successfully!" -ForegroundColor Green
    Write-Host "Image: $acrLoginServer/$imageName:$ImageTag" -ForegroundColor Cyan
}
```

#### 3. configure-permissions.ps1
```powershell
param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$ContainerAppName,
    
    [Parameter(Mandatory=$true)]
    [string]$SqlServerName,
    
    [Parameter(Mandatory=$true)]
    [string]$DatabaseName
)

# Get Container App Managed Identity
$app = Get-AzContainerApp -ResourceGroupName $ResourceGroupName -Name $ContainerAppName
$identityId = $app.IdentityPrincipalId

Write-Host "Container App Identity: $identityId" -ForegroundColor Cyan

# SQL Database permissions (must be done via SQL commands)
Write-Host "`nConfiguring SQL Database permissions..." -ForegroundColor Yellow
Write-Host "Run these SQL commands as the Entra ID admin:" -ForegroundColor Cyan
Write-Host @"

-- Connect to $DatabaseName database
CREATE USER [$ContainerAppName] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [$ContainerAppName];
ALTER ROLE db_datawriter ADD MEMBER [$ContainerAppName];
ALTER ROLE db_ddladmin ADD MEMBER [$ContainerAppName];
GO

"@ -ForegroundColor White
```

---

### Phase 4: Documentation

#### deployment-guide.md
Comprehensive step-by-step guide including:
1. Prerequisites (Azure CLI, Docker, .NET 10 SDK, Azure subscription)
2. Clone repository and navigate to project
3. Update `main.bicepparam` with your values
4. Run `deploy-infra.ps1` to create Azure resources
5. Run SQL commands to grant Managed Identity access
6. Update Entra ID app registration redirect URIs
7. Build and push Docker image with `build-push-image.ps1`
8. Update Container App with new image
9. Validate deployment (health checks, authentication)
10. Troubleshooting common issues

---

## Implementation Order

### Step 1: Create Bicep Modules (Day 1)
1. ✅ Create directory structure
2. ✅ Create monitoring.bicep
3. ✅ Create storage.bicep
4. ✅ Create keyvault.bicep
5. ✅ Create sql.bicep
6. ✅ Create containerregistry.bicep
7. ✅ Create containerapp.bicep
8. ✅ Create main.bicep (orchestration)
9. ✅ Create main.bicepparam

### Step 2: Create Application Files (Day 1)
1. ✅ Create Dockerfile
2. ✅ Create .dockerignore
3. ✅ Update Program.cs with Data Protection
4. ✅ Add required NuGet packages

### Step 3: Create Deployment Scripts (Day 1)
1. ✅ Create deploy-infra.ps1
2. ✅ Create build-push-image.ps1
3. ✅ Create configure-permissions.ps1

### Step 4: Testing & Validation (Day 2)
1. ✅ Run `bicep build` on all modules
2. ✅ Run `bicep lint` and fix warnings
3. ✅ Test deployment to Azure (dry-run)
4. ✅ Create deployment guide
5. ✅ Test full deployment end-to-end

---

## Success Criteria

- [ ] All Bicep templates compile without errors
- [ ] Infrastructure deploys successfully to Azure
- [ ] Container App runs with scale-to-zero enabled
- [ ] SQL Database auto-pauses after 1 hour
- [ ] Managed Identity authentication works for SQL, Key Vault, Storage
- [ ] Data Protection keys stored in Azure Blob + encrypted with Key Vault
- [ ] Session affinity maintains Blazor Server connections
- [ ] Entra ID authentication works with Azure URL
- [ ] Application Insights receives telemetry
- [ ] Monthly costs stay within $7-35 range
- [ ] Documentation allows fresh deployment from scratch

---

## Cost Verification

After deployment, verify costs align with estimates:
- Container Apps: $0 when idle (scale-to-zero working)
- SQL Serverless: Paused state showing (no compute charges)
- Storage/Key Vault/Monitoring: Minimal charges ($1-5/month)

---

## Promotion to Production

When ready to promote to production:
1. Copy `main.bicepparam` → `main.prod.bicepparam`
2. Update parameters:
   - `minReplicas: 0` → `1` (no scale-to-zero)
   - `maxReplicas: 1` → `5`
   - `cpuCores: 0.25` → `0.5`
   - `memory: 0.5Gi` → `1.0Gi`
   - `sqlTier: GP_S_Gen5` → `GP_Gen5` (always-on)
   - `environment: dev` → `prod`
3. Deploy to production resource group
4. Add zone redundancy and VNet if needed

---

## Next Steps

Ready to begin implementation. Start with Phase 1: Creating the Bicep infrastructure templates.
