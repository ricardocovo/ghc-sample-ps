using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.Identity;

public class UserClaimTests
{
    [Fact(DisplayName = "UserClaim can be created with required properties")]
    public void UserClaim_WithRequiredProperties_CreatesSuccessfully()
    {
        var claim = new UserClaim
        {
            Type = "test_claim",
            Value = "test_value"
        };

        Assert.NotNull(claim);
        Assert.Equal("test_claim", claim.Type);
        Assert.Equal("test_value", claim.Value);
    }

    [Fact(DisplayName = "UserClaim IssuedAt defaults to current time")]
    public void UserClaim_IssuedAt_DefaultsToCurrentTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var claim = new UserClaim
        {
            Type = "test_claim",
            Value = "test_value"
        };
        var afterCreation = DateTime.UtcNow;

        Assert.True(claim.IssuedAt >= beforeCreation);
        Assert.True(claim.IssuedAt <= afterCreation);
    }

    [Fact(DisplayName = "IsValid returns true when claim has no expiration")]
    public void IsValid_WhenClaimHasNoExpiration_ReturnsTrue()
    {
        var claim = TestClaimFactory.CreateEmailVerifiedClaim();

        var result = claim.IsValid();

        Assert.True(result);
    }

    [Fact(DisplayName = "IsValid returns true when claim has not expired")]
    public void IsValid_WhenClaimHasNotExpired_ReturnsTrue()
    {
        var claim = TestClaimFactory.CreateExpiringClaim("test_claim", "test_value", 60);

        var result = claim.IsValid();

        Assert.True(result);
    }

    [Fact(DisplayName = "IsValid returns false when claim has expired")]
    public void IsValid_WhenClaimHasExpired_ReturnsFalse()
    {
        var claim = TestClaimFactory.CreateExpiredClaim("test_claim", "test_value");

        var result = claim.IsValid();

        Assert.False(result);
    }

    [Fact(DisplayName = "Equals returns true for claims with same type and value")]
    public void Equals_WhenClaimsHaveSameTypeAndValue_ReturnsTrue()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };

        var result = claim1.Equals(claim2);

        Assert.True(result);
    }

    [Fact(DisplayName = "Equals returns false for claims with different type")]
    public void Equals_WhenClaimsHaveDifferentType_ReturnsFalse()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "profile_complete",
            Value = "true"
        };

        var result = claim1.Equals(claim2);

        Assert.False(result);
    }

    [Fact(DisplayName = "Equals returns false for claims with different value")]
    public void Equals_WhenClaimsHaveDifferentValue_ReturnsFalse()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "email_verified",
            Value = "false"
        };

        var result = claim1.Equals(claim2);

        Assert.False(result);
    }

    [Fact(DisplayName = "Equals is case-insensitive for type")]
    public void Equals_WithDifferentTypeCasing_IsCaseInsensitive()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "EMAIL_VERIFIED",
            Value = "true"
        };

        var result = claim1.Equals(claim2);

        Assert.True(result);
    }

    [Fact(DisplayName = "Equals is case-sensitive for value")]
    public void Equals_WithDifferentValueCasing_IsCaseSensitive()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "email_verified",
            Value = "TRUE"
        };

        var result = claim1.Equals(claim2);

        Assert.False(result);
    }

    [Fact(DisplayName = "Equals returns false when comparing with null")]
    public void Equals_WhenComparingWithNull_ReturnsFalse()
    {
        var claim = TestClaimFactory.CreateEmailVerifiedClaim();

        var result = claim.Equals(null);

        Assert.False(result);
    }

    [Fact(DisplayName = "Equals returns false when comparing with non-UserClaim object")]
    public void Equals_WhenComparingWithDifferentType_ReturnsFalse()
    {
        var claim = TestClaimFactory.CreateEmailVerifiedClaim();

        var result = claim.Equals("not a claim");

        Assert.False(result);
    }

    [Fact(DisplayName = "GetHashCode returns same value for equal claims")]
    public void GetHashCode_WhenClaimsAreEqual_ReturnsSameValue()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };

        var hash1 = claim1.GetHashCode();
        var hash2 = claim2.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact(DisplayName = "GetHashCode returns different value for different claims")]
    public void GetHashCode_WhenClaimsAreDifferent_ReturnsDifferentValue()
    {
        var claim1 = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };
        var claim2 = new UserClaim
        {
            Type = "profile_complete",
            Value = "false"
        };

        var hash1 = claim1.GetHashCode();
        var hash2 = claim2.GetHashCode();

        Assert.NotEqual(hash1, hash2);
    }

    [Fact(DisplayName = "ToString returns formatted claim string")]
    public void ToString_ReturnsFormattedString()
    {
        var claim = new UserClaim
        {
            Type = "email_verified",
            Value = "true"
        };

        var result = claim.ToString();

        Assert.Equal("email_verified: true", result);
    }

    [Fact(DisplayName = "UserClaim with issuer is correctly set")]
    public void UserClaim_WithIssuer_IsCorrectlySet()
    {
        var claim = new UserClaim
        {
            Type = "test_claim",
            Value = "test_value",
            Issuer = "test-issuer"
        };

        Assert.Equal("test-issuer", claim.Issuer);
    }

    [Fact(DisplayName = "UserClaim with expiration is correctly set")]
    public void UserClaim_WithExpiration_IsCorrectlySet()
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var claim = new UserClaim
        {
            Type = "test_claim",
            Value = "test_value",
            ExpiresAt = expiresAt
        };

        Assert.Equal(expiresAt, claim.ExpiresAt);
    }
}
