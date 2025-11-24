# Azure Entra ID Infrastructure Verification Checklist

This checklist helps verify that all Azure infrastructure has been correctly set up for Entra ID External Identities integration.

## Purpose

Use this checklist to:
- Verify all Azure resources are created and configured correctly
- Confirm test users can authenticate
- Validate configuration values are documented
- Ensure the setup is ready for application integration

## Pre-Verification Requirements

Before starting verification:
- ✅ All Azure setup steps completed (see `docs/Azure_EntraID_Setup_Guide.md`)
- ✅ Development environment configured (see `docs/Development_Environment_Setup.md`)
- ✅ Access to Azure Portal
- ✅ Test user credentials available

---

## Phase 1: Entra ID Tenant Verification

### 1.1 Tenant Creation
- [ ] Entra ID External Identities tenant created
- [ ] Tenant appears in tenant list in Azure Portal
- [ ] Can switch to the new tenant successfully
- [ ] Tenant Overview page shows correct information

### 1.2 Tenant Information Documented
- [ ] Tenant ID documented in team shared location
- [ ] Domain name documented (e.g., ghcsampleps.onmicrosoft.com)
- [ ] Tenant region/location noted
- [ ] Tenant information added to project documentation

**Verification Steps:**
1. Sign in to Azure Portal
2. Navigate to Microsoft Entra ID
3. Click "Overview"
4. Verify "Tenant ID" and "Primary domain" match documentation

**Expected Values:**
```
Tenant ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx (GUID format)
Domain: [your-tenant].onmicrosoft.com
Tenant Type: Azure AD B2C / External Identities
```

---

## Phase 2: Application Registration Verification

### 2.1 App Registration Created
- [ ] Application registration created in Entra ID
- [ ] Application appears in "App registrations" list
- [ ] Application name is correct: "GhcSamplePs Web Application"
- [ ] Application type is "Web"

### 2.2 Basic Configuration
- [ ] Client ID (Application ID) documented
- [ ] Supported account types set to "Any identity provider or organizational directory"
- [ ] Application status is "Enabled"

### 2.3 Authentication Configuration
- [ ] Development redirect URI added: `https://localhost:5001/signin-oidc`
- [ ] Production redirect URI added: `https://[app-name].azurecontainerapps.io/signin-oidc`
- [ ] Front-channel logout URL configured for development
- [ ] Front-channel logout URL configured for production
- [ ] ID tokens enabled under "Implicit grant and hybrid flows"
- [ ] Public client flows set to "No"

### 2.4 Client Secret
- [ ] Client secret created
- [ ] Secret value copied and stored securely (immediately after creation)
- [ ] Secret expiration date documented
- [ ] Secret stored in Azure Key Vault (production)
- [ ] Secret stored in user secrets (development)
- [ ] Secret NOT committed to source control

### 2.5 API Permissions
- [ ] Microsoft Graph permissions present (default)
- [ ] User.Read delegated permission included
- [ ] Admin consent granted if required

### 2.6 App Roles
- [ ] "Admin" role created
  - Display name: Admin
  - Value: Admin
  - Allowed member types: Users/Groups
  - Status: Enabled
- [ ] "User" role created
  - Display name: User
  - Value: User
  - Allowed member types: Users/Groups
  - Status: Enabled

**Verification Steps:**
1. Go to Azure Portal > Entra ID > App registrations
2. Click "GhcSamplePs Web Application"
3. Verify each section (Authentication, Certificates & secrets, API permissions, App roles)

**Expected Client ID Format:**
```
Client ID: yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy (GUID format)
```

---

## Phase 3: Identity Providers Verification

### 3.1 Microsoft Account Provider
- [ ] Microsoft Account provider enabled
- [ ] Available in identity providers list
- [ ] No configuration errors

### 3.2 Google Provider (if configured)
- [ ] Google provider added to Entra ID
- [ ] Google OAuth client ID configured
- [ ] Google OAuth client secret configured
- [ ] Google credentials documented
- [ ] Redirect URI configured in Google Console

