# Plan: Make Authentication Test-Ready for Playwright

Configure your Blazor application to support deterministic authentication testing by creating a test-friendly authentication provider that can programmatically set user roles and authentication state without external dependencies.

## Steps

1. Create test authentication infrastructure in Services with TestCurrentUserProvider, TestAuthenticationHandler, and configuration support
2. Add conditional authentication setup in Program.cs to use test providers when environment is "Testing" or config flag is set
3. Create test configuration with appsettings.Testing.json to bypass Entra ID and enable test authentication mode
4. Implement Playwright test helpers in E2E test project for setting up authenticated users, admin users, and clearing authentication state programmatically
5. Add authentication test scenarios covering admin access, regular user access, unauthorized access, and authentication state transitions

## Further Considerations

- Environment isolation - Use ASPNETCORE_ENVIRONMENT=Testing vs configuration flags? Recommend environment-based for cleaner separation
- Test user persistence - Store test users in memory vs database? Recommend in-memory for speed and isolation
- Role management - Hard-coded test roles vs configurable test users? Start with helper methods, expand if needed
- Authentication bypass scope - Bypass all auth vs selective bypass? Recommend selective - keep authorization policies active for testing

This approach lets you test all authentication scenarios (admin, user, anonymous) without external dependencies while maintaining your production Entra ID setup unchanged.
