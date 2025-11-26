# Authorization Testing Guide

This guide provides comprehensive documentation for testing the authorization system with multiple user roles in the GhcSamplePs application.

## Overview

The GhcSamplePs application implements role-based access control (RBAC) using Microsoft Entra ID External Identities. This document covers:

- Authorization architecture overview
- Test user creation and role assignment
- Authorization scenarios and expected behaviors
- Unit test coverage
- Manual testing procedures
- Troubleshooting common authorization issues

## Authorization Architecture

### Authorization Policies

The application defines three authorization policies in `Program.cs`:

| Policy Name | Description | Required Role(s) |
|-------------|-------------|------------------|
| `RequireAuthenticatedUser` | Requires any authenticated user | Any authenticated user |
| `RequireAdminRole` | Requires the Admin role | Admin |
| `RequireUserRole` | Requires the User role (or Admin) | User or Admin |

### Application Roles

| Role | Description | Permissions |
|------|-------------|-------------|
| Admin | Full administrative access | read, write, delete, admin.users, admin.settings |
| User | Standard user access | read, write |

### Authorization Components

```
┌────────────────────────────────────────────────────────────┐
│                    Blazor Components                        │
│  (AdminDashboard.razor, MainLayout.razor, etc.)            │
│                                                             │
│  Uses: @attribute [Authorize(Policy = "RequireAdminRole")] │
│        <AuthorizeView Policy="RequireAdminRole">           │
└────────────────────────────┬───────────────────────────────┘
                             │
                             ▼
┌────────────────────────────────────────────────────────────┐
│           Microsoft.AspNetCore.Authorization               │
│           (ASP.NET Core Authorization Middleware)          │
└────────────────────────────┬───────────────────────────────┘
                             │
                             ▼
┌────────────────────────────────────────────────────────────┐
│              GhcSamplePs.Core.Services                     │
│  ┌─────────────────────────────────────────────────────┐  │
│  │           IAuthorizationService                      │  │
│  │  - AuthorizeAsync(policyName)                       │  │
│  │  - AuthorizeAsync(resource, policyName)             │  │
│  │  - CanAccessAsync(resourceId)                       │  │
│  │  - GetUserPermissionsAsync()                        │  │
│  └─────────────────────────────────────────────────────┘  │
│  ┌─────────────────────────────────────────────────────┐  │
│  │           AuthorizationService                       │  │
│  │  - Evaluates policies                               │  │
│  │  - Checks user roles                                │  │
│  │  - Returns AuthorizationResult                      │  │
│  └─────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────┘
```

## Test User Setup in Entra ID

### Required Test Users

Create the following test users in your Entra ID tenant for comprehensive authorization testing:

| User | Email | Roles | Purpose |
|------|-------|-------|---------|
| Test Admin | admin@[tenant].onmicrosoft.com | Admin, User | Test admin access |
| Test User | user@[tenant].onmicrosoft.com | User | Test standard user access |
| No Roles User | noroles@[tenant].onmicrosoft.com | (none) | Test users without roles |

### Step-by-Step: Creating Test Users

#### 1. Create Admin Test User

```
1. Navigate to Azure Portal > Entra ID > Users
2. Click "+ New user" > "Create new user"
3. Configure:
   - User principal name: admin@[tenant].onmicrosoft.com
   - Display name: Test Admin
   - First name: Test
   - Last name: Admin
   - Password: Auto-generate and copy
4. Click "Create"
```

#### 2. Create Standard Test User

```
1. Navigate to Azure Portal > Entra ID > Users
2. Click "+ New user" > "Create new user"
3. Configure:
   - User principal name: user@[tenant].onmicrosoft.com
   - Display name: Test User
   - First name: Test
   - Last name: User
   - Password: Auto-generate and copy
4. Click "Create"
```

#### 3. Create User Without Roles (for testing access denied)

```
1. Navigate to Azure Portal > Entra ID > Users
2. Click "+ New user" > "Create new user"
3. Configure:
   - User principal name: noroles@[tenant].onmicrosoft.com
   - Display name: No Roles User
   - First name: No
   - Last name: Roles
   - Password: Auto-generate and copy
4. Click "Create"
```

### Role Assignment Process

Roles are assigned through the Enterprise Application, not directly in App Registrations.

#### Assign Admin Role

