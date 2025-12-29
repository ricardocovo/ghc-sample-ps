using GhcSamplePs.Web.Services;

namespace GhcSamplePs.Web.Helpers;

/// <summary>
/// Helper class for managing authentication in Playwright E2E tests.
/// Provides methods to programmatically set up user authentication states.
/// </summary>
public static class AuthenticationHelper
{
    /// <summary>
    /// Sets up an authenticated user with specified attributes and roles.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="displayName">The user's display name.</param>
    /// <param name="roles">The roles to assign to the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAuthenticatedUserAsync(IServiceProvider services,
        string userId = "test-user",
        string email = "test@example.com",
        string displayName = "Test User",
        params string[] roles)
    {
        var testProvider = services.GetRequiredService<TestCurrentUserProvider>();
        testProvider.SetTestUser(userId, email, displayName, roles);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets up an admin user with admin and user roles.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAdminUserAsync(IServiceProvider services)
    {
        var testProvider = services.GetRequiredService<TestCurrentUserProvider>();
        testProvider.SetAdminUser();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets up a regular user with user role only.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetRegularUserAsync(IServiceProvider services)
    {
        var testProvider = services.GetRequiredService<TestCurrentUserProvider>();
        testProvider.SetRegularUser();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears authentication, setting the user to unauthenticated state.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task ClearAuthenticationAsync(IServiceProvider services)
    {
        var testProvider = services.GetRequiredService<TestCurrentUserProvider>();
        testProvider.ClearUser();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets up a user with specific role for testing authorization scenarios.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="role">The role to assign to the test user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetUserWithRoleAsync(IServiceProvider services, string role)
    {
        return SetAuthenticatedUserAsync(services, $"{role.ToLower()}-user", $"{role.ToLower()}@test.com", $"Test {role}", role);
    }
}
