# Azure Container Apps + SQL Server Deployment Architecture

## Overview

This document outlines the high-level architecture for deploying the GhcSamplePs Blazor Server application to Azure using Container Apps and SQL Server, following Azure Well-Architected Framework principles.

---

## Architecture Components

### Core Services

#### 1. **Azure Container Apps**

- **Purpose**: Host the Blazor Server application (.NET 10)
- **Key Features Required**:
  - ✅ Session Affinity (Sticky Sessions) - Critical for Blazor Server
  - ✅ Multi-instance support with Data Protection
  - ✅ HTTPS ingress
  - ✅ Health probes
  - ✅ Managed Identity for Azure service authentication

#### 2. **Azure SQL Database**

- **Purpose**: Primary data store for application data
- **Authentication**: Managed Identity (passwordless)
- **Key Features**:
  - Connection resilience with retry policies
  - Automatic backups
  - Point-in-time restore capability
  - Firewall rules for Container Apps

#### 3. **Azure Key Vault**

- **Purpose**: Secrets management and Data Protection key storage
- **Stores**:
  - Application secrets
  - Connection strings (if needed)
  - Data Protection keys encryption
- **Access**: Via Managed Identity

#### 4. **Azure Storage Account**

- **Purpose**: Data Protection key persistence for multi-instance Blazor Server
- **Key Features**:
  - Blob container for ASP.NET Core Data Protection keys
  - Managed Identity access
  - Keys shared across all container instances

#### 5. **Log Analytics Workspace**

- **Purpose**: Centralized logging and monitoring
- **Collects**:
  - Container logs (stdout/stderr)
  - Application logs
  - System metrics
  - Health check results

#### 6. **Application Insights**

- **Purpose**: Application performance monitoring and diagnostics
- **Key Features**:
  - Request tracking
  - Dependency monitoring
  - Exception tracking
  - Custom metrics and events

---

## Critical Architectural Requirements (Clarification Needed)

### 🔍 Security & Compliance

**Decision**: Regulatory compliance requirements

- **Selected**: GDPR (General Data Protection Regulation)
- **Impact**: 
  - Encryption at rest and in transit (enabled by default in Azure)
  - Data residency in Canada meets GDPR adequacy requirements
  - Audit logging enabled via Log Analytics
  - Data retention and deletion policies required
  - User consent and data access rights must be implemented in application

**Question**: What is your data residency requirement?

- **Selected**: Canada (Canada Central or Canada East)
- **Impact**: Determines Azure region for all resources

**Decision**: SQL Authentication method

- **Selected**: Managed Identity (passwordless)
- **Benefits**:
  - No credentials to store or rotate
  - Microsoft security best practice
  - Uses DefaultAzureCredential in code
  - Works locally (dev) and in Azure (production)
- **Implementation**: System-assigned managed identity with SQL database roles

### ✅ Performance & Scale

**Decision: Development-Appropriate Sizing**

**SLA Requirements**: 
- **None** (Development environment)
- No SLA needed for testing and validation
- Single-zone deployment sufficient

**Anticipated Load**:
- **Concurrent users**: 1-5 (developers only)
- **Usage pattern**: Intermittent (active development hours only)
- **Requests per second**: <10
- **Impact**: Minimal scaling needed, scale-to-zero acceptable

**SQL Database Tier: Azure SQL Serverless**
- **Auto-scaling**: 0.5-2 vCores (adjusts based on usage)
- **Auto-pause**: After 1 hour of inactivity
- **Storage**: Up to 32GB included
- **Cost**: \$5-15/month (variable based on actual compute usage)
- **Resume time**: ~1 second from paused state
- **Perfect for**: Development workloads with intermittent usage
- **Trade-off**: ~1 second delay on first request after idle period (acceptable for dev)

### 💰 Cost & Environment Strategy

**Question**: Environment type?

- **Production**: Full resilience, monitoring, and security
- **Staging**: Production-like but potentially lower tier
- **Development**: Minimal costs, can use emulators

**Question**: Multi-region deployment?

- Single region: Lower cost, simpler
- Multi-region: Higher availability, geographic distribution
- **Trade-off**: Cost vs. resilience (2x-3x cost increase)

**Question**: Budget constraints?

- **Estimated Monthly Costs** (single region, production):
  - Container Apps: \-100 (depends on scale)
  - SQL Database Standard S2: \
  - Key Vault: \
  - Storage Account: \
  - Log Analytics: \-50 (depends on ingestion)
  - **Total**: ~\-235/month

### ✅ Entra ID (Azure AD) Integration

**Decision: Migrate Existing Configuration**

- **Tenant**: MngEnvMCAP091123.onmicrosoft.com (existing)
- **Approach**: Update redirect URIs for Azure Container Apps URL
- **Registration**: Use existing app registration, no new registration needed
- **External Identities**: Not required for development environment
- **Impact**: Minimal configuration changes, just add new redirect URI after deployment