```
1. Navigate to Azure Portal > Entra ID > Enterprise applications
2. Find and click "GhcSamplePs Web Application"
3. Click "Users and groups" in the left menu
4. Click "+ Add user/group"
5. Under "Users", select "Test Admin"
6. Under "Select a role", choose "Admin"
7. Click "Assign"
```

#### Assign User Role

```
1. In the same Enterprise Application
2. Click "+ Add user/group"
3. Under "Users", select "Test User"
4. Under "Select a role", choose "User"
5. Click "Assign"
```

#### Verify Role Assignments

```
1. In Enterprise Application > "Users and groups"
2. Verify the list shows:
   - Test Admin - Admin
   - Test User - User
   - No Roles User - (not listed or no role)
```

## Authorization Test Scenarios

### Scenario Matrix

| Scenario | User Type | Resource | Expected Result |
|----------|-----------|----------|-----------------|
| S1 | Anonymous | Home page | Redirect to login |
| S2 | Anonymous | Admin page | Redirect to login |
| S3 | Admin | Home page | Granted |
| S4 | Admin | Admin page | Granted |
| S5 | Regular User | Home page | Granted |
| S6 | Regular User | Admin page | Access Denied |
| S7 | No Roles User | Home page | Access Denied (RequireUserRole) |
| S8 | No Roles User | Admin page | Access Denied |

### Detailed Test Cases

#### S1: Anonymous User - Home Page Access

**Precondition**: User is not authenticated  
**Action**: Navigate to `/` (home page)  
**Expected**: Redirect to Entra ID sign-in page  
**Rationale**: Default fallback policy requires authenticated user

#### S2: Anonymous User - Admin Page Access

**Precondition**: User is not authenticated  
**Action**: Navigate to `/admin`  
**Expected**: Redirect to Entra ID sign-in page  
**Rationale**: Page has `[Authorize(Policy = "RequireAdminRole")]` attribute

#### S3: Admin User - Home Page Access

**Precondition**: User signed in with Admin role  
**Action**: Navigate to `/` (home page)  
**Expected**: Page renders successfully  
**Verification**: Welcome message displayed

#### S4: Admin User - Admin Page Access

**Precondition**: User signed in with Admin role  
**Action**: Navigate to `/admin`  
**Expected**: Admin Dashboard renders successfully  
**Verification**: 
- Admin Dashboard title visible
- User Management, System Settings, Audit Logs cards visible
- Admin navigation menu visible in sidebar

#### S5: Regular User - Home Page Access

**Precondition**: User signed in with User role  
**Action**: Navigate to `/` (home page)  
**Expected**: Page renders successfully  
**Verification**: Welcome message displayed

#### S6: Regular User - Admin Page Access

**Precondition**: User signed in with User role (not Admin)  
**Action**: Navigate to `/admin`  
**Expected**: Access Denied page displayed  
**Verification**: 
- Redirected to access denied page
- Admin navigation link NOT visible in sidebar

#### S7: User Without Roles - Protected Page Access

**Precondition**: User signed in but has no assigned roles  
**Action**: Navigate to pages requiring RequireUserRole  
**Expected**: Access Denied (if page requires User/Admin role)  
**Verification**: Access denied message displayed

#### S8: User Without Roles - Admin Page Access

**Precondition**: User signed in but has no assigned roles  
**Action**: Navigate to `/admin`  
**Expected**: Access Denied  
**Verification**: Access denied message displayed

## AuthorizeView Component Testing

The application uses `<AuthorizeView>` components for conditional rendering based on authorization.

### MainLayout.razor Authorization

The navigation menu conditionally shows the Admin section:

```razor
<AuthorizeView Policy="RequireAdminRole">
    <Authorized>
        <MudDivider Class="my-2" />
        <MudNavGroup Title="Admin" Icon="@Icons.Material.Filled.AdminPanelSettings" Expanded="true">
            <MudNavLink Href="/admin" Icon="@Icons.Material.Filled.Dashboard" Match="NavLinkMatch.All">Dashboard</MudNavLink>
        </MudNavGroup>
    </Authorized>
</AuthorizeView>
```

### Expected AuthorizeView Behavior

| User Type | Admin Navigation Visible |
|-----------|-------------------------|
| Anonymous | No |
| Admin | Yes |
| Regular User | No |
| No Roles User | No |

## Unit Test Coverage