### 3.3 Email Signup (Local Accounts)
- [ ] Email signup enabled
- [ ] Password policy configured
- [ ] Email verification settings configured

**Verification Steps:**
1. Go to Entra ID > Identity providers
2. Verify each provider is listed and shows "Configured" status
3. Click on each provider to verify configuration

**Expected Providers:**
- ✅ Microsoft Account (built-in)
- ✅ Google (if configured)
- ✅ Email signup (local accounts)

---

## Phase 4: User Flows Verification

### 4.1 Sign Up and Sign In Flow
- [ ] User flow created: "signupsignin1" or similar name
- [ ] User flow type: "Sign up and sign in"
- [ ] User flow version: Recommended
- [ ] User flow status: Enabled

### 4.2 Identity Providers in Flow
- [ ] Email signup selected
- [ ] Microsoft Account selected
- [ ] Google selected (if configured)

### 4.3 User Attributes Configuration
- [ ] Email Address attribute collected
- [ ] Display Name attribute collected
- [ ] Given Name attribute collected
- [ ] Surname attribute collected

### 4.4 Token Claims Configuration
- [ ] Email Addresses claim returned
- [ ] Display Name claim returned
- [ ] Given Name claim returned
- [ ] Surname claim returned
- [ ] User's Object ID claim returned

### 4.5 MFA Configuration
- [ ] MFA setting documented (Optional/Required)
- [ ] MFA policy aligns with security requirements

**Verification Steps:**
1. Go to Entra ID > User flows
2. Click on "signupsignin1" user flow
3. Review "Identity providers" section
4. Review "User attributes" section
5. Review "Application claims" section

**Test User Flow:**
1. Click "Run user flow"
2. Select application
3. Click "Run user flow"
4. Verify sign-in page loads correctly
5. Verify identity provider options appear
6. DO NOT complete sign-in yet (save for Phase 6)

---

## Phase 5: Test Users Verification

### 5.1 Admin Test User
- [ ] Admin user created: admin@[tenant].onmicrosoft.com
- [ ] Display name: "Test Admin"
- [ ] Password documented securely
- [ ] User status: Enabled
- [ ] User can sign in to Azure Portal

### 5.2 Regular Test User
- [ ] Regular user created: testuser@[tenant].onmicrosoft.com
- [ ] Display name: "Test User"
- [ ] Password documented securely
- [ ] User status: Enabled
- [ ] User can sign in to Azure Portal

### 5.3 Role Assignments
- [ ] Admin user assigned "Admin" role in Enterprise Applications
- [ ] Regular user assigned "User" role in Enterprise Applications
- [ ] Role assignments visible in "Users and groups" section

**Verification Steps:**

**For Each User:**
1. Go to Entra ID > Users
2. Find and click on user (admin or testuser)
3. Verify user details (name, email, status)
4. Click "Assigned roles" to verify roles

**For Role Assignments:**
1. Go to Entra ID > Enterprise applications
2. Find "GhcSamplePs Web Application"
3. Click "Users and groups"
4. Verify both users are listed with correct roles:
   - Test Admin → Admin role
   - Test User → User role

**Test Sign-In (Azure Portal):**
1. Open browser in incognito/private mode
2. Go to https://portal.azure.com
3. Sign in with admin@[tenant].onmicrosoft.com
4. Enter password (may require password change on first login)
5. Verify successful sign-in
6. Sign out
7. Repeat for testuser@[tenant].onmicrosoft.com

---

## Phase 6: Azure Key Vault Verification

### 6.1 Key Vault Created
- [ ] Key Vault created in correct subscription
- [ ] Key Vault name documented: kv-ghcsampleps-prod
- [ ] Key Vault in correct region (same as Container Apps)
- [ ] Pricing tier: Standard
- [ ] Resource group documented

### 6.2 Access Policies
- [ ] Your user account has access policy (Get, List for Secrets)
- [ ] Can view Key Vault in Azure Portal
- [ ] Access policy configuration documented

### 6.3 Secrets Stored
- [ ] Client secret stored in Key Vault
- [ ] Secret name: "AzureAd--ClientSecret" (note double dash)
- [ ] Secret value matches client secret from app registration
- [ ] Secret version documented
- [ ] Can view secret properties (not value) in portal

