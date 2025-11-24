# Development Environment Setup for Entra ID Integration

This guide walks you through configuring your local development environment to work with Azure Entra ID External Identities.

## Prerequisites

Before starting, ensure you have completed:

- ✅ Azure Entra ID infrastructure setup (see `docs/Azure_EntraID_Setup_Guide.md`)
- ✅ Entra ID tenant created and application registered
- ✅ Client ID, Tenant ID, and Client Secret obtained
- ✅ .NET 10 SDK installed
- ✅ Code editor (Visual Studio 2022, VS Code, or Rider)

## Overview

Development environment configuration involves:

1. Updating `appsettings.json` with non-sensitive Entra ID configuration
2. Storing sensitive secrets (Client Secret) in user secrets
3. Configuring redirect URIs for localhost
4. Testing the configuration

## Step 1: Configure appsettings.json

The `appsettings.json` file contains non-sensitive configuration that's safe to commit to source control.

### File Location
`src/GhcSamplePs.Web/appsettings.json`

### Configuration to Add

The Entra ID configuration section should be added with placeholder values:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "00000000-0000-0000-0000-000000000000",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Note**: These are placeholder values. Do NOT put real credentials in `appsettings.json`.

## Step 2: Configure appsettings.Development.json

The development-specific configuration file overrides settings for local development.

### File Location
`src/GhcSamplePs.Web/appsettings.Development.json`

### Configuration to Add

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "ClientId": "yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Identity": "Information"
    }
  }
}
```

### Replace Placeholders

Replace the following with values from your Entra ID setup:

- `your-tenant.onmicrosoft.com` → Your actual tenant domain (e.g., `ghcsampleps.onmicrosoft.com`)
- `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx` → Your Tenant ID (GUID)
- `yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy` → Your Client ID (GUID)

**Security Note**: Client Secret should NOT be in this file. It goes in user secrets (next step).

## Step 3: Configure User Secrets

User secrets provide a secure way to store sensitive values during development without committing them to source control.

### Initialize User Secrets

Navigate to the Web project directory and initialize user secrets:

```bash
cd src/GhcSamplePs.Web
dotnet user-secrets init
```

This command adds a `<UserSecretsId>` element to your `GhcSamplePs.Web.csproj` file.

### Add Client Secret

Store the client secret from your Entra ID app registration:

```bash
dotnet user-secrets set "AzureAd:ClientSecret" "your-actual-client-secret-value"
```

Replace `your-actual-client-secret-value` with the actual secret from Step 2.3 of the Azure setup guide.

### Verify User Secrets

List all configured secrets to verify:

```bash
dotnet user-secrets list
```

Expected output:
```
AzureAd:ClientSecret = your-actual-client-secret-value
```

### Where Are User Secrets Stored?

User secrets are stored outside the project directory:

- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- **Linux/macOS**: `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json`

This ensures they're never accidentally committed to source control.

## Step 4: Verify Redirect URIs

Ensure your Entra ID app registration includes the localhost redirect URI for development.

### Development Redirect URIs

Your app registration should include:

- **Sign-in callback**: `https://localhost:5001/signin-oidc`
- **Sign-out callback**: `https://localhost:5001/signout-callback-oidc`

### Verify in Azure Portal

1. Go to Azure Portal > Entra ID > App registrations
2. Select "GhcSamplePs Web Application"
3. Go to "Authentication"
4. Under "Platform configurations" > "Web" > "Redirect URIs"
5. Confirm `https://localhost:5001/signin-oidc` is listed
6. Under "Front-channel logout URL", confirm `https://localhost:5001/signout-oidc` is listed

### Add if Missing

If the localhost URIs are missing:

1. Click "Add URI" under Redirect URIs
2. Enter `https://localhost:5001/signin-oidc`
3. Scroll to "Front-channel logout URL"
4. Enter `https://localhost:5001/signout-oidc`
5. Click "Save"

## Step 5: Install Development Certificate (HTTPS)

ASP.NET Core requires HTTPS for authentication. Ensure you have a development certificate installed.

### Install Certificate

```bash
dotnet dev-certs https --trust
```

If prompted, confirm you trust the certificate.

### Verify Certificate

```bash
dotnet dev-certs https --check
```

Expected output: `A valid HTTPS certificate is already present.`

## Step 6: Test Configuration

Build and run the application to verify configuration.

### Build Solution

```bash
cd [repository-root]
dotnet build
```

Expected: Build succeeds without errors.

### Run Application

```bash
cd src/GhcSamplePs.Web
dotnet run
```

Expected output should include:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Access Application

1. Open browser to `https://localhost:5001`
2. Application should load (may show certificate warning - accept it)
3. If authentication is configured, you should see sign-in option

**Note**: Authentication integration code is not yet implemented. This step verifies configuration is valid.

## Configuration Files Summary

After setup, your configuration structure should look like:

```
src/GhcSamplePs.Web/
├── appsettings.json                    (template with placeholders - safe to commit)
├── appsettings.Development.json        (real tenant/client IDs - safe to commit)
└── [User Secrets]                      (client secret - never committed)
    └── AzureAd:ClientSecret
```

