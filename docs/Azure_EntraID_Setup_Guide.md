# Azure Entra ID External Identities Setup Guide

This guide provides step-by-step instructions for setting up the Azure infrastructure required for Microsoft Entra ID External Identities integration with the GhcSamplePs application.

## Overview

This setup creates the following Azure resources:
- **Entra ID External Identities Tenant** - Identity provider for external users
- **Application Registration** - Represents the GhcSamplePs application in Entra ID
- **App Roles** - Admin and User roles for authorization
- **Identity Providers** - Microsoft Account and Google authentication
- **User Flows** - Sign-up and sign-in experiences
- **Azure Key Vault** - Secure storage for application secrets
- **Test Users** - For development and testing

## Prerequisites

Before starting, ensure you have:

- [ ] Active Azure subscription
- [ ] Owner or Contributor role on the subscription
- [ ] Permission to create Entra ID resources
- [ ] Azure CLI installed (version 2.50.0 or later)
- [ ] .NET 10 SDK installed
- [ ] Access to Google Cloud Console (for Google authentication setup)

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     GhcSamplePs Application                 │
│                    (Azure Container Apps)                   │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │         Authentication Middleware                     │  │
│  │   (Microsoft.Identity.Web + OpenID Connect)          │  │
│  └────────────────┬─────────────────────────────────────┘  │
└───────────────────┼─────────────────────────────────────────┘
                    │
                    │ HTTPS/OAuth 2.0/OIDC
                    │
        ┌───────────▼──────────────────────────────┐
        │   Azure Entra ID External Identities     │
        │                                          │
        │  ┌────────────────────────────────────┐  │
        │  │    Application Registration        │  │
        │  │  - Client ID                       │  │
        │  │  - Client Secret (in Key Vault)    │  │
        │  │  - Redirect URIs                   │  │
        │  │  - App Roles (Admin, User)         │  │
        │  └────────────────────────────────────┘  │
        │                                          │
        │  ┌────────────────────────────────────┐  │
        │  │    Identity Providers              │  │
        │  │  - Microsoft Account               │  │
        │  │  - Google                          │  │
        │  └────────────────────────────────────┘  │
        │                                          │
        │  ┌────────────────────────────────────┐  │
        │  │    User Flows                      │  │
        │  │  - Sign-up and Sign-in             │  │
        │  └────────────────────────────────────┘  │
        └──────────────────────────────────────────┘
                    │
                    │ Secret Access
                    │
        ┌───────────▼──────────────────────────────┐
        │         Azure Key Vault                  │
        │  - Client Secret                         │
        │  - Connection Strings (if needed)        │
        │  - Other sensitive configuration         │
        └──────────────────────────────────────────┘
```

## Phase 1: Create Entra ID External Identities Tenant

### Step 1.1: Create the Tenant

1. **Sign in to Azure Portal**
   - Navigate to [https://portal.azure.com](https://portal.azure.com)
   - Sign in with your Azure credentials

2. **Navigate to Entra ID**
   - Search for "Microsoft Entra ID" in the top search bar
   - Click on "Microsoft Entra ID" service

3. **Create External Identities Tenant**
   - Click on "Manage tenants" at the top
   - Click "+ Create"
   - Select "Azure AD B2C" (External Identities)
   - Click "Continue"

4. **Configure Tenant**
   - **Organization name**: `GhcSamplePs External`
   - **Initial domain name**: `ghcsampleps` (will become ghcsampleps.onmicrosoft.com)
   - **Country/Region**: Select your region (e.g., United States)
   - Click "Review + create"
   - Click "Create"

5. **Wait for Deployment**
   - Deployment typically takes 1-2 minutes
   - Once complete, click "Go to resource"

6. **Document Tenant Information**
   - Copy the **Tenant ID** (found in Overview page)
   - Copy the **Domain name** (e.g., ghcsampleps.onmicrosoft.com)
   - Save these values - you'll need them later

   ```
   Tenant ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
   Domain: ghcsampleps.onmicrosoft.com
   ```

### Step 1.2: Switch to the New Tenant

1. **Switch Directory**
   - Click on your profile icon in top-right corner
   - Click "Switch directory"
   - Select the newly created "GhcSamplePs External" tenant
   - Confirm you're now in the correct tenant (check tenant name in top bar)

## Phase 2: Register Application

### Step 2.1: Create App Registration

1. **Navigate to App Registrations**
   - In the Entra ID portal, go to "App registrations"
   - Click "+ New registration"

2. **Configure Application**
   - **Name**: `GhcSamplePs Web Application`
   - **Supported account types**: 
     - Select "Accounts in any identity provider or organizational directory (for authenticating users with user flows)"
   - **Redirect URI**: 
     - Platform: "Web"
     - URI: `https://localhost:5001/signin-oidc` (development)
   - Click "Register"

