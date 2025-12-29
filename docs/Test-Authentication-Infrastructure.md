# Test Authentication Infrastructure

This document describes the test-ready authentication infrastructure added to support Playwright E2E testing.

## Overview

The application now supports a test authentication mode that allows E2E tests to programmatically control user authentication state without external dependencies on Microsoft Entra ID.

## Components Added

### 1. TestCurrentUserProvider
Located: `src/GhcSamplePs.Web/Services/TestCurrentUserProvider.cs`

Provides programmatic control over user authentication state for testing:
- `SetTestUser()` - Sets up a user with custom attributes and roles
- `SetAdminUser()` - Quick setup for admin user
- `SetRegularUser()` - Quick setup for regular user
- `ClearUser()` - Sets unauthenticated state

### 2. TestAuthenticationHandler
Located: `src/GhcSamplePs.Web/Services/TestAuthenticationHandler.cs`

ASP.NET Core authentication handler that authenticates users based on TestCurrentUserProvider state.

### 3. AuthenticationHelper
Located: `src/GhcSamplePs.Web/Helpers/AuthenticationHelper.cs`

Static helper class for E2E tests to easily manage authentication:
- `SetAuthenticatedUserAsync()` - General user setup
- `SetAdminUserAsync()` - Admin user setup
- `SetRegularUserAsync()` - Regular user setup
- `ClearAuthenticationAsync()` - Clear authentication
- `SetUserWithRoleAsync()` - User with specific role

### 4. TestAuthController
Located: `src/GhcSamplePs.Web/Controllers/TestAuthController.cs`

API endpoints for verifying authentication state in tests:
- `GET /api/testauth/current-user` - Get current user info
- `GET /api/testauth/is-admin` - Test admin access
- `GET /api/testauth/is-user` - Test user access
- `GET /api/testauth/is-authenticated` - Test authenticated access

## Configuration

### Testing Environment
The test authentication is enabled when:
- `ASPNETCORE_ENVIRONMENT=Testing`, OR
- `Authentication:UseTestProvider=true` in configuration

### appsettings.Testing.json
Created with test-specific configuration:
```json
{
  "Authentication": {
    "UseTestProvider": true,
    "BypassEntraId": true
  }
}
```

## Usage in E2E Tests

### Example Test Setup
```csharp
[Test]
public async Task AdminPage_WithAdminUser_ShowsAdminContent()
{
    // Arrange - Set up admin user
    await AuthenticationHelper.SetAdminUserAsync(Services);

    // Act - Navigate to admin page
    await Page.GotoAsync("/admin");

    // Assert - Verify admin content
    await Expect(Page.Locator("text=Admin Dashboard")).ToBeVisibleAsync();
}
```

### Authentication State Verification
```csharp
// Verify authentication state via test API
var response = await Page.Request.GetAsync("/api/testauth/current-user");
var user = await response.JsonAsync();
Assert.IsTrue(user.IsAuthenticated);
```

## Benefits

1. **No External Dependencies** - Tests run without Entra ID connectivity
2. **Deterministic** - Controllable user roles and authentication state
3. **Fast** - No OAuth flows or token exchanges
4. **Flexible** - Easy to test different user scenarios and edge cases
5. **Isolated** - Each test can set up its own authentication context

## Security

- Test authentication only works in Testing environment
- TestAuthController is hidden from API documentation in production
- Production Entra ID authentication remains unchanged
- Authorization policies remain active for proper access control testing

## Next Steps

1. Create full Playwright E2E test project in `tests/GhcSamplePs.Web.E2E.Tests/`
2. Add test database setup with in-memory providers
3. Implement core authentication test scenarios
4. Add CI/CD pipeline integration for automated E2E testing
