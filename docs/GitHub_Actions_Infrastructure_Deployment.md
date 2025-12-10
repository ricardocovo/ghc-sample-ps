# GitHub Actions Infrastructure Deployment Setup

## Overview

This guide explains how to set up automated infrastructure deployment using GitHub Actions and Azure Bicep templates.

## Prerequisites

- Azure subscription
- Azure CLI installed locally (for initial setup)
- GitHub repository with infrastructure code
- Permissions to create service principals in Azure AD

## Azure Setup

### 1. Create Azure Service Principal with OIDC

GitHub Actions will use Federated Identity (OIDC) to authenticate with Azure without storing credentials.

```powershell
# Set variables
$subscriptionId = "1e06b317-efe8-4e37-9aa6-881b07bb52ad"
$resourceGroup = "rg-ghcsampleps-dev"
$appName = "ghcsampleps-github-deploy"
$githubOrg = "ricardocovo"
$githubRepo = "ghc-sample-ps"

# Login to Azure
az login
az account set --subscription $subscriptionId

# Create service principal with federated credentials
az ad sp create-for-rbac --name $appName --role contributor --scopes /subscriptions/$subscriptionId --sdk-auth

# Note down the output:
# - clientId
# - clientSecret (not needed for OIDC)
# - subscriptionId
# - tenantId

# Get the application ID
$appId = az ad sp list --display-name $appName --query "[0].appId" -o tsv

# Add User Access Administrator role (required for role assignments in Bicep)
$spObjectId = az ad sp list --display-name $appName --query "[0].id" -o tsv
az role assignment create --assignee-object-id $spObjectId --assignee-principal-type ServicePrincipal --role "User Access Administrator" --scope /subscriptions/$subscriptionId

# Create federated credential for main branch
az ad app federated-credential create --id $appId --parameters ('{\"name\":\"github-deploy-main\",\"issuer\":\"https://token.actions.githubusercontent.com\",\"subject\":\"repo:' + $githubOrg + '/' + $githubRepo + ':ref:refs/heads/main\",\"audiences\":[\"api://AzureADTokenExchange\"]}')

# Create federated credential for pull requests (optional)
az ad app federated-credential create --id $appId --parameters ('{\"name\":\"github-deploy-pr\",\"issuer\":\"https://token.actions.githubusercontent.com\",\"subject\":\"repo:' + $githubOrg + '/' + $githubRepo + ':pull_request\",\"audiences\":[\"api://AzureADTokenExchange\"]}')

# Create federated credential for dev environment
az ad app federated-credential create --id $appId --parameters ('{\"name\":\"github-deploy-env-dev\",\"issuer\":\"https://token.actions.githubusercontent.com\",\"subject\":\"repo:' + $githubOrg + '/' + $githubRepo + ':environment:dev\",\"audiences\":[\"api://AzureADTokenExchange\"]}')

# Create federated credential for prod environment (optional)
az ad app federated-credential create --id $appId --parameters ('{\"name\":\"github-deploy-env-prod\",\"issuer\":\"https://token.actions.githubusercontent.com\",\"subject\":\"repo:' + $githubOrg + '/' + $githubRepo + ':environment:prod\",\"audiences\":[\"api://AzureADTokenExchange\"]}')
```

### 2. Get SQL Admin Object ID

```powershell
# Get your current user's object ID (if you're the SQL admin)
$objectId = az ad signed-in-user show --query id -o tsv
echo "SQL Admin Object ID: $objectId"

# Or get a specific user's object ID
$email = "your-email@domain.com"
$objectId = az ad user show --id $email --query id -o tsv
echo "SQL Admin Object ID: $objectId"
```

## GitHub Repository Setup

### 1. Configure GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions → New repository secret

Add the following secrets:

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `AZURE_CLIENT_ID` | `<app-id-from-step-1>` | Service principal application ID |
| `AZURE_TENANT_ID` | `<tenant-id>` | Azure AD tenant ID |
| `AZURE_SUBSCRIPTION_ID` | `<subscription-id>` | Azure subscription ID |
| `SQL_ADMIN_EMAIL` | `your-email@domain.com` | SQL Server Entra ID admin email |
| `SQL_ADMIN_OBJECT_ID` | `<object-id-from-step-2>` | SQL Server admin object ID |
| `ENTRA_CLIENT_ID` | `<your-app-registration-client-id>` | Application Entra ID client ID |
| `ENTRA_TENANT_ID` | `<tenant-id>` | Application Entra ID tenant ID |

### 2. Configure GitHub Environments (Optional but Recommended)

For better control and approvals:

1. Go to Settings → Environments
2. Create environments: `dev` and `prod`
3. Configure environment protection rules:
   - **dev**: No restrictions
   - **prod**: Required reviewers, wait timer

## Workflow Triggers

The workflow runs automatically when:

1. **Manual trigger**: Use "Run workflow" button in GitHub Actions
2. **Push to main**: When changes are pushed to main branch affecting:
   - `infra/**` (any Bicep file changes)
   - `.github/workflows/deploy-infrastructure.yml` (workflow changes)

## Manual Deployment

To manually deploy infrastructure:

1. Go to **Actions** tab in GitHub
2. Select **Deploy Infrastructure** workflow
3. Click **Run workflow**
4. Select environment: `dev` or `prod`
5. Click **Run workflow** button

## Workflow Stages

### 1. Validate Stage

- Validates all Bicep templates
- Checks for syntax errors
- Verifies parameter values
- Does not deploy any resources

### 2. Deploy Stage

- Creates resource group if it doesn't exist
- Deploys all infrastructure using Bicep
- Outputs deployment information
- Configures RBAC permissions

### 3. Verify Stage

- Checks Container App provisioning state
- Verifies SQL Server deployment
- Displays deployment summary

## Expected Resources Deployed

After successful deployment, the following Azure resources will be created:

- **Resource Group**: `rg-ghcsampleps-{environment}`
- **Log Analytics Workspace**: Monitoring and diagnostics
- **Application Insights**: Application telemetry
- **Storage Account**: Data Protection keys storage
- **Key Vault**: Secrets and encryption keys
- **SQL Server**: Database server with Entra ID authentication
- **SQL Database**: Serverless database
- **Container Registry**: Docker image hosting
- **Container App Environment**: Application hosting environment
- **Container App**: Blazor application instance

## Monitoring Deployment

### View workflow progress

1. Go to **Actions** tab
2. Click on the running workflow
3. Expand each job to see detailed logs

### Check Azure deployment

```powershell
# List all resources in the resource group
az resource list --resource-group rg-ghcsampleps-dev --output table

# Check deployment history
az deployment group list --resource-group rg-ghcsampleps-dev --output table

# View specific deployment details
az deployment group show \
  --resource-group rg-ghcsampleps-dev \
  --name infra-<run-number>-1
```

## Troubleshooting

### Common Issues

**Issue**: "The client does not have authorization to perform action"
- **Solution**: Ensure service principal has Contributor role on subscription

**Issue**: "Federated credential not found"
- **Solution**: Verify federated credential was created correctly with exact repository name

**Issue**: "Parameter validation failed"
- **Solution**: Check all required secrets are configured in GitHub

**Issue**: "Resource provider not registered"
- **Solution**: Register required providers:
  ```powershell
  az provider register --namespace Microsoft.App
  az provider register --namespace Microsoft.ContainerRegistry
  az provider register --namespace Microsoft.KeyVault
  az provider register --namespace Microsoft.Sql
  az provider register --namespace Microsoft.Storage
  ```

### View detailed errors

```powershell
# Get deployment error details
az deployment group show \
  --resource-group rg-ghcsampleps-dev \
  --name infra-<run-number>-1 \
  --query "properties.error"
```

## Security Best Practices

1. **Use OIDC instead of service principal secrets** ✅ (already configured)
2. **Enable environment protection rules** for production deployments
3. **Review deployment logs** regularly for suspicious activity
4. **Rotate service principal credentials** if compromised
5. **Use GitHub environment secrets** for environment-specific values
6. **Enable branch protection** on main branch

## Next Steps

After infrastructure is deployed:

1. **Deploy application**: Create workflow to build and deploy Docker image
2. **Run database migrations**: Apply EF Core migrations to Azure SQL
3. **Configure custom domain**: Set up custom domain for Container App
4. **Enable monitoring**: Configure alerts in Application Insights
5. **Set up staging slots**: Add staging environment for blue-green deployments

## Cost Optimization

The infrastructure is configured for cost optimization:

- **SQL Database**: Serverless tier (auto-pause after 1 hour)
- **Container App**: Scale to zero when idle
- **Storage**: Standard tier with lifecycle management
- **Log Analytics**: 30-day retention

**Estimated monthly cost**: $20-50 (dev environment with minimal usage)

## Cleanup

To delete all resources:

```powershell
# Delete entire resource group
az group delete --name rg-ghcsampleps-dev --yes --no-wait

# Delete service principal
az ad sp delete --id <client-id>
```

## Support

For issues or questions:
- Check workflow logs in GitHub Actions
- Review Azure deployment logs
- Consult Bicep documentation: <https://learn.microsoft.com/azure/azure-resource-manager/bicep/>
- Open an issue in the repository