### 6.4 Managed Identity Access (for Production)
- [ ] Managed identity configured for Container App (or documented plan)
- [ ] Managed identity has access policy in Key Vault
- [ ] Access policy grants Get and List permissions for secrets

**Verification Steps:**

**Key Vault Basics:**
1. Go to Azure Portal > Key Vaults
2. Click on "kv-ghcsampleps-prod"
3. Verify Overview information (name, location, pricing tier)

**Verify Secrets:**
1. In Key Vault, go to "Secrets"
2. Verify "AzureAd--ClientSecret" appears in list
3. Click on secret name
4. Verify "Current version" shows status "Enabled"
5. Do NOT expose secret value in shared documentation

**Verify Access Policies:**
1. In Key Vault, go to "Access policies"
2. Verify your user account is listed
3. Verify permissions include "Get" and "List" for Secrets
4. If managed identity configured, verify it's listed

**Test Secret Access (Optional):**
```bash
# Azure CLI command to retrieve secret (requires authentication)
az keyvault secret show --name "AzureAd--ClientSecret" --vault-name "kv-ghcsampleps-prod"
```

---

## Phase 7: Configuration Files Verification

### 7.1 appsettings.json
- [ ] File updated with AzureAd section
- [ ] Contains placeholder values (safe to commit)
- [ ] Instance: "https://login.microsoftonline.com/"
- [ ] Domain: placeholder value
- [ ] TenantId: placeholder GUID (00000000-0000-0000-0000-000000000000)
- [ ] ClientId: placeholder GUID
- [ ] CallbackPath: "/signin-oidc"
- [ ] SignedOutCallbackPath: "/signout-callback-oidc"
- [ ] NO client secret in this file

### 7.2 appsettings.Development.json
- [ ] File updated with real Tenant ID
- [ ] File updated with real Client ID
- [ ] File updated with correct Domain
- [ ] NO client secret in this file
- [ ] Safe to commit to source control

### 7.3 User Secrets
- [ ] User secrets initialized for GhcSamplePs.Web project
- [ ] Client secret stored in user secrets
- [ ] Can retrieve secret: `dotnet user-secrets list`
- [ ] User secrets NOT in source control

### 7.4 .gitignore
- [ ] .gitignore includes `**/appsettings.*.json` patterns (if using real values)
- [ ] .gitignore includes user secrets path (should be default)
- [ ] .gitignore includes `*.user` files
- [ ] No secrets accidentally committed

**Verification Steps:**

**Check Configuration Files:**
```bash
# From repository root
cd src/GhcSamplePs.Web

# View appsettings.json (should have placeholders)
cat appsettings.json

# View appsettings.Development.json (should have real Tenant/Client IDs)
cat appsettings.Development.json

# List user secrets (should show Client Secret)
dotnet user-secrets list
```

**Verify Git Status:**
```bash
# From repository root
git status

# Ensure no secrets in tracked files
git diff
```

