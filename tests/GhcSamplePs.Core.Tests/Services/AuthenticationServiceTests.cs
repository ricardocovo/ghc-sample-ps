using System.Security.Claims;
using GhcSamplePs.Core.Services.Implementations;
using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GhcSamplePs.Core.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<ICurrentUserProvider> _mockUserProvider;
    private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _mockUserProvider = new Mock<ICurrentUserProvider>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
        _service = new AuthenticationService(_mockUserProvider.Object, _mockLogger.Object);
    }

    [Fact(DisplayName = "GetCurrentUserAsync returns user when authenticated")]
    public async Task GetCurrentUserAsync_WhenUserAuthenticated_ReturnsUserInformation()
    {
        var claims = CreateTestClaims("test-user-id", "test@example.com", "Test User");
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.GetCurrentUserAsync();

        Assert.NotNull(result);
        Assert.Equal("test-user-id", result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("Test User", result.DisplayName);
    }

    [Fact(DisplayName = "GetCurrentUserAsync returns null when not authenticated")]
    public async Task GetCurrentUserAsync_WhenUserNotAuthenticated_ReturnsNull()
    {
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns((ClaimsPrincipal?)null);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact(DisplayName = "GetCurrentUserAsync returns null for unauthenticated identity")]
    public async Task GetCurrentUserAsync_WhenIdentityNotAuthenticated_ReturnsNull()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact(DisplayName = "GetUserRolesAsync returns roles when user has roles")]
    public async Task GetUserRolesAsync_WhenUserHasRoles_ReturnsRoleList()
    {
        var claims = CreateTestClaimsWithRoles("test-id", "test@test.com", "Test", ["Admin", "User"]);
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.GetUserRolesAsync();

        Assert.NotNull(result);
        Assert.Contains("Admin", result);
        Assert.Contains("User", result);
    }

    [Fact(DisplayName = "GetUserRolesAsync returns empty list when not authenticated")]
    public async Task GetUserRolesAsync_WhenNotAuthenticated_ReturnsEmptyList()
    {
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns((ClaimsPrincipal?)null);

        var result = await _service.GetUserRolesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "IsInRoleAsync returns true when user in role")]
    public async Task IsInRoleAsync_WhenUserInRole_ReturnsTrue()
    {
        var claims = CreateTestClaimsWithRoles("test-id", "test@test.com", "Test", ["Admin"]);
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.IsInRoleAsync("Admin");

        Assert.True(result);
    }

    [Fact(DisplayName = "IsInRoleAsync returns false when user not in role")]
    public async Task IsInRoleAsync_WhenUserNotInRole_ReturnsFalse()
    {
        var claims = CreateTestClaimsWithRoles("test-id", "test@test.com", "Test", ["User"]);
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.IsInRoleAsync("Admin");

        Assert.False(result);
    }

    [Fact(DisplayName = "IsInRoleAsync returns false when not authenticated")]
    public async Task IsInRoleAsync_WhenNotAuthenticated_ReturnsFalse()
    {
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns((ClaimsPrincipal?)null);

        var result = await _service.IsInRoleAsync("Admin");

        Assert.False(result);
    }

    [Fact(DisplayName = "IsInRoleAsync throws when roleName is null or empty")]
    public async Task IsInRoleAsync_WhenRoleNameNullOrEmpty_ThrowsArgumentException()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.IsInRoleAsync(null!));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.IsInRoleAsync(string.Empty));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.IsInRoleAsync("   "));
    }

    [Fact(DisplayName = "HasClaimAsync returns true when user has claim")]
    public async Task HasClaimAsync_WhenUserHasClaim_ReturnsTrue()
    {
        var claims = new List<Claim>
        {
            new("sub", "test-id"),
            new("email", "test@test.com"),
            new("email_verified", "true")
        };
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.HasClaimAsync("email_verified", "true");

        Assert.True(result);
    }

    [Fact(DisplayName = "HasClaimAsync returns false when user does not have claim")]
    public async Task HasClaimAsync_WhenUserDoesNotHaveClaim_ReturnsFalse()
    {
        var claims = CreateTestClaims("test-id", "test@test.com", "Test");
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.HasClaimAsync("nonexistent", "value");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasClaimAsync returns false when not authenticated")]
    public async Task HasClaimAsync_WhenNotAuthenticated_ReturnsFalse()
    {
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns((ClaimsPrincipal?)null);

        var result = await _service.HasClaimAsync("email_verified", "true");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasClaimAsync throws when parameters are null or empty")]
    public async Task HasClaimAsync_WhenParametersInvalid_ThrowsArgumentException()
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.HasClaimAsync(null!, "value"));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.HasClaimAsync("type", null!));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.HasClaimAsync("", "value"));
        await Assert.ThrowsAnyAsync<ArgumentException>(() => _service.HasClaimAsync("type", ""));
    }

    [Fact(DisplayName = "GetUserClaimsAsync returns claims dictionary when authenticated")]
    public async Task GetUserClaimsAsync_WhenAuthenticated_ReturnsClaimsDictionary()
    {
        var claims = new List<Claim>
        {
            new("sub", "test-id"),
            new("email", "test@test.com"),
            new("name", "Test User")
        };
        var principal = CreateAuthenticatedPrincipal(claims);
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns(principal);

        var result = await _service.GetUserClaimsAsync();

        Assert.NotNull(result);
        Assert.True(result.ContainsKey("sub"));
        Assert.Equal("test-id", result["sub"]);
    }

    [Fact(DisplayName = "GetUserClaimsAsync returns empty dictionary when not authenticated")]
    public async Task GetUserClaimsAsync_WhenNotAuthenticated_ReturnsEmptyDictionary()
    {
        _mockUserProvider.Setup(p => p.GetCurrentUser()).Returns((ClaimsPrincipal?)null);

        var result = await _service.GetUserClaimsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "Constructor throws when currentUserProvider is null")]
    public void Constructor_WhenCurrentUserProviderNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthenticationService(null!, _mockLogger.Object));
    }

    [Fact(DisplayName = "Constructor throws when logger is null")]
    public void Constructor_WhenLoggerNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => 
            new AuthenticationService(_mockUserProvider.Object, null!));
    }

    private static List<Claim> CreateTestClaims(string id, string email, string name)
    {
        return
        [
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", id),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", email),
            new Claim("name", name)
        ];
    }

    private static List<Claim> CreateTestClaimsWithRoles(string id, string email, string name, string[] roles)
    {
        var claims = CreateTestClaims(id, email, name);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private static ClaimsPrincipal CreateAuthenticatedPrincipal(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}
