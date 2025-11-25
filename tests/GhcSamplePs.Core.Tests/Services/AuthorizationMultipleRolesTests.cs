using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services;

/// <summary>
/// Comprehensive integration tests for authorization with multiple user roles.
/// These tests verify the authorization system works correctly across different user types
/// and scenarios as required for production readiness.
/// </summary>
public class AuthorizationMultipleRolesTests
{
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<ILogger<AuthorizationService>> _mockLogger;
    private readonly AuthorizationService _service;

    public AuthorizationMultipleRolesTests()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthorizationService>>();
        _service = new AuthorizationService(_mockAuthService.Object, _mockLogger.Object);
    }

    #region Admin User Tests

    [Fact(DisplayName = "Admin user granted access to admin page")]
    public async Task AuthorizeAsync_WhenAdminUser_GrantedAccessToAdminPage()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(result.Succeeded);
        Assert.Null(result.FailureReason);
        Assert.Empty(result.MissingPermissions);
    }

    [Fact(DisplayName = "Admin user has access to all standard pages")]
    public async Task AuthorizeAsync_WhenAdminUser_HasAccessToAllStandardPages()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var authenticatedResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(authenticatedResult.Succeeded);
        Assert.True(userRoleResult.Succeeded);
        Assert.True(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Admin user permissions include admin-specific permissions")]
    public async Task GetUserPermissionsAsync_WhenAdminUser_IncludesAdminPermissions()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.Contains("admin.users", permissions);
        Assert.Contains("admin.settings", permissions);
        Assert.Contains("delete", permissions);
    }

    #endregion

    #region Regular User Tests

    [Fact(DisplayName = "Regular user denied access to admin page")]
    public async Task AuthorizeAsync_WhenRegularUser_DeniedAccessToAdminPage()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(result.Succeeded);
        Assert.NotNull(result.FailureReason);
        Assert.Contains("Admin", result.FailureReason);
        Assert.Contains("Admin", result.MissingPermissions);
    }

    [Fact(DisplayName = "Regular user has access to user pages but not admin pages")]
    public async Task AuthorizeAsync_WhenRegularUser_AccessToUserPagesNotAdminPages()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var authenticatedResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(authenticatedResult.Succeeded);
        Assert.True(userRoleResult.Succeeded);
        Assert.False(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Regular user permissions do not include admin permissions")]
    public async Task GetUserPermissionsAsync_WhenRegularUser_ExcludesAdminPermissions()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.Contains("read", permissions);
        Assert.Contains("write", permissions);
        Assert.DoesNotContain("admin.users", permissions);
        Assert.DoesNotContain("admin.settings", permissions);
        Assert.DoesNotContain("delete", permissions);
    }

    #endregion

    #region Unauthenticated User Tests

    [Fact(DisplayName = "Unauthenticated user redirected - denied all policies")]
    public async Task AuthorizeAsync_WhenUnauthenticated_DeniedAllPolicies()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var authenticatedResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(authenticatedResult.Succeeded);
        Assert.False(userRoleResult.Succeeded);
        Assert.False(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "Unauthenticated user has no permissions")]
    public async Task GetUserPermissionsAsync_WhenUnauthenticated_ReturnsEmpty()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.NotNull(permissions);
        Assert.Empty(permissions);
    }

    [Fact(DisplayName = "Unauthenticated user cannot access any resource")]
    public async Task CanAccessAsync_WhenUnauthenticated_ReturnsFalseForAllResources()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var canAccessAdmin = await _service.CanAccessAsync("admin-resource");
        var canAccessUser = await _service.CanAccessAsync("user-resource");
        var canAccessPublic = await _service.CanAccessAsync("public-resource");

        Assert.False(canAccessAdmin);
        Assert.False(canAccessUser);
        Assert.False(canAccessPublic);
    }

    #endregion

    #region User Without Roles Tests

    [Fact(DisplayName = "User without roles denied role-based policies")]
    public async Task AuthorizeAsync_WhenUserWithoutRoles_DeniedRoleBasedPolicies()
    {
        var noRolesUser = TestUserFactory.CreateCustomUser(
            id: "no-roles-user",
            email: "noroles@test.com",
            displayName: "No Roles User",
            roles: []);
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(noRolesUser);

        var authenticatedResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser);
        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.True(authenticatedResult.Succeeded);
        Assert.False(userRoleResult.Succeeded);
        Assert.False(adminRoleResult.Succeeded);
    }

    [Fact(DisplayName = "User without roles has no permissions")]
    public async Task GetUserPermissionsAsync_WhenUserWithoutRoles_ReturnsEmpty()
    {
        var noRolesUser = TestUserFactory.CreateCustomUser(
            id: "no-roles-user",
            email: "noroles@test.com",
            displayName: "No Roles User",
            roles: []);
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(noRolesUser);

        var permissions = await _service.GetUserPermissionsAsync();

        Assert.NotNull(permissions);
        Assert.Empty(permissions);
    }

    #endregion

    #region Resource Access Tests

    [Fact(DisplayName = "Admin can access any user's resources")]
    public async Task CanAccessAsync_WhenAdmin_CanAccessAnyUserResources()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var canAccessOwnResource = await _service.CanAccessAsync(adminUser.Id);
        var canAccessOtherUserResource = await _service.CanAccessAsync("other-user-id");
        var canAccessRandomResource = await _service.CanAccessAsync("random-resource-123");

        Assert.True(canAccessOwnResource);
        Assert.True(canAccessOtherUserResource);
        Assert.True(canAccessRandomResource);
    }

    [Fact(DisplayName = "Regular user can only access own resources")]
    public async Task CanAccessAsync_WhenRegularUser_CanOnlyAccessOwnResources()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var canAccessOwnResource = await _service.CanAccessAsync(regularUser.Id);
        var canAccessOtherUserResource = await _service.CanAccessAsync("other-user-id");
        var canAccessAdminResource = await _service.CanAccessAsync("admin-test-id");

        Assert.True(canAccessOwnResource);
        Assert.False(canAccessOtherUserResource);
        Assert.False(canAccessAdminResource);
    }

    #endregion

    #region Authorization Result Details Tests

    [Fact(DisplayName = "Authorization failure for regular user includes specific missing permission")]
    public async Task AuthorizeAsync_WhenRegularUserDenied_IncludesSpecificMissingPermission()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);

        Assert.False(result.Succeeded);
        Assert.Single(result.MissingPermissions);
        Assert.Equal("Admin", result.MissingPermissions[0]);
    }

    [Fact(DisplayName = "Authorization failure for user without roles includes both missing roles")]
    public async Task AuthorizeAsync_WhenUserWithoutRolesDenied_IncludesBothMissingRoles()
    {
        var noRolesUser = TestUserFactory.CreateCustomUser(
            id: "no-roles-user",
            email: "noroles@test.com",
            displayName: "No Roles User",
            roles: []);
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(noRolesUser);

        var result = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);

        Assert.False(result.Succeeded);
        Assert.Contains("User", result.MissingPermissions);
        Assert.Contains("Admin", result.MissingPermissions);
    }

    #endregion

    #region Concurrent Authorization Tests

    [Fact(DisplayName = "Multiple concurrent authorization checks for same user")]
    public async Task AuthorizeAsync_WhenConcurrentChecks_AllReturnCorrectResults()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var authTasks = new List<Task<AuthorizationResult>>
        {
            _service.AuthorizeAsync(AuthorizationService.Policies.RequireAuthenticatedUser),
            _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole),
            _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole)
        };

        var results = await Task.WhenAll(authTasks);

        Assert.All(results, result => Assert.True(result.Succeeded));
    }

    #endregion

    #region Role Hierarchy Tests

    [Fact(DisplayName = "Admin role inherits User role privileges")]
    public async Task AuthorizeAsync_WhenAdminRole_InheritsUserRolePrivileges()
    {
        var adminUser = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUser);

        var userRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireUserRole);
        var permissions = await _service.GetUserPermissionsAsync();

        Assert.True(userRoleResult.Succeeded);
        Assert.Contains("read", permissions);
        Assert.Contains("write", permissions);
    }

    [Fact(DisplayName = "User role does not have Admin privileges")]
    public async Task AuthorizeAsync_WhenUserRole_DoesNotHaveAdminPrivileges()
    {
        var regularUser = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regularUser);

        var adminRoleResult = await _service.AuthorizeAsync(AuthorizationService.Policies.RequireAdminRole);
        var permissions = await _service.GetUserPermissionsAsync();

        Assert.False(adminRoleResult.Succeeded);
        Assert.DoesNotContain("admin.users", permissions);
        Assert.DoesNotContain("admin.settings", permissions);
    }

    #endregion
}
