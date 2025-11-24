# Azure Entra ID Configuration Quick Reference

This document provides quick reference information for the Azure Entra ID External Identities infrastructure.

## Configuration Values

### Entra ID Tenant

| Property | Value | Location |
|----------|-------|----------|
| Tenant Type | Azure AD B2C / External Identities | Azure Portal |
| Tenant ID | `00000000-0000-0000-0000-000000000000` | Replace with actual GUID |
| Domain | `your-tenant.onmicrosoft.com` | Replace with actual domain |
| Instance | `https://login.microsoftonline.com/` | Fixed value |

### Application Registration

| Property | Value | Location |
|----------|-------|----------|
| Application Name | GhcSamplePs Web Application | Entra ID > App registrations |
| Client ID | `00000000-0000-0000-0000-000000000000` | Replace with actual GUID |
| Client Secret | `[Stored in Key Vault/User Secrets]` | NEVER commit to source control |

### Redirect URIs

| Environment | Sign-in Redirect URI | Sign-out Redirect URI |
|-------------|---------------------|----------------------|
| Development | `https://localhost:5001/signin-oidc` | `https://localhost:5001/signout-oidc` |
| Staging | `https://[app]-staging.azurecontainerapps.io/signin-oidc` | `https://[app]-staging.azurecontainerapps.io/signout-oidc` |
| Production | `https://[app].azurecontainerapps.io/signin-oidc` | `https://[app].azurecontainerapps.io/signout-oidc` |

### App Roles

| Role Name | Value | Description |
|-----------|-------|-------------|
| Admin | `Admin` | Full administrative access |
| User | `User` | Standard user access |

### Identity Providers

| Provider | Status | Configuration Required |
|----------|--------|----------------------|
| Microsoft Account | ✅ Enabled | Built-in, no additional configuration |
| Google | ⚙️ Optional | Requires Google OAuth credentials |
| Email Signup | ✅ Enabled | Local accounts with email/password |

### User Flows

| Flow Name | Type | Purpose |
|-----------|------|---------|
| signupsignin1 | Sign up and sign in | Combined registration and authentication flow |

### Test Users

| Username | Display Name | Role | Password Location |
|----------|--------------|------|-------------------|
| admin@[tenant].onmicrosoft.com | Test Admin | Admin | Secure team vault |
| testuser@[tenant].onmicrosoft.com | Test User | User | Secure team vault |

### Azure Key Vault

| Property | Value |
|----------|-------|
| Key Vault Name | `kv-ghcsampleps-prod` |
| Region | Same as Container Apps |
| Secret Name | `AzureAd--ClientSecret` |
| Access Method | Managed Identity (production) |

## Configuration in Code

### appsettings.json (Template)

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "00000000-0000-0000-0000-000000000000",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

### appsettings.Development.json (Development)

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-actual-tenant.onmicrosoft.com",
    "TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "ClientId": "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

### User Secrets (Development)

```bash
# Set client secret in user secrets
dotnet user-secrets set "AzureAd:ClientSecret" "your-actual-secret"
```

### Environment Variables (Production)

```bash
# Environment variables in Azure Container Apps
AZUREAD__INSTANCE=https://login.microsoftonline.com/
AZUREAD__DOMAIN=your-tenant.onmicrosoft.com
AZUREAD__TENANTID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
AZUREAD__CLIENTID=yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
AZUREAD__CLIENTSECRET=@Microsoft.KeyVault(SecretUri=https://kv-ghcsampleps-prod.vault.azure.net/secrets/AzureAd--ClientSecret)
```

## Common Commands

### User Secrets Management

```bash
# Initialize user secrets
cd src/GhcSamplePs.Web
dotnet user-secrets init

# Set client secret
dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"

# List all secrets
dotnet user-secrets list

# Remove a secret
dotnet user-secrets remove "AzureAd:ClientSecret"

# Clear all secrets
dotnet user-secrets clear
```

### Azure CLI Commands

```bash
# Login to Azure
az login

# List tenants
az account list --output table

# Switch tenant
az account set --subscription "subscription-id"

# Get Key Vault secret
az keyvault secret show --name "AzureAd--ClientSecret" --vault-name "kv-ghcsampleps-prod"

# Set Key Vault secret
az keyvault secret set --name "AzureAd--ClientSecret" --vault-name "kv-ghcsampleps-prod" --value "your-secret"
```

