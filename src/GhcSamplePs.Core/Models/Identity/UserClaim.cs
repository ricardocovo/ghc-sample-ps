namespace GhcSamplePs.Core.Models.Identity;

/// <summary>
/// Represents a custom claim associated with a user.
/// Claims are key-value pairs that represent information about the user.
/// </summary>
public sealed class UserClaim
{
    /// <summary>
    /// Gets the claim type (e.g., "email_verified", "profile_complete").
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets the claim value.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Gets the optional issuer of the claim.
    /// </summary>
    public string? Issuer { get; init; }

    /// <summary>
    /// Gets the timestamp when the claim was issued.
    /// </summary>
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the optional expiration timestamp for the claim.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// Determines whether the claim is currently valid (not expired).
    /// </summary>
    /// <returns>True if the claim is valid; otherwise, false.</returns>
    public bool IsValid()
    {
        return ExpiresAt is null || ExpiresAt.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Determines whether two claims are equal based on type and value.
    /// </summary>
    /// <param name="obj">The object to compare with the current claim.</param>
    /// <returns>True if the specified object is equal to the current claim; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is UserClaim other &&
               string.Equals(Type, other.Type, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a hash code for the claim based on type and value.
    /// </summary>
    /// <returns>A hash code for the current claim.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Type.ToLowerInvariant(),
            Value);
    }

    /// <summary>
    /// Returns a string representation of the claim.
    /// </summary>
    /// <returns>A string that represents the current claim.</returns>
    public override string ToString()
    {
        return $"{Type}: {Value}";
    }
}
