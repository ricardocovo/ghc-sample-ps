namespace GhcSamplePs.Core.Models.Identity;

/// <summary>
/// Represents an authenticated user in the application with identity information extracted from Entra ID claims.
/// This is a domain model that encapsulates user identity without dependencies on external authentication frameworks.
/// </summary>
public sealed class ApplicationUser
{
    /// <summary>
    /// Gets the unique identifier for the user from Entra ID (subject claim).
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the user's email address from the email claim.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets the user's display name from the name claim.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets the user's first name from the given_name claim.
    /// </summary>
    public string? GivenName { get; init; }

    /// <summary>
    /// Gets the user's last name from the family_name claim.
    /// </summary>
    public string? FamilyName { get; init; }

    /// <summary>
    /// Gets the collection of role names assigned to the user.
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = [];

    /// <summary>
    /// Gets the collection of custom claims associated with the user.
    /// Key is the claim type, value is the claim value.
    /// </summary>
    public IReadOnlyDictionary<string, string> Claims { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; init; } = true;

    /// <summary>
    /// Gets the timestamp of the last successful login.
    /// </summary>
    public DateTime? LastLoginDate { get; init; }

    /// <summary>
    /// Gets the timestamp when the account was created.
    /// </summary>
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Determines whether the user has the specified role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    public bool IsInRole(string roleName)
    {
        ArgumentNullException.ThrowIfNull(roleName);
        return Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the user has the specified claim with the given value.
    /// </summary>
    /// <param name="claimType">The type of the claim to check.</param>
    /// <param name="claimValue">The value of the claim to check.</param>
    /// <returns>True if the user has the claim with the specified value; otherwise, false.</returns>
    public bool HasClaim(string claimType, string claimValue)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        ArgumentNullException.ThrowIfNull(claimValue);

        return Claims.TryGetValue(claimType, out var value) &&
               string.Equals(value, claimValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Attempts to get the value of a specific claim.
    /// </summary>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <param name="claimValue">When this method returns, contains the claim value if found; otherwise, null.</param>
    /// <returns>True if the claim exists; otherwise, false.</returns>
    public bool TryGetClaim(string claimType, out string? claimValue)
    {
        ArgumentNullException.ThrowIfNull(claimType);
        return Claims.TryGetValue(claimType, out claimValue);
    }
}