### ✅ Operational Capabilities

**Decision: Manual Deployment with Scripts (Development)**

**DevOps Approach**:
- **Manual deployment** using Azure CLI scripts (appropriate for dev environment)
- **IaC**: Bicep templates for infrastructure reproducibility
- **No CI/CD pipeline initially** (can add GitHub Actions later when promoting to production)
- **Development workflow**: Deploy on-demand when testing infrastructure changes

**Infrastructure-as-Code**:
- **Tool**: Bicep (Microsoft recommended for Azure)
- **Organization**: Modular templates in `infra/` directory
- **Deployment**: Azure CLI commands with parameter files
- **Version control**: Bicep files in git repository

**Deployment Process**:
1. Update Bicep parameters for development environment
2. Run `az deployment group create` to deploy/update infrastructure
3. Build and push Docker image to Azure Container Registry
4. Update Container App with new image revision
5. Validate deployment with health checks

**Rationale**: 
- Manual deployment appropriate for development environment
- Faster iteration without pipeline overhead
- Easy to understand and troubleshoot
- Clear path to CI/CD when needed (GitHub Actions templates ready)

---

## Azure Well-Architected Framework Analysis

### 🛡️ Security Pillar

**Strengths**:

- Managed Identity eliminates credential storage
- Key Vault for secrets management
- Entra ID integration for authentication
- HTTPS-only communication

**Decision**: Network Security - Basic (Public Ingress)

- Public HTTPS ingress with Entra ID authentication
- Azure SQL firewall rules for Container Apps
- HTTPS encryption for all traffic
- Can upgrade to VNet integration later if needed

**Future Considerations**:

- VNet integration (+$150/month) for internal-only access
- Private endpoints for enhanced data protection
- WAF (+$300/month) for advanced threat protection

### ⚡ Reliability Pillar

**Strengths**:

- Container Apps auto-scaling
- SQL Database automated backups
- Health probes for container health

**Decision**: 99.9% SLA (Single Zone)

- Single-zone deployment in Canada Central
- Sufficient for most business applications
- Lower cost compared to zone redundancy
- Automated backups provide data protection

**Future Considerations**:

- Zone redundancy (+30% cost) for 99.95% SLA
- Multi-region deployment (2-3x cost) for 99.99% SLA
- Active-passive failover to Canada East

### 🚀 Performance Efficiency Pillar

**Strengths**:

- Horizontal scaling for Container Apps
- SQL connection pooling and retry logic
- Blazor Server session affinity optimization

**Decision**: Scaling Configuration for ~50 concurrent users

- **Container Apps**: 0.5 vCPU, 1GB RAM per replica
- **Replicas**: Min 1, Max 5 (sufficient for load)
- **Autoscaling**: HTTP request-based scaling
- **SQL Tier**: Standard S1 or S2 adequate for this load

**Considerations**:

- Scale-to-zero during off-season for cost savings
- Monitor actual usage and adjust scaling rules
- Peak times (evenings/weekends) handled by autoscaling

### 💵 Cost Optimization Pillar

**Decision**: Azure SQL Serverless for variable workload

- Auto-pause during inactivity (weekdays, off-season)
- Auto-resume when users connect
- Pay only for compute actually used
- Fixed storage cost for actual data size

**Optimization Strategies**:

- Serverless SQL perfect for intermittent usage
- Container Apps scale-to-zero for dev/staging
- Monitor actual usage and adjust pause delay
- Budget alerts for cost management

**Monitoring**:

- Azure Cost Management + Billing alerts
- Budget thresholds
- Resource tagging for cost allocation

**Trade-offs**:

- **Performance vs. Cost**: Lower tiers save money but may affect UX
- **Availability vs. Cost**: Multi-region and zone redundancy increase costs

### 🔧 Operational Excellence Pillar

**Strengths**:

- Infrastructure as Code (Bicep)
- Centralized logging (Log Analytics)
- Application monitoring (App Insights)

**Considerations**:

- Automated deployment pipelines
- Blue/green or canary deployments
- Automated rollback capabilities
- Disaster recovery testing

**Trade-offs**:

- **Automation complexity**: More robust but requires investment
- **Manual control**: Simpler initially but error-prone at scale

---

## Recommended Minimal Viable Architecture

## Development Environment Architecture

### Configuration