3. **Document Application Information**
   - Copy the **Application (client) ID** from the Overview page
   - Save this value:

   ```
   Client ID: yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
   ```

### Step 2.2: Configure Authentication Settings

1. **Add Additional Redirect URIs**
   - Go to "Authentication" in the left menu
   - Under "Platform configurations" > "Web" > "Redirect URIs", click "Add URI"
   - Add production URI: `https://your-app-name.azurecontainerapps.io/signin-oidc`
   - Add staging URI if needed: `https://your-app-name-staging.azurecontainerapps.io/signin-oidc`

2. **Configure Front-channel Logout**
   - Under "Authentication" > "Front-channel logout URL"
   - Add: `https://localhost:5001/signout-oidc` (development)
   - Add: `https://your-app-name.azurecontainerapps.io/signout-oidc` (production)

3. **Configure Token Configuration**
   - Stay in "Authentication" section
   - Under "Implicit grant and hybrid flows"
   - **Check**: "ID tokens (used for implicit and hybrid flows)"
   - Click "Save"

4. **Configure Advanced Settings**
   - Under "Authentication" > "Advanced settings"
   - **Allow public client flows**: No
   - Click "Save"

### Step 2.3: Create Client Secret

1. **Navigate to Certificates & Secrets**
   - Click "Certificates & secrets" in left menu
   - Click "Client secrets" tab
   - Click "+ New client secret"

2. **Create Secret**
   - **Description**: `GhcSamplePs Development Secret`
   - **Expires**: 6 months (or as per your security policy)
   - Click "Add"

3. **Copy Secret Immediately**
   - **IMPORTANT**: Copy the secret **Value** immediately - it won't be shown again
   - Save it securely:

   ```
   Client Secret: your-secret-value-here
   ```

   **Security Warning**: Never commit this secret to source control!

### Step 2.4: Configure API Permissions

1. **Navigate to API Permissions**
   - Click "API permissions" in left menu
   - Default permissions should include:
     - Microsoft Graph > User.Read (Delegated)

2. **Verify Permissions**
   - These default permissions are sufficient for basic authentication
   - Additional permissions can be added later if needed

### Step 2.5: Create App Roles

1. **Navigate to App Roles**
   - Click "App roles" in left menu
   - Click "+ Create app role"

2. **Create Admin Role**
   - **Display name**: `Admin`
   - **Allowed member types**: "Users/Groups"
   - **Value**: `Admin`
   - **Description**: `Administrators with full access to the application`
   - **Enable this app role**: Checked
   - Click "Apply"

3. **Create User Role**
   - Click "+ Create app role" again
   - **Display name**: `User`
   - **Allowed member types**: "Users/Groups"
   - **Value**: `User`
   - **Description**: `Standard users with basic access to the application`
   - **Enable this app role**: Checked
   - Click "Apply"

4. **Verify Roles**
   - Confirm both "Admin" and "User" roles are listed and enabled

## Phase 3: Configure Identity Providers

### Step 3.1: Configure Microsoft Account Provider

1. **Navigate to Identity Providers**
   - In the Entra ID tenant portal, go to "Identity providers"
   - Click on "Microsoft Account"

2. **Enable Microsoft Account**
   - The Microsoft Account provider should be available by default
   - If not listed, click "+ New Microsoft Account"
   - No additional configuration needed
   - Microsoft Account is built-in and doesn't require separate app registration

### Step 3.2: Configure Google Provider

**Prerequisites**: You need a Google Cloud project and OAuth 2.0 credentials