The authorization system has comprehensive unit test coverage in the `GhcSamplePs.Core.Tests` project.

### Test Classes

#### AuthorizationServiceTests.cs

Tests for the `AuthorizationService` class:

| Test Name | Scenario |
|-----------|----------|
| `AuthorizeAsync_WhenUserMeetsPolicy_ReturnsSuccess` | Authenticated user passes RequireAuthenticatedUser |
| `AuthorizeAsync_WhenUserDoesNotMeetPolicy_ReturnsFailure` | Unauthenticated user fails authorization |
| `AuthorizeAsync_WhenAdminUserWithAdminPolicy_ReturnsSuccess` | Admin user passes RequireAdminRole |
| `AuthorizeAsync_WhenRegularUserWithAdminPolicy_ReturnsFailure` | Regular user fails RequireAdminRole |
| `AuthorizeAsync_WhenUserWithUserPolicy_ReturnsSuccess` | User passes RequireUserRole |
| `AuthorizeAsync_WhenAdminWithUserPolicy_ReturnsSuccess` | Admin passes RequireUserRole |
| `CanAccessAsync_WhenUserOwnsResource_ReturnsTrue` | Admin can access any resource |
| `CanAccessAsync_WhenUserAccessesOwnResource_ReturnsTrue` | User can access own resource |
| `CanAccessAsync_WhenUserDoesNotOwnResource_ReturnsFalse` | User cannot access others' resources |
| `GetUserPermissionsAsync_WhenAdminUser_ReturnsAdminPermissions` | Admin has full permissions |
| `GetUserPermissionsAsync_WhenRegularUser_ReturnsUserPermissions` | User has limited permissions |

#### AuthorizationScenariosTests.cs

Integration-style tests for comprehensive authorization scenarios:

| Test Name | Scenario |
|-----------|----------|
| `AuthorizeAsync_WhenAnonymousUser_DeniesAccessToAllPolicies` | Anonymous access denied |
| `AuthorizeAsync_WhenRegularUser_CanAccessAuthenticatedAndUserPolicies` | User role access |
| `AuthorizeAsync_WhenRegularUser_DeniesAccessToAdminPolicy` | User denied admin access |
| `AuthorizeAsync_WhenAdminUser_CanAccessAllStandardPolicies` | Admin has full access |
| `GetUserPermissionsAsync_WhenAdminUser_HasFullPermissions` | Admin permissions |
| `GetUserPermissionsAsync_WhenRegularUser_HasLimitedPermissions` | User permissions |
| `CanAccessAsync_WhenAdminUser_CanAccessAnyResource` | Admin resource access |
| `CanAccessAsync_WhenRegularUser_CanOnlyAccessOwnResources` | User resource restrictions |
| `AuthorizeAsync_WhenUserWithoutRoles_FailsRoleBasedPolicies` | No roles = no role-based access |

#### AuthorizationResultTests.cs

Tests for the `AuthorizationResult` class:

| Test Name | Scenario |
|-----------|----------|
| `Success_CreatesSuccessfulResult` | Success factory method |
| `Failure_WithReason_CreatesFailedResult` | Failure with reason |
| `Failure_WithMissingPermissions_CreatesFailedResultWithPermissions` | Failure with permissions list |

### Running Tests

```bash
# Run all tests
dotnet test

# Run authorization tests only
dotnet test --filter "FullyQualifiedName~Authorization"

# Run with coverage
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

### Test User Factory

The `TestUserFactory` class in `TestHelpers/` provides methods for creating test users:

```csharp
// Create admin user (has Admin and User roles)
var admin = TestUserFactory.CreateAdminUser();

// Create regular user (has User role only)
var user = TestUserFactory.CreateRegularUser();

// Create user without roles
var noRoles = TestUserFactory.CreateCustomUser(
    id: "no-roles-user",
    email: "noroles@test.com",
    displayName: "No Roles User",
    roles: []);

// Create inactive user
var inactive = TestUserFactory.CreateInactiveUser();
```

## Manual Testing Procedures

### Prerequisites

1. Application running locally (`dotnet run` in `src/GhcSamplePs.Web`)
2. Test users created in Entra ID
3. Role assignments completed

### Test Execution

#### Test 1: Unauthenticated Access

1. Open browser in incognito/private mode
2. Navigate to `https://localhost:5001`
3. **Expected**: Redirect to Entra ID sign-in
4. Navigate to `https://localhost:5001/admin`
5. **Expected**: Redirect to Entra ID sign-in