| Component | Specification | Rationale |
|-----------|--------------|-----------|
| **Environment Type** | Development | Cost-optimized for testing and validation |
| **Region** | Canada Central | GDPR adequacy, data residency requirement |
| **Container Apps** | 0.25 vCPU, 0.5GB RAM | Minimal resources for dev workload |
| **Replicas** | Min: 0, Max: 1 | Scale-to-zero for cost savings |
| **SQL Database** | Serverless (0.5-2 vCores) | Auto-pause/resume, variable cost |
| **Storage Account** | Standard LRS | Data Protection keys, minimal cost |
| **Key Vault** | Standard | Sufficient for development |
| **Authentication** | Managed Identity | Microsoft best practice, passwordless |
| **Monitoring** | App Insights (sampled) | Basic telemetry, low ingestion cost |
| **Log Analytics** | Free tier (500MB/day) | Sufficient for development logs |
| **IaC Tool** | Bicep | Microsoft recommended, modern syntax |

### Cost-Saving Features Enabled

- ✅ **Container Apps scale-to-zero**: No compute cost when idle
- ✅ **SQL auto-pause**: No compute cost after 1 hour inactive
- ✅ **Basic monitoring**: Free tier Log Analytics, sampled App Insights
- ✅ **Single replica**: No redundancy overhead
- ✅ **Minimal resources**: Smallest container sizes
- ✅ **No VNet**: Public ingress only (can add later)
- ✅ **No zone redundancy**: Single zone deployment

### Deployment Strategy

1. **Infrastructure Deployment**:
   - Use Bicep templates in \infra/\ directory
   - Validate with \z deployment group what-if\
   - Deploy to resource group

2. **Application Deployment**:
   - Build Docker image with multi-stage Dockerfile
   - Test locally before pushing
   - Push to Azure Container Registry
   - Deploy to Container Apps

3. **Configuration**:
   - Update Entra ID app registration with Container Apps URL
   - Configure managed identity access to SQL, Key Vault, Storage
   - Set environment variables in Container Apps
   - Apply SQL schema migrations

4. **Validation**:
   - Health check endpoints responding
   - Application Insights receiving telemetry
   - Blazor Server app loads correctly
   - Authentication flow works
   - Database connectivity confirmed

---

## Blazor Server Specific Requirements

### Session Affinity (Sticky Sessions)

**Why**: Blazor Server maintains state on the server for each client connection. Without session affinity, requests from the same client could hit different container instances, breaking the SignalR connection.

**How**: Azure Container Apps session affinity configuration
\\\yaml
sessionAffinity: sticky
\\\

### Data Protection Configuration

**Why**: When running multiple container instances, ASP.NET Core Data Protection keys must be shared across instances to decrypt cookies and authentication tokens.

**How**: Configure Data Protection to store keys in Azure Blob Storage and protect them with Azure Key Vault.

**Code Changes Required**:
\\\csharp
builder.Services.AddDataProtection()
    .PersistKeysToAzureBlobStorage(new Uri("..."))
    .ProtectKeysWithAzureKeyVault(new Uri("..."), new DefaultAzureCredential());
\\\

### SignalR Considerations

**Not Required**: Unlike standalone SignalR apps, Blazor Server with session affinity doesn't need an external backplane (like Azure SignalR Service).

---

## Security Considerations

### Managed Identity Flow

1. Container App has system-assigned or user-assigned managed identity
2. Identity granted appropriate RBAC roles:
   - SQL Database: \db_datareader\, \db_datawriter\, \db_ddladmin\
   - Key Vault: \Key Vault Secrets User\, \Key Vault Crypto User\
   - Storage Account: \Storage Blob Data Contributor\
3. Application code uses \DefaultAzureCredential\ - works locally (dev) and in Azure (managed identity)

### Network Security Options

| Level | Configuration | Cost Impact | Security Benefit |
|-------|--------------|-------------|------------------|
| **Basic** | Public ingress, firewall rules | Baseline | Basic protection |
| **Standard** | VNet integration, private endpoints | +\/month | Internal-only access |
| **Advanced** | WAF, DDoS protection, NSG rules | +\/month | Enterprise-grade |

**Recommendation**: Start with Basic, add VNet integration for production if handling sensitive data.

---

## Migration Path for Existing App

### Required Code Changes

1. **Connection String Format** (if using Managed Identity):
   \\\
   Server=tcp:<server>.database.windows.net,1433;
   Initial Catalog=<database>;
   Encrypt=True;
   TrustServerCertificate=False;
   Connection Timeout=30;
   Authentication="Active Directory Default";
   \\\

2. **Data Protection Configuration**:
   - Add NuGet packages
   - Configure in \Program.cs\

3. **Environment Variables**:
   - Move \ppsettings.json\ secrets to environment variables
   - Use Azure Container Apps secrets feature

4. **Health Check Endpoint**:
   - Already configured at \/health\
   - Add startup probe for initial load time

5. **Dockerfile**:
   - Multi-stage build for optimization
   - Non-root user for security
   - Port 8080 (standard for Container Apps)

### No Changes Required

