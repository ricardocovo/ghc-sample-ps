using GhcSamplePs.Core.Models.Identity;

namespace GhcSamplePs.Core.Tests.TestHelpers;

/// <summary>
/// Test helper class for creating ApplicationUser instances for testing.
/// </summary>
public static class TestUserFactory
{
    /// <summary>
    /// Creates a test admin user with standard admin roles and permissions.
    /// </summary>
    /// <returns>An ApplicationUser configured as an admin.</returns>
    public static ApplicationUser CreateAdminUser()
    {
        return new ApplicationUser
        {
            Id = "admin-test-id",
            Email = "admin@test.com",
            DisplayName = "Test Admin",
            GivenName = "Test",
            FamilyName = "Admin",
            Roles = new List<string> { "Admin", "User" },
            Claims = new Dictionary<string, string>
            {
                { "email_verified", "true" },
                { "profile_complete", "true" }
            },
            IsActive = true,
            LastLoginDate = DateTime.UtcNow.AddHours(-1),
            CreatedDate = DateTime.UtcNow.AddDays(-30)
        };
    }

    /// <summary>
    /// Creates a test regular user with standard user role.
    /// </summary>
    /// <returns>An ApplicationUser configured as a regular user.</returns>
    public static ApplicationUser CreateRegularUser()
    {
        return new ApplicationUser
        {
            Id = "user-test-id",
            Email = "user@test.com",
            DisplayName = "Test User",
            GivenName = "Test",
            FamilyName = "User",
            Roles = new List<string> { "User" },
            Claims = new Dictionary<string, string>
            {
                { "email_verified", "true" },
                { "profile_complete", "true" }
            },
            IsActive = true,
            LastLoginDate = DateTime.UtcNow.AddMinutes(-30),
            CreatedDate = DateTime.UtcNow.AddDays(-15)
        };
    }

    /// <summary>
    /// Creates a test user with an incomplete profile.
    /// </summary>
    /// <returns>An ApplicationUser with incomplete profile.</returns>
    public static ApplicationUser CreateIncompleteProfileUser()
    {
        return new ApplicationUser
        {
            Id = "newuser-test-id",
            Email = "newuser@test.com",
            DisplayName = "New User",
            GivenName = "New",
            FamilyName = "User",
            Roles = new List<string> { "User" },
            Claims = new Dictionary<string, string>
            {
                { "email_verified", "true" },
                { "profile_complete", "false" }
            },
            IsActive = true,
            LastLoginDate = DateTime.UtcNow.AddMinutes(-5),
            CreatedDate = DateTime.UtcNow.AddHours(-1)
        };
    }

    /// <summary>
    /// Creates a test user with custom properties for specific test scenarios.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="email">User email.</param>
    /// <param name="displayName">User display name.</param>
    /// <param name="roles">User roles.</param>
    /// <param name="claims">User claims.</param>
    /// <param name="isActive">Whether the user is active.</param>
    /// <returns>A customized ApplicationUser.</returns>
    public static ApplicationUser CreateCustomUser(
        string id,
        string email,
        string displayName,
        IReadOnlyList<string>? roles = null,
        IReadOnlyDictionary<string, string>? claims = null,
        bool isActive = true)
    {
        return new ApplicationUser
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            Roles = roles ?? [],
            Claims = claims ?? new Dictionary<string, string>(),
            IsActive = isActive,
            CreatedDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an inactive test user.
    /// </summary>
    /// <returns>An inactive ApplicationUser.</returns>
    public static ApplicationUser CreateInactiveUser()
    {
        return new ApplicationUser
        {
            Id = "inactive-test-id",
            Email = "inactive@test.com",
            DisplayName = "Inactive User",
            Roles = new List<string> { "User" },
            Claims = new Dictionary<string, string>(),
            IsActive = false,
            CreatedDate = DateTime.UtcNow.AddDays(-60)
        };
    }
}
