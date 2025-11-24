using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.Identity;

public class ApplicationUserTests
{
    [Fact(DisplayName = "ApplicationUser can be created with required properties")]
    public void ApplicationUser_WithRequiredProperties_CreatesSuccessfully()
    {
        var user = new ApplicationUser
        {
            Id = "test-id",
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        Assert.NotNull(user);
        Assert.Equal("test-id", user.Id);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("Test User", user.DisplayName);
    }

    [Fact(DisplayName = "ApplicationUser has default empty collections")]
    public void ApplicationUser_WithoutCollections_HasEmptyDefaults()
    {
        var user = new ApplicationUser
        {
            Id = "test-id",
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        Assert.NotNull(user.Roles);
        Assert.Empty(user.Roles);
        Assert.NotNull(user.Claims);
        Assert.Empty(user.Claims);
    }

    [Fact(DisplayName = "IsInRole returns true when user has role")]
    public void IsInRole_WhenUserHasRole_ReturnsTrue()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.IsInRole("Admin");

        Assert.True(result);
    }

    [Fact(DisplayName = "IsInRole returns false when user does not have role")]
    public void IsInRole_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        var user = TestUserFactory.CreateRegularUser();

        var result = user.IsInRole("Admin");

        Assert.False(result);
    }

    [Fact(DisplayName = "IsInRole is case-insensitive")]
    public void IsInRole_WithDifferentCasing_IsCaseInsensitive()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.IsInRole("admin");

        Assert.True(result);
    }

    [Fact(DisplayName = "IsInRole throws when roleName is null")]
    public void IsInRole_WhenRoleNameIsNull_ThrowsArgumentNullException()
    {
        var user = TestUserFactory.CreateRegularUser();

        Assert.Throws<ArgumentNullException>(() => user.IsInRole(null!));
    }

    [Fact(DisplayName = "HasClaim returns true when user has matching claim")]
    public void HasClaim_WhenUserHasMatchingClaim_ReturnsTrue()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.HasClaim("email_verified", "true");

        Assert.True(result);
    }

    [Fact(DisplayName = "HasClaim returns false when user does not have claim")]
    public void HasClaim_WhenUserDoesNotHaveClaim_ReturnsFalse()
    {
        var user = TestUserFactory.CreateRegularUser();

        var result = user.HasClaim("non_existent_claim", "value");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasClaim returns false when claim value does not match")]
    public void HasClaim_WhenClaimValueDoesNotMatch_ReturnsFalse()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.HasClaim("email_verified", "false");

        Assert.False(result);
    }

    [Fact(DisplayName = "HasClaim is case-insensitive for values")]
    public void HasClaim_WithDifferentCasing_IsCaseInsensitive()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.HasClaim("email_verified", "TRUE");

        Assert.True(result);
    }

    [Fact(DisplayName = "HasClaim throws when claimType is null")]
    public void HasClaim_WhenClaimTypeIsNull_ThrowsArgumentNullException()
    {
        var user = TestUserFactory.CreateRegularUser();

        Assert.Throws<ArgumentNullException>(() => user.HasClaim(null!, "value"));
    }

    [Fact(DisplayName = "HasClaim throws when claimValue is null")]
    public void HasClaim_WhenClaimValueIsNull_ThrowsArgumentNullException()
    {
        var user = TestUserFactory.CreateRegularUser();

        Assert.Throws<ArgumentNullException>(() => user.HasClaim("type", null!));
    }

    [Fact(DisplayName = "TryGetClaim returns true and claim value when claim exists")]
    public void TryGetClaim_WhenClaimExists_ReturnsTrueAndValue()
    {
        var user = TestUserFactory.CreateAdminUser();

        var result = user.TryGetClaim("email_verified", out var value);

        Assert.True(result);
        Assert.Equal("true", value);
    }

    [Fact(DisplayName = "TryGetClaim returns false when claim does not exist")]
    public void TryGetClaim_WhenClaimDoesNotExist_ReturnsFalse()
    {
        var user = TestUserFactory.CreateRegularUser();

        var result = user.TryGetClaim("non_existent_claim", out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact(DisplayName = "TryGetClaim throws when claimType is null")]
    public void TryGetClaim_WhenClaimTypeIsNull_ThrowsArgumentNullException()
    {
        var user = TestUserFactory.CreateRegularUser();

        Assert.Throws<ArgumentNullException>(() => user.TryGetClaim(null!, out _));
    }

    [Fact(DisplayName = "ApplicationUser with inactive status is correctly set")]
    public void ApplicationUser_WithInactiveStatus_IsCorrectlySet()
    {
        var user = TestUserFactory.CreateInactiveUser();

        Assert.False(user.IsActive);
    }

    [Fact(DisplayName = "ApplicationUser with active status defaults to true")]
    public void ApplicationUser_DefaultActiveStatus_IsTrue()
    {
        var user = new ApplicationUser
        {
            Id = "test-id",
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        Assert.True(user.IsActive);
    }

    [Fact(DisplayName = "ApplicationUser roles are read-only")]
    public void ApplicationUser_Roles_AreReadOnly()
    {
        var user = TestUserFactory.CreateAdminUser();

        Assert.IsAssignableFrom<IReadOnlyList<string>>(user.Roles);
    }

    [Fact(DisplayName = "ApplicationUser claims are read-only")]
    public void ApplicationUser_Claims_AreReadOnly()
    {
        var user = TestUserFactory.CreateAdminUser();

        Assert.IsAssignableFrom<IReadOnlyDictionary<string, string>>(user.Claims);
    }

    [Fact(DisplayName = "ApplicationUser with optional properties can be null")]
    public void ApplicationUser_WithoutOptionalProperties_HasNullValues()
    {
        var user = new ApplicationUser
        {
            Id = "test-id",
            Email = "test@example.com",
            DisplayName = "Test User"
        };

        Assert.Null(user.GivenName);
        Assert.Null(user.FamilyName);
        Assert.Null(user.LastLoginDate);
    }

    [Fact(DisplayName = "ApplicationUser CreatedDate defaults to current time")]
    public void ApplicationUser_CreatedDate_DefaultsToCurrentTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var user = new ApplicationUser
        {
            Id = "test-id",
            Email = "test@example.com",
            DisplayName = "Test User"
        };
        var afterCreation = DateTime.UtcNow;

        Assert.True(user.CreatedDate >= beforeCreation);
        Assert.True(user.CreatedDate <= afterCreation);
    }
}