- ✅ Existing Entra ID configuration (just update redirect URIs)
- ✅ Entity Framework Core code
- ✅ Blazor components and pages
- ✅ MudBlazor components
- ✅ Core business logic

---

## Next Steps: Development Environment Deployment

### Ready to Create Deployment Artifacts

With all architectural decisions finalized for the **Development environment**, I'll now create:

1. **Dockerfile** 
   - Multi-stage build optimized for .NET 10 Blazor Server
   - Non-root user for security
   - Port 8080 for Container Apps compatibility
   - Minimal image size

2. **Bicep Infrastructure Templates** (`infra/` directory)
   - `main.bicep` - Main orchestration template
   - `containerapp.bicep` - Container Apps with scale-to-zero
   - `sql.bicep` - Azure SQL Serverless with auto-pause
   - `keyvault.bicep` - Key Vault for secrets and key protection
   - `storage.bicep` - Storage Account for Data Protection keys
   - `monitoring.bicep` - Log Analytics (free tier) + App Insights (sampled)
   - `main.parameters.dev.json` - Development environment parameters

3. **Application Code Updates**
   - Add Data Protection NuGet packages
   - Update `Program.cs` with Azure Blob + Key Vault configuration
   - Configure Managed Identity authentication for SQL
   - Environment variable configuration

4. **Deployment Guide** (`docs/infra/deployment-guide.md`)
   - Prerequisites (Azure CLI, Docker)
   - Step-by-step Azure resource deployment
   - Container image build and push
   - Managed Identity permission configuration
   - Entra ID redirect URI updates
   - Validation and troubleshooting

5. **Deployment Scripts** (`infra/scripts/`)
   - `deploy-infra.ps1` - Deploy/update infrastructure
   - `build-push-image.ps1` - Build and push Docker image
   - `grant-permissions.ps1` - Configure Managed Identity access

### Promotion Path to Production

When ready to deploy to production later:
- ✅ Same Bicep templates, different parameter file (`main.parameters.prod.json`)
- ✅ Change: Min replicas 0→1, Max replicas 1→5
- ✅ Change: Container resources 0.25vCPU→0.5vCPU, 0.5GB→1GB
- ✅ Change: Scale-to-zero disabled
- ✅ Add: Zone redundancy for 99.95% SLA
- ✅ Add: VNet integration for network isolation (optional)
- ✅ Add: GitHub Actions CI/CD pipeline

### Estimated Development Costs

**Monthly**: \$7-35/month
- Idle (auto-paused): ~\$7-10/month
- Active development (8 hours/day, 5 days/week): ~\$25-35/month

**Cost Savings**: 80-95% vs. production configuration (\$125-235/month)

---

## Additional Resources

- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [Blazor Server Hosting and Deployment](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/server)
- [Azure SQL Managed Identity](https://learn.microsoft.com/azure/azure-sql/database/authentication-aad-configure)
- [Azure Well-Architected Framework](https://learn.microsoft.com/azure/well-architected/)
- [Container Apps Best Practices](https://learn.microsoft.com/azure/well-architected/service-guides/azure-container-apps)

---

## Decision Log

This section will be updated as architectural decisions are made.

| Date | Decision | Rationale | Trade-offs Accepted |
|------|----------|-----------|---------------------|
| 2025-12-02 | **Region**: Canada Central | GDPR adequacy, data residency compliance | Higher latency for non-Canadian users |
| 2025-12-02 | **Compliance**: GDPR | Regulatory requirement | Additional encryption and audit overhead |
| 2025-12-02 | **SQL Auth**: Managed Identity | Microsoft best practice, passwordless | Initial setup complexity vs. connection strings |
| 2025-12-02 | **Network**: Basic (public ingress) | Cost-effective for development | No VNet isolation (can add later) |
| 2025-12-02 | **SLA**: None (Development) | No uptime requirements for dev | No guaranteed availability |
| 2025-12-02 | **Load**: 1-5 concurrent developers | Development environment only | Not sized for production load |
| 2025-12-02 | **SQL**: Serverless (0.5-2 vCores) | Auto-pause/resume for intermittent dev usage | ~1 second resume delay from paused |
| 2025-12-02 | **Environment**: Development only | Validate deployment, minimize costs | No production readiness features |
| 2025-12-02 | **Replicas**: Min 0, Max 1 | Scale-to-zero for cost savings | Cold start delays (1-10 seconds) |
| 2025-12-02 | **Entra ID**: Migrate existing | Reuse MngEnvMCAP091123.onmicrosoft.com | Update redirect URIs post-deployment |
| 2025-12-02 | **Deployment**: Manual with Bicep | Appropriate for dev, fast iteration | No automated CI/CD pipeline |
| 2025-12-02 | **Monitoring**: Basic (free tier) | Sufficient for development debugging | Limited retention and sampling |
