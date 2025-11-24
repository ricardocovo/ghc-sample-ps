using GhcSamplePs.Core.Models.Identity;

namespace GhcSamplePs.Core.Tests.TestHelpers;

/// <summary>
/// Test helper class for creating UserClaim instances for testing.
/// </summary>
public static class TestClaimFactory
{
    /// <summary>
    /// Creates a test claim for email verification.
    /// </summary>
    /// <param name="isVerified">Whether the email is verified.</param>
    /// <returns>A UserClaim for email verification.</returns>
    public static UserClaim CreateEmailVerifiedClaim(bool isVerified = true)
    {
        return new UserClaim
        {
            Type = "email_verified",
            Value = isVerified.ToString().ToLowerInvariant(),
            Issuer = "test-issuer",
            IssuedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test claim for profile completion status.
    /// </summary>
    /// <param name="isComplete">Whether the profile is complete.</param>
    /// <returns>A UserClaim for profile completion.</returns>
    public static UserClaim CreateProfileCompleteClaim(bool isComplete = true)
    {
        return new UserClaim
        {
            Type = "profile_complete",
            Value = isComplete.ToString().ToLowerInvariant(),
            Issuer = "test-issuer",
            IssuedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a test claim with expiration.
    /// </summary>
    /// <param name="type">Claim type.</param>
    /// <param name="value">Claim value.</param>
    /// <param name="expiresInMinutes">Minutes until expiration.</param>
    /// <returns>A UserClaim with expiration.</returns>
    public static UserClaim CreateExpiringClaim(string type, string value, int expiresInMinutes)
    {
        return new UserClaim
        {
            Type = type,
            Value = value,
            Issuer = "test-issuer",
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes)
        };
    }

    /// <summary>
    /// Creates an expired test claim.
    /// </summary>
    /// <param name="type">Claim type.</param>
    /// <param name="value">Claim value.</param>
    /// <returns>An expired UserClaim.</returns>
    public static UserClaim CreateExpiredClaim(string type, string value)
    {
        return new UserClaim
        {
            Type = type,
            Value = value,
            Issuer = "test-issuer",
            IssuedAt = DateTime.UtcNow.AddHours(-2),
            ExpiresAt = DateTime.UtcNow.AddHours(-1)
        };
    }

    /// <summary>
    /// Creates a custom test claim with all properties specified.
    /// </summary>
    /// <param name="type">Claim type.</param>
    /// <param name="value">Claim value.</param>
    /// <param name="issuer">Claim issuer.</param>
    /// <param name="issuedAt">Issued timestamp.</param>
    /// <param name="expiresAt">Expiration timestamp.</param>
    /// <returns>A custom UserClaim.</returns>
    public static UserClaim CreateCustomClaim(
        string type,
        string value,
        string? issuer = null,
        DateTime? issuedAt = null,
        DateTime? expiresAt = null)
    {
        return new UserClaim
        {
            Type = type,
            Value = value,
            Issuer = issuer ?? "test-issuer",
            IssuedAt = issuedAt ?? DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Creates a collection of standard test claims.
    /// </summary>
    /// <returns>A list of standard test claims.</returns>
    public static IReadOnlyList<UserClaim> CreateStandardClaims()
    {
        return
        [
            CreateEmailVerifiedClaim(true),
            CreateProfileCompleteClaim(true),
            CreateCustomClaim("preferred_username", "testuser"),
            CreateCustomClaim("tenant_id", "test-tenant-id")
        ];
    }
}