### What Gets Committed to Git

✅ **Commit these files**:
- `appsettings.json` (with placeholder values)
- `appsettings.Development.json` (with real Tenant ID and Client ID)

❌ **Never commit**:
- User secrets (stored outside project)
- Client secret values in any file
- Personal credentials or tokens

## Environment Variables (Alternative to User Secrets)

As an alternative to user secrets, you can use environment variables:

### Set Environment Variable

**Windows (PowerShell)**:
```powershell
$env:AzureAd__ClientSecret = "your-actual-client-secret-value"
```

**Linux/macOS**:
```bash
export AzureAd__ClientSecret="your-actual-client-secret-value"
```

**Note**: Use double underscores (`__`) to represent colons (`:`) in configuration paths.

### Verify Environment Variable

**Windows (PowerShell)**:
```powershell
echo $env:AzureAd__ClientSecret
```

**Linux/macOS**:
```bash
echo $AzureAd__ClientSecret
```

## IDE Configuration

### Visual Studio 2022

1. **Open Solution**: Open `GhcSamplePs.sln`
2. **Set Startup Project**: Right-click `GhcSamplePs.Web` → "Set as Startup Project"
3. **Run**: Press F5 or click "Run"
4. **User Secrets**: Right-click project → "Manage User Secrets" to edit secrets.json directly

### Visual Studio Code

1. **Open Folder**: Open the repository root folder
2. **Install Extensions**: 
   - C# Dev Kit
   - C# (Microsoft)
3. **Run**: Press F5 (select "C#: GhcSamplePs.Web" launch configuration)
4. **User Secrets**: Edit manually using `dotnet user-secrets` commands

### JetBrains Rider

1. **Open Solution**: Open `GhcSamplePs.sln`
2. **Set Startup Project**: Right-click `GhcSamplePs.Web` → "Set as Startup Project"
3. **Run**: Click Run button or Shift+F10
4. **User Secrets**: Rider automatically detects and manages user secrets

## Troubleshooting

### Issue: "Unable to find configuration for 'AzureAd:ClientSecret'"

**Cause**: Client secret not set in user secrets or environment variables

**Solution**:
```bash
cd src/GhcSamplePs.Web
dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"
```

### Issue: "The reply URL does not match"

**Cause**: Redirect URI mismatch between app code and Azure configuration

**Solution**:
1. Verify `CallbackPath` in appsettings matches "/signin-oidc"
2. Verify redirect URI in Azure includes full URL: `https://localhost:5001/signin-oidc`
3. Check for typos, case sensitivity, trailing slashes

### Issue: "Certificate not trusted" browser warning

**Cause**: Development HTTPS certificate not trusted

**Solution**:
```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Issue: "Invalid client secret"

**Cause**: Client secret expired, incorrect, or not accessible

**Solution**:
1. Verify secret in user secrets: `dotnet user-secrets list`
2. Regenerate secret in Azure Portal if expired
3. Update user secrets with new value

### Issue: Application builds but won't start

**Cause**: Port conflict or configuration error

**Solution**:
1. Check if port 5001 is in use: `netstat -ano | findstr 5001` (Windows) or `lsof -i :5001` (Linux/macOS)
2. Stop other applications using the port
3. Check application logs for specific errors

## Next Steps

Once your development environment is configured:

1. ✅ **Verify Infrastructure**: Complete the infrastructure verification checklist
   - See `docs/Infrastructure_Verification_Checklist.md`

2. ✅ **Implement Authentication**: Add authentication middleware and components
   - See specification: `docs/specs/EntraID_ExternalIdentities_Integration_Specification.md`
   - Implementation will be in future development phases

3. ✅ **Test Authentication**: Sign in with test users
   - Use admin@ghcsampleps.onmicrosoft.com
   - Use testuser@ghcsampleps.onmicrosoft.com

## Configuration Reference

### Complete appsettings.Development.json Example

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "ghcsampleps.onmicrosoft.com",
    "TenantId": "12345678-1234-1234-1234-123456789012",
    "ClientId": "87654321-4321-4321-4321-210987654321",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Identity": "Information",
      "Microsoft.Identity.Web": "Information"
    }
  }
}
```

### User Secrets Structure

```json
{
  "AzureAd:ClientSecret": "your-actual-client-secret-value-from-azure-portal"
}
```

## Security Best Practices

- ✅ Never commit secrets to source control
- ✅ Rotate client secrets every 6 months
- ✅ Use separate app registrations for development, staging, and production
- ✅ Use user secrets or environment variables for sensitive data
- ✅ Review .gitignore to ensure secrets are excluded
- ✅ Use HTTPS for all authentication endpoints
- ✅ Keep development certificates up to date

## Resources

- [Safe Storage of App Secrets in Development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [User Secrets in .NET](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-24  
**Maintained By**: Development Team  
**Review Schedule**: When configuration structure changes
