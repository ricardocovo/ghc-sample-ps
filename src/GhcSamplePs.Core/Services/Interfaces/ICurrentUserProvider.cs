using System.Security.Claims;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Provides access to the current user's claims principal.
/// This abstraction allows the Core project to remain UI-agnostic while
/// enabling authentication services to access user claims.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Gets the current user's claims principal.
    /// </summary>
    /// <returns>The current user's ClaimsPrincipal, or null if no user is authenticated.</returns>
    ClaimsPrincipal? GetCurrentUser();

    /// <summary>
    /// Gets a value indicating whether there is an authenticated user.
    /// </summary>
    bool IsAuthenticated { get; }
}
