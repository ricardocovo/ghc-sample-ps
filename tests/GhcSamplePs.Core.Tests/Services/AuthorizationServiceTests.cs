using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using GhcSamplePs.Core.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services;

public class AuthorizationServiceTests
{
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<ILogger<AuthorizationService>> _mockLogger;
    private readonly AuthorizationService _service;

    public AuthorizationServiceTests()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockLogger = new Mock<ILogger<AuthorizationService>>();
        _service = new AuthorizationService(_mockAuthService.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "AuthorizeAsync succeeds for authenticated user with RequireAuthenticatedUser policy")]
    public async Task AuthorizeAsync_WhenUserMeetsPolicy_ReturnsSuccess()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("RequireAuthenticatedUser");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync fails when user is not authenticated")]
    public async Task AuthorizeAsync_WhenUserDoesNotMeetPolicy_ReturnsFailure()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _service.AuthorizeAsync("RequireAuthenticatedUser");

        Assert.False(result.Succeeded);
        Assert.NotNull(result.FailureReason);
    }

    [Fact(DisplayName = "AuthorizeAsync succeeds for admin user with RequireAdminRole policy")]
    public async Task AuthorizeAsync_WhenAdminUserWithAdminPolicy_ReturnsSuccess()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("RequireAdminRole");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync fails for regular user with RequireAdminRole policy")]
    public async Task AuthorizeAsync_WhenRegularUserWithAdminPolicy_ReturnsFailure()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("RequireAdminRole");

        Assert.False(result.Succeeded);
        Assert.Contains("Admin", result.MissingPermissions);
    }

    [Fact(DisplayName = "AuthorizeAsync succeeds for user with RequireUserRole policy")]
    public async Task AuthorizeAsync_WhenUserWithUserPolicy_ReturnsSuccess()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("RequireUserRole");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync succeeds for admin with RequireUserRole policy")]
    public async Task AuthorizeAsync_WhenAdminWithUserPolicy_ReturnsSuccess()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.AuthorizeAsync("RequireUserRole");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync throws when policyName is null or empty")]
    public async Task AuthorizeAsync_WhenPolicyNameInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.AuthorizeAsync(null!));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.AuthorizeAsync(string.Empty));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.AuthorizeAsync("   "));
    }

    [Fact(DisplayName = "CanAccessAsync returns true when admin accesses any resource")]
    public async Task CanAccessAsync_WhenUserOwnsResource_ReturnsTrue()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.CanAccessAsync("any-resource-id");

        Assert.True(result);
    }

    [Fact(DisplayName = "CanAccessAsync returns true when user accesses own resource")]
    public async Task CanAccessAsync_WhenUserAccessesOwnResource_ReturnsTrue()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.CanAccessAsync(user.Id);

        Assert.True(result);
    }

    [Fact(DisplayName = "CanAccessAsync returns false when user accesses another's resource")]
    public async Task CanAccessAsync_WhenUserDoesNotOwnResource_ReturnsFalse()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.CanAccessAsync("different-user-id");

        Assert.False(result);
    }

    [Fact(DisplayName = "CanAccessAsync returns false when not authenticated")]
    public async Task CanAccessAsync_WhenNotAuthenticated_ReturnsFalse()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _service.CanAccessAsync("resource-id");

        Assert.False(result);
    }

    [Fact(DisplayName = "CanAccessAsync throws when resourceId is null or empty")]
    public async Task CanAccessAsync_WhenResourceIdInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.CanAccessAsync(null!));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.CanAccessAsync(string.Empty));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.CanAccessAsync("   "));
    }

    [Fact(DisplayName = "GetUserPermissionsAsync returns admin permissions for admin user")]
    public async Task GetUserPermissionsAsync_WhenAdminUser_ReturnsAdminPermissions()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetUserPermissionsAsync();

        Assert.NotNull(result);
        Assert.Contains("admin.users", result);
        Assert.Contains("admin.settings", result);
    }

    [Fact(DisplayName = "GetUserPermissionsAsync returns user permissions for regular user")]
    public async Task GetUserPermissionsAsync_WhenRegularUser_ReturnsUserPermissions()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetUserPermissionsAsync();

        Assert.NotNull(result);
        Assert.Contains("read", result);
        Assert.Contains("write", result);
        Assert.DoesNotContain("admin.users", result);
    }

    [Fact(DisplayName = "GetUserPermissionsAsync returns empty list when not authenticated")]
    public async Task GetUserPermissionsAsync_WhenNotAuthenticated_ReturnsEmptyList()
    {
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _service.GetUserPermissionsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "AuthorizeAsync with resource checks policy first")]
    public async Task AuthorizeAsync_WithResource_ChecksPolicyFirst()
    {
        var user = TestUserFactory.CreateRegularUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var resource = new object();

        var result = await _service.AuthorizeAsync(resource, "RequireAdminRole");

        Assert.False(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync with resource succeeds when policy met")]
    public async Task AuthorizeAsync_WithResourceWhenPolicyMet_ReturnsSuccess()
    {
        var user = TestUserFactory.CreateAdminUser();
        _mockAuthService.Setup(s => s.GetCurrentUserAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        var resource = new object();

        var result = await _service.AuthorizeAsync(resource, "RequireAdminRole");

        Assert.True(result.Succeeded);
    }

    [Fact(DisplayName = "AuthorizeAsync with resource throws when resource is null")]
    public async Task AuthorizeAsync_WithResourceNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.AuthorizeAsync(null!, "RequireAdminRole"));
    }

    [Fact(DisplayName = "Constructor throws when authenticationService is null")]
    public void Constructor_WhenAuthenticationServiceNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizationService(null!, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws when logger is null")]
    public void Constructor_WhenLoggerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthorizationService(_mockAuthService.Object, null!));
    }
}