1. **Create Google OAuth Credentials**
   - Go to [Google Cloud Console](https://console.cloud.google.com)
   - Create a new project or select existing one
   - Navigate to "APIs & Services" > "Credentials"
   - Click "+ Create Credentials" > "OAuth client ID"

2. **Configure OAuth Consent Screen** (if not already done)
   - User Type: External
   - App name: `GhcSamplePs`
   - User support email: Your email
   - Developer contact: Your email
   - Save and continue through the steps

3. **Create OAuth Client ID**
   - Application type: "Web application"
   - Name: `GhcSamplePs Web`
   - Authorized redirect URIs: 
     - `https://ghcsampleps.b2clogin.com/ghcsampleps.onmicrosoft.com/oauth2/authresp`
   - Click "Create"
   - Copy the **Client ID** and **Client Secret**

4. **Add Google Provider to Entra ID**
   - Return to Azure Portal > Entra ID > Identity providers
   - Click "+ Google"
   - **Client ID**: Paste Google OAuth Client ID
   - **Client Secret**: Paste Google OAuth Client Secret
   - Click "Save"

5. **Document Google Credentials**
   ```
   Google Client ID: your-google-client-id
   Google Client Secret: your-google-client-secret
   ```

## Phase 4: Create User Flows

### Step 4.1: Create Sign Up and Sign In Flow

1. **Navigate to User Flows**
   - In Entra ID portal, go to "User flows"
   - Click "+ New user flow"

2. **Select Flow Type**
   - Select "Sign up and sign in"
   - Select "Recommended" version
   - Click "Create"

3. **Configure User Flow**
   - **Name**: `signupsignin1`
   - **Identity providers**: 
     - Check "Email signup"
     - Check "Microsoft Account"
     - Check "Google" (if configured)
   
4. **Select User Attributes**
   - Multifactor authentication: "Optional" (or "Required" for production)
   - User attributes and claims:
     - **Collect attributes** (during sign-up):
       - ☑ Email Address
       - ☑ Display Name
       - ☑ Given Name
       - ☑ Surname
     - **Return claims** (in token):
       - ☑ Email Addresses
       - ☑ Display Name
       - ☑ Given Name
       - ☑ Surname
       - ☑ User's Object ID
   - Click "Create"

5. **Test User Flow**
   - Once created, click on the user flow
   - Click "Run user flow"
   - Select your application
   - Click "Run user flow" to test the sign-up/sign-in experience

## Phase 5: Create Test Users

### Step 5.1: Create Admin Test User

1. **Navigate to Users**
   - In Entra ID portal, go to "Users"
   - Click "+ New user"
   - Select "Create new user"

2. **Configure Admin User**
   - **User principal name**: `admin@ghcsampleps.onmicrosoft.com`
   - **Display name**: `Test Admin`
   - **Password**: Click "Auto-generate password" and copy it
   - **First name**: `Test`
   - **Last name**: `Admin`
   - Click "Create"

3. **Assign Admin Role**
   - Go to the created user
   - Click "Assigned roles"
   - Click "+ Add assignments"
   - Search for "Global Administrator" or use app-specific role
   - Select and add
   - Click "Add"

4. **Document Admin Credentials**
   ```
   Admin User: admin@ghcsampleps.onmicrosoft.com
   Password: [auto-generated - saved securely]
   ```

### Step 5.2: Create Regular Test User

1. **Create User**
   - Go to "Users" > "+ New user"
   - **User principal name**: `testuser@ghcsampleps.onmicrosoft.com`
   - **Display name**: `Test User`
   - **Password**: Auto-generate and copy
   - **First name**: `Test`
   - **Last name**: `User`
   - Click "Create"

2. **Document User Credentials**
   ```
   Test User: testuser@ghcsampleps.onmicrosoft.com
   Password: [auto-generated - saved securely]
   ```

### Step 5.3: Assign App Roles to Users

1. **Navigate to Enterprise Applications**
   - In Entra ID portal, go to "Enterprise applications"
   - Find and click on "GhcSamplePs Web Application"

2. **Assign Admin Role**
   - Click "Users and groups"
   - Click "+ Add user/group"
   - Under "Users", click "None Selected"
   - Search for and select "Test Admin"
   - Under "Select a role", choose "Admin"
   - Click "Assign"

3. **Assign User Role**
   - Click "+ Add user/group" again
   - Select "Test User"
   - Under "Select a role", choose "User"
   - Click "Assign"

## Phase 6: Set Up Azure Key Vault

### Step 6.1: Create Key Vault

1. **Create Resource**
   - In Azure Portal, search for "Key vaults"
   - Click "+ Create"

2. **Configure Key Vault**
   - **Subscription**: Select your subscription
   - **Resource group**: Create new or select existing (e.g., `rg-ghcsampleps-prod`)
   - **Key vault name**: `kv-ghcsampleps-prod` (must be globally unique)
   - **Region**: Same as your Container Apps (e.g., East US)
   - **Pricing tier**: Standard
   - Click "Review + create"
   - Click "Create"

3. **Configure Access Policy**
   - Once created, go to the Key Vault
   - Click "Access policies"
   - Click "+ Create"
   - Select permissions:
     - **Secret permissions**: Get, List
   - Under "Principal", search for and select your Azure AD user
   - Click "Review + create"
   - Click "Create"

### Step 6.2: Add Secrets to Key Vault

1. **Add Client Secret**
   - In Key Vault, go to "Secrets"
   - Click "+ Generate/Import"
   - **Name**: `AzureAd--ClientSecret` (note the double dash)
   - **Value**: Paste the client secret from Step 2.3
   - Click "Create"

2. **Verify Secret**
   - Confirm the secret appears in the list
   - Click on it to view details (but don't expose the value in shared documentation)

### Step 6.3: Configure Managed Identity Access (For Production)

When deploying to Azure Container Apps:

1. **Create Managed Identity**
   - Your Container App will have a system-assigned managed identity
   - Document the Object (principal) ID for reference

2. **Grant Key Vault Access**
   - In Key Vault > "Access policies"
   - Click "+ Create"
   - Select permissions: Get, List (for Secrets)
   - Under "Principal", search for your Container App's managed identity
   - Click "Create"

## Configuration Summary

After completing all phases, you should have:

### Entra ID Configuration
```
Tenant ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
Domain: ghcsampleps.onmicrosoft.com
Client ID: yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy
Client Secret: [stored in Key Vault]
User Flow: signupsignin1
```

### Identity Providers
- ✅ Microsoft Account (built-in)
- ✅ Google (configured)
- ✅ Email signup (local accounts)

### App Roles
- ✅ Admin role (created)
- ✅ User role (created)

### Test Users
- ✅ admin@ghcsampleps.onmicrosoft.com (Admin role)
- ✅ testuser@ghcsampleps.onmicrosoft.com (User role)

### Azure Key Vault
- ✅ Key Vault created
- ✅ Client secret stored
- ✅ Access policies configured

## Next Steps

Now that the Azure infrastructure is set up:

1. **Configure Application**
   - Update `appsettings.json` with Entra ID configuration
   - Configure user secrets for local development
   - See `docs/Development_Environment_Setup.md`

2. **Verify Setup**
   - Test sign-in through Azure Portal
   - Verify user flows work correctly
   - Confirm roles are assigned
   - See `docs/Infrastructure_Verification_Checklist.md`

3. **Integrate with Application**
   - Add required NuGet packages
   - Configure authentication middleware
   - Implement authorization policies
   - See specification: `docs/specs/EntraID_ExternalIdentities_Integration_Specification.md`

## Troubleshooting

### Common Issues

**Issue**: "AADSTS50011: The reply URL specified in the request does not match"
- **Solution**: Verify redirect URI in app registration matches exactly (including case and trailing slashes)

**Issue**: "AADSTS700016: Application not found"
- **Solution**: Ensure you're using the correct Client ID and are in the correct tenant

**Issue**: Cannot access Key Vault secrets
- **Solution**: Verify access policies are configured correctly and managed identity has permissions

**Issue**: Google authentication not working
- **Solution**: Verify redirect URI in Google Console matches the exact Entra ID callback URL

**Issue**: Roles not appearing in tokens
- **Solution**: Ensure roles are assigned to users through Enterprise Applications, not App Registrations

## Security Considerations

- ✅ Client secret stored only in Key Vault (production) and user secrets (development)
- ✅ Never commit secrets to source control
- ✅ Use HTTPS for all redirect URIs
- ✅ Regularly rotate client secrets (every 6 months recommended)
- ✅ Review access policies quarterly
- ✅ Monitor sign-in logs for suspicious activity
- ✅ Enable MFA for production environments

## Resources

- [Microsoft Entra ID External Identities Documentation](https://learn.microsoft.com/en-us/entra/external-id/)
- [Azure AD B2C User Flows](https://learn.microsoft.com/en-us/azure/active-directory-b2c/user-flow-overview)
- [Azure Key Vault Best Practices](https://learn.microsoft.com/en-us/azure/key-vault/general/best-practices)
- [Secure ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-24  
**Maintained By**: Infrastructure Team  
**Review Schedule**: Quarterly or when infrastructure changes