**Expected appsettings.json Structure:**
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
  "Logging": { ... },
  "AllowedHosts": "*"
}
```

---

## Phase 8: Documentation Verification

### 8.1 Setup Documentation
- [ ] Azure setup guide exists: `docs/Azure_EntraID_Setup_Guide.md`
- [ ] Development setup guide exists: `docs/Development_Environment_Setup.md`
- [ ] Verification checklist exists: `docs/Infrastructure_Verification_Checklist.md`

### 8.2 Configuration Documentation
- [ ] Tenant ID documented in team wiki/shared location
- [ ] Client ID documented
- [ ] Domain documented
- [ ] Key Vault name documented
- [ ] Test user credentials documented (securely)
- [ ] App role names documented

### 8.3 Team Communication
- [ ] Team notified of new infrastructure
- [ ] Access instructions shared with team members
- [ ] Test user credentials shared securely
- [ ] Azure Portal access confirmed for team members

---

## Phase 9: End-to-End Verification

### 9.1 Manual Sign-In Test (Azure Portal)
- [ ] Can sign in as admin user to Azure Portal
- [ ] Can sign in as regular user to Azure Portal
- [ ] User profiles show correct information
- [ ] Role assignments are correct

### 9.2 User Flow Test (Azure Portal)
- [ ] Navigate to user flow in Azure Portal
- [ ] Click "Run user flow"
- [ ] Sign-in page displays correctly
- [ ] Can select identity provider
- [ ] Microsoft Account sign-in works
- [ ] Google sign-in works (if configured)
- [ ] Email signup works
- [ ] User can complete authentication flow

### 9.3 Configuration Test
- [ ] Application builds successfully with new configuration
- [ ] No configuration errors in build output
- [ ] Application starts without errors (even if auth not integrated yet)

**End-to-End Test Steps:**

1. **Test User Flow via Azure Portal:**
   ```
   1. Go to Entra ID > User flows > signupsignin1
   2. Click "Run user flow"
   3. Select "GhcSamplePs Web Application"
   4. Click "Run user flow"
   5. In new window, verify sign-in page appears
   6. Verify identity provider options (Microsoft, Google, Email)
   7. Choose "Microsoft Account" or "Email signup"
   8. Complete sign-in with test user credentials
   9. Verify successful authentication (may show error about redirect - this is expected until app code is integrated)
   ```

2. **Test Application Build:**
   ```bash
   cd <repo-root>   # Change to the root of your cloned repository
   dotnet clean
   dotnet build
   # Should build successfully
   ```

3. **Test Application Run:**
   ```bash
   cd src/GhcSamplePs.Web
   dotnet run
   # Should start without errors (Ctrl+C to stop)
   ```

---

## Phase 10: Readiness Checklist

### 10.1 Azure Resources Ready
- [ ] All Azure resources created
- [ ] All resources configured correctly
- [ ] Access permissions granted
- [ ] Test users created and verified

### 10.2 Configuration Ready
- [ ] Configuration files updated
- [ ] Secrets stored securely
- [ ] No secrets in source control
- [ ] Development environment configured

### 10.3 Documentation Ready
- [ ] All setup guides created
- [ ] Configuration values documented
- [ ] Team has access to documentation
- [ ] Troubleshooting information available

### 10.4 Team Ready
- [ ] Team briefed on new infrastructure
- [ ] Team has access to Azure Portal
- [ ] Team has test user credentials
- [ ] Team knows how to configure local environment

---

## Completion Criteria

All items in all phases must be checked before infrastructure setup is considered complete.

### Sign-Off

**Infrastructure Setup Completed By:** ___________________  
**Date:** ___________________  
**Verified By:** ___________________  
**Date:** ___________________

---

## Next Steps After Verification

Once this checklist is complete:

1. ✅ **Begin Code Integration**
   - Add NuGet packages for Microsoft.Identity.Web
   - Configure authentication middleware in Program.cs
   - See: `docs/specs/EntraID_ExternalIdentities_Integration_Specification.md`

2. ✅ **Implement Authentication UI**
   - Create LoginDisplay component
   - Add Authorize attributes to pages
   - Test sign-in flow in application

3. ✅ **Implement Authorization**
   - Define authorization policies
   - Implement role-based access control
   - Test with different user roles

---

## Troubleshooting

If any verification step fails, refer to:
- `docs/Azure_EntraID_Setup_Guide.md` - Troubleshooting section
- `docs/Development_Environment_Setup.md` - Troubleshooting section
- Microsoft Entra ID documentation
- Azure Portal diagnostic logs

## Common Issues During Verification

**Issue**: Cannot find tenant in Azure Portal
- **Solution**: Ensure you're switched to correct directory (check tenant name in top-right)

**Issue**: Client secret doesn't work
- **Solution**: Verify secret was copied correctly immediately after creation; if lost, generate new secret

**Issue**: User cannot sign in
- **Solution**: Verify user is assigned correct role in Enterprise Applications (not just App registrations)

**Issue**: User flow shows error
- **Solution**: Verify redirect URIs are configured in app registration; check for typos

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-24  
**Maintained By**: Infrastructure Team  
**Review Schedule**: After infrastructure changes