### Build and Run

```bash
# Build solution
dotnet build

# Run application
cd src/GhcSamplePs.Web
dotnet run

# Run on specific port
dotnet run --urls "https://localhost:5001"

# Test with HTTPS certificate
dotnet dev-certs https --trust
```

## Azure Portal URLs

### Quick Links

| Resource | URL |
|----------|-----|
| Azure Portal | https://portal.azure.com |
| Entra ID Portal | https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/Overview |
| App Registrations | https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/RegisteredApps |
| Enterprise Applications | https://portal.azure.com/#view/Microsoft_AAD_IAM/StartboardApplicationsMenuBlade/~/AppAppsPreview |
| Identity Providers | https://portal.azure.com/#view/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/~/Providers |
| User Flows | https://portal.azure.com/#view/Microsoft_AAD_B2CAdmin/TenantManagementMenuBlade/~/UserFlows |
| Key Vaults | https://portal.azure.com/#view/HubsExtension/BrowseResource/resourceType/Microsoft.KeyVault%2Fvaults |

## Security Guidelines

### What to Commit to Source Control

✅ **Safe to commit:**
- `appsettings.json` (with placeholder values)
- `appsettings.Development.json` (with real Tenant ID and Client ID)
- Configuration templates and documentation
- Build scripts and deployment configurations

❌ **Never commit:**
- Client secrets
- User secrets files
- Personal access tokens
- Any credentials or passwords
- `secrets.json` files

### Secret Storage by Environment

| Environment | Client Secret Storage | Access Method |
|-------------|---------------------|---------------|
| Development | User Secrets | `dotnet user-secrets` |
| Staging | Azure Key Vault | Environment variables with Key Vault reference |
| Production | Azure Key Vault | Managed Identity + Environment variables |

### Secret Rotation Schedule

| Secret Type | Rotation Frequency | Next Rotation |
|-------------|-------------------|---------------|
| Client Secret | Every 6 months | Document date |
| Google OAuth Secret | Every 12 months | Document date |
| Key Vault Access Keys | Every 12 months | Document date |

## Troubleshooting Quick Reference

### Common Issues

| Issue | Cause | Solution |
|-------|-------|----------|
| "AADSTS50011: Reply URL mismatch" | Redirect URI not configured | Add exact URI to app registration |
| "AADSTS700016: Application not found" | Wrong Client ID or Tenant | Verify IDs match configuration |
| "Invalid client secret" | Secret expired or incorrect | Generate new secret, update storage |
| "Certificate not trusted" | HTTPS cert not installed | Run `dotnet dev-certs https --trust` |
| "Cannot access Key Vault" | Permission issue | Check access policies and managed identity |

### Diagnostic Commands

```bash
# Check configuration values
cd src/GhcSamplePs.Web
dotnet run --no-build -- --urls "https://localhost:5001"

# Verify user secrets
dotnet user-secrets list

# Check git status for accidental commits
git status
git diff

# Verify HTTPS certificate
dotnet dev-certs https --check
```

## Support Contacts

| Area | Contact | Notes |
|------|---------|-------|
| Azure Infrastructure | Infrastructure Team | Azure Portal access issues |
| Entra ID Configuration | Security Team | Identity provider and role configuration |
| Application Integration | Development Team | Code and configuration questions |
| Key Vault Access | Security Team | Secret management and access policies |

## Related Documentation

- **Setup Guide**: `docs/Azure_EntraID_Setup_Guide.md`
- **Development Setup**: `docs/Development_Environment_Setup.md`
- **Verification Checklist**: `docs/Infrastructure_Verification_Checklist.md`
- **Integration Spec**: `docs/specs/EntraID_ExternalIdentities_Integration_Specification.md`

## Updates and Maintenance

This document should be updated when:
- Tenant or application configuration changes
- New environments are added (e.g., staging)
- Identity providers are added or removed
- Secret rotation occurs
- URLs or endpoints change

**Last Updated**: 2025-11-24  
**Next Review**: When infrastructure changes  
**Maintained By**: Infrastructure Team

---

## Notes

**IMPORTANT REMINDERS:**
- Always use HTTPS for redirect URIs
- Never commit secrets to source control
- Rotate secrets according to schedule
- Test configuration changes in development first
- Document all configuration changes
- Keep this reference updated with actual values (in secure team wiki)