#### Test 2: Admin User Access

1. Sign in as admin@[tenant].onmicrosoft.com
2. Navigate to home page
3. **Expected**: Welcome page displayed
4. Open navigation menu
5. **Expected**: Admin section visible with Dashboard link
6. Navigate to `/admin`
7. **Expected**: Admin Dashboard displayed with all cards
8. Sign out

#### Test 3: Regular User Access

1. Sign in as user@[tenant].onmicrosoft.com
2. Navigate to home page
3. **Expected**: Welcome page displayed
4. Open navigation menu
5. **Expected**: Admin section NOT visible
6. Navigate directly to `/admin`
7. **Expected**: Access Denied page displayed
8. Sign out

#### Test 4: User Without Roles

1. Sign in as noroles@[tenant].onmicrosoft.com
2. **Expected**: Behavior depends on application policy
   - If FallbackPolicy requires user role: Access Denied to all protected pages
   - If FallbackPolicy only requires authentication: Home page accessible, Admin denied
3. Open navigation menu
4. **Expected**: Admin section NOT visible
5. Navigate directly to `/admin`
6. **Expected**: Access Denied page displayed

### Recording Test Results

Document test results using this template:

```
Test Execution Date: [Date]
Tester: [Name]
Environment: [Local/Staging/Production]

| Test | User | Expected | Actual | Pass/Fail | Notes |
|------|------|----------|--------|-----------|-------|
| T1   | Anonymous | Redirect | Redirect | Pass | |
| T2   | Admin | Dashboard shown | Dashboard shown | Pass | |
| T3   | User | Access Denied | Access Denied | Pass | |
| T4   | No Roles | Access Denied | Access Denied | Pass | |
```

## Troubleshooting

### Common Issues

#### Issue: Roles not appearing in token claims

**Symptoms**: User has role assigned in Entra ID but authorization fails

**Solution**:
1. Verify role is assigned through Enterprise Application (not App Registration)
2. Check if user needs to sign out and sign back in
3. Verify role is enabled in App Roles configuration

#### Issue: AuthorizeView not rendering correctly

**Symptoms**: Content appears regardless of authorization

**Solution**:
1. Verify `CascadingAuthenticationState` is registered in `Program.cs`
2. Check policy name matches exactly (case-sensitive)
3. Ensure `Routes.razor` includes authentication components

#### Issue: Access denied when should be granted

**Symptoms**: User with correct role gets access denied

**Solution**:
1. Check FallbackPolicy configuration in `Program.cs`
2. Verify policy requirements match role values exactly
3. Check for case sensitivity issues in role names

#### Issue: Admin page accessible to non-admin users

**Symptoms**: Regular users can access admin pages

**Solution**:
1. Verify `[Authorize(Policy = "RequireAdminRole")]` attribute on page
2. Check policy definition in `Program.cs`
3. Ensure user doesn't accidentally have Admin role

### Debug Logging

Enable detailed authorization logging by updating `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Authorization": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "GhcSamplePs.Core.Services": "Debug"
    }
  }
}
```

## Security Considerations

### Best Practices

1. **Principle of Least Privilege**: Assign minimum required roles
2. **Regular Audits**: Review role assignments quarterly
3. **Separation of Duties**: Different users for admin and regular testing
4. **Secure Test Credentials**: Store test passwords in secure location
5. **Environment Isolation**: Use separate tenants for dev/staging/production

### Security Checklist

- [ ] Test users have strong passwords
- [ ] Test accounts are disabled in production tenant
- [ ] Role assignments are documented and reviewed
- [ ] Access denied paths are tested thoroughly
- [ ] Token claims are validated correctly
- [ ] Authorization logs are monitored

## References

- [Azure Entra ID Setup Guide](Azure_EntraID_Setup_Guide.md)
- [Development Environment Setup](Development_Environment_Setup.md)
- [Infrastructure Verification Checklist](Infrastructure_Verification_Checklist.md)
- [Microsoft Authorization Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/)
- [Blazor Security Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)

---

**Document Version**: 1.0  
**Last Updated**: 2025-11-25  
**Maintained By**: Development Team  
**Review Schedule**: Quarterly or when authorization changes
