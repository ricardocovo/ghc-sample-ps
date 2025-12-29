using System.Security.Claims;
using GhcSamplePs.Core.Services.Interfaces;

namespace GhcSamplePs.Web.Services;

/// <summary>
/// Test implementation of ICurrentUserProvider for Playwright E2E testing.
/// Allows programmatic control of authentication state and user roles.
/// </summary>
public sealed class TestCurrentUserProvider : ICurrentUserProvider
{
    private ClaimsPrincipal? _currentUser;

    /// <inheritdoc/>
    public ClaimsPrincipal? GetCurrentUser() => _currentUser;

    /// <inheritdoc/>
    public bool IsAuthenticated => _currentUser?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Sets the current user for testing purposes.
    /// </summary>
    /// <param name="user">The claims principal to set as current user.</param>
    public void SetUser(ClaimsPrincipal? user) => _currentUser = user;

    /// <summary>
    /// Sets a test user with specified attributes and roles.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="displayName">The user's display name.</param>
    /// <param name="roles">The roles to assign to the user.</param>
    public void SetTestUser(string userId, string email, string displayName, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", userId),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", email),
            new("name", displayName),
            new("preferred_username", email)
        };

        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        _currentUser = new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Clears the current user, setting authentication state to unauthenticated.
    /// </summary>
    public void ClearUser() => _currentUser = null;

    /// <summary>
    /// Sets up a test admin user with admin privileges.
    /// </summary>
    public void SetAdminUser()
    {
        SetTestUser("admin-user", "admin@test.com", "Test Admin", "Admin", "User");
    }

    /// <summary>
    /// Sets up a test regular user with user privileges.
    /// </summary>
    public void SetRegularUser()
    {
        SetTestUser("regular-user", "user@test.com", "Test User", "User");
    }
}
