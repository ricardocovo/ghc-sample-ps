using GhcSamplePs.Core.Models.Identity;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Service for retrieving and validating authenticated user information.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Retrieves the current authenticated user's information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current authenticated user, or null if no user is authenticated.</returns>
    Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all claims for the current authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of claim types and their values.</returns>
    Task<IReadOnlyDictionary<string, string>> GetUserClaimsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles assigned to the current authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of role names assigned to the user.</returns>
    Task<IReadOnlyList<string>> GetUserRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the current user is in the specified role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user is in the role; otherwise, false.</returns>
    Task<bool> IsInRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the current user has a specific claim with the given value.
    /// </summary>
    /// <param name="claimType">The type of the claim to check.</param>
    /// <param name="claimValue">The value of the claim to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the claim with the specified value; otherwise, false.</returns>
    Task<bool> HasClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default);
}
