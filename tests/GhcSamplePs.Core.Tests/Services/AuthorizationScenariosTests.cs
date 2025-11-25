using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services;

/// <summary>
/// Integration-style tests for authorization scenarios with multiple user roles.
/// These tests verify the authorization system works correctly across different user types.
/// </summary>
public class AuthorizationScenariosTests
{
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<ILogger<AuthorizationService>> _mockLogger;
    private readonly AuthorizationService _service;

    public AuthorizationScenariosTests()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthorizationService>>();
        _service = new AuthorizationService(_mockAuthService.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "Anonymous user is denied access to all policies")]
    public async Task AnonymousUser_IsDeniedAccessToAllPolicies()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var authUserResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(authUserResult.Succeeded);
        Assert.False(userRoleResult.Succeeded);
        Assert.False(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Regular user can access authenticated and user policies")]
    public async Task AuthorizeAsync_WhenRegularUser_CanAccessAuthenticatedAndUserPolicies()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authUserResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);

        Assert.True(authUserResult.Succeeded);
        Assert.True(userRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Regular user is denied access to admin policy")]
    public async Task RegularUser_IsDeniedAccessToAdminPolicy()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(result.Succeeded);
        Assert.Contains("Admin", result.MissingPermissions);
    }

    [Fact(DisplayName = "Admin user can access all standard policies")]
    public async Task AuthorizeAsync_WhenAdminUser_CanAccessAllStandardPolicies()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authUserResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(authUserResult.Succeeded);
        Assert.True(userRoleResult.Succeeded);
        Assert.True(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Admin has full permissions including admin-specific")]
    public async Task GetUserPermissionsAsync_WhenAdminUser_HasFullPermissions()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.Contains("read", permissions);
        Assert.Contains("write", permissions);
        Assert.Contains("delete", permissions);
        Assert.Contains("admin.users", permissions);
        Assert.Contains("admin.settings", permissions);
    }

    [Fact(DisplayName = "Regular user has limited permissions")]
    public async Task GetUserPermissionsAsync_WhenRegularUser_HasLimitedPermissions()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.Contains("read", permissions);
        Assert.Contains("write", permissions);
        Assert.DoesNotContain("delete", permissions);
        Assert.DoesNotContain("admin.users", permissions);
        Assert.DoesNotContain("admin.settings", permissions);
    }

    [Fact(DisplayName = "Admin can access any resource")]
    public async Task AdminUser_CanAccessAnyResource()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var canAccessOwn = await _service.CanAccessAsync(user.Id);
        var canAccessOther = await _service.CanAccessAsync("other-user-id");
        var canAccessRandom = await _service.CanAccessAsync("random-resource-123");

        Assert.True(canAccessOwn);
        Assert.True(canAccessOther);
        Assert.True(canAccessRandom);
    }

    [Fact(DisplayName = "Regular user can only access own resources")]
    public async Task CanAccessAsync_WhenRegularUser_CanOnlyAccessOwnResources()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var canAccessOwn = await _service.CanAccessAsync(user.Id);
        var canAccessOther = await _service.CanAccessAsync("other-user-id");

        Assert.True(canAccessOwn);
        Assert.False(canAccessOther);
    }

    [Fact(DisplayName = "Inactive user is treated as authenticated user")]
    public async Task AuthorizeAsync_WhenInactiveUser_IsTreatedAsAuthenticatedUser()
    {
        var user = TestUserFactory.CreateInactiveUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authUserResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);

        Assert.True(authUserResult.Succeeded);
        Assert.True(userRoleResult.Succeeded);
    }

    [Fact(DisplayName = "User without roles fails role-based policies")]
    public async Task AuthorizeAsync_WhenUserWithoutRoles_FailsRoleBasedPolicies()
    {
        var user = TestUserFactory.CreateCustomUser(
            id: "no-roles-user",
            email: "noroles@test.com",
            displayName: "No Roles User",
            roles: []);
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var authUserResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(authUserResult.Succeeded);
        Assert.False(userRoleResult.Succeeded);
        Assert.False(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Resource-based authorization respects admin role")]
    public async Task AuthorizeAsync_WhenResourceBasedAuthorizationForAdmin_RespectsAdminRole()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);
        var resource = new { Id = "some-resource" };

        var result = await _service.AuthorizeAsync(resource, AuthorizationService.Policies.RequireAdminRole);

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Resource-based authorization denies regular user for admin policy")]
    public async Task AuthorizeAsync_WhenResourceBasedAuthorizationForRegularUser_DeniesAdminPolicy()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);
        var resource = new { Id = "some-resource" };

        var result = await _service.AuthorizeAsync(resource, AuthorizationService.Policies.RequireAdminRole);

        Assert.False(result.Succeeded);
    }

    [Theory(DisplayName = "User role case insensitivity for resource access")]
    [InlineData("user-test-id")]
    [InlineData("USER-TEST-ID")]
    [InlineData("User-Test-Id")]
    public async Task CanAccessAsync_UserIdIsCaseInsensitive(string resourceId)
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var canAccess = await _service.CanAccessAsync(resourceId);

        Assert.True(canAccess);
    }

    [Fact(DisplayName = "Custom policy falls back to role check")]
    public async Task AuthorizeAsync_WhenCustomPolicy_FallsBackToRoleCheck()
    {
        var user = TestUserFactory.CreateCustomUser(
            id: "custom-user",
            email: "custom@test.com",
            displayName: "Custom User",
            roles: ["CustomRole"]);
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("CustomRole");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "Authorization failure includes meaningful reason for missing admin role")]
    public async Task AuthorizationFailure_IncludesMeaningfulReason()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(result.Succeeded);
        Assert.NotNull(result.FailureReason);
        Assert.Contains("Admin", result.FailureReason);
    }
}
