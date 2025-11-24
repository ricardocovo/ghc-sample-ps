using System.Security.Claims;
using GhcSamplePs.Core.Models.Identity;
using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service for retrieving and validating authenticated user information.
/// Extracts user data from claims provided by the authentication system.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<AuthenticationService> _logger;

    /// <summary>
    /// Standard claim types used for extracting user information.
    /// </summary>
    private static class ClaimTypes
    {
        public const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public const string PreferredUsername = "preferred_username";
        public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string DisplayName = "name";
        public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public const string FamilyName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="currentUserProvider">Provider for the current user's claims.</param>
    /// <param name="logger">Logger for authentication events.</param>
    public AuthenticationService(
        ICurrentUserProvider currentUserProvider,
        ILogger<AuthenticationService> logger)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var principal = _currentUserProvider.GetCurrentUser();
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("No authenticated user found");
            return Task.FromResult<ApplicationUser?>(null);
        }

        var user = MapClaimsToApplicationUser(principal);
        _logger.LogDebug("Retrieved authenticated user: {UserId}", user.Id);
        
        return Task.FromResult<ApplicationUser?>(user);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<string, string>> GetUserClaimsAsync(CancellationToken cancellationToken = default)
    {
        var principal = _currentUserProvider.GetCurrentUser();
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<IReadOnlyDictionary<string, string>>(new Dictionary<string, string>());
        }

        var claims = principal.Claims
            .GroupBy(c => c.Type)
            .ToDictionary(
                g => g.Key,
                g => g.First().Value);

        return Task.FromResult<IReadOnlyDictionary<string, string>>(claims);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<string>> GetUserRolesAsync(CancellationToken cancellationToken = default)
    {
        var principal = _currentUserProvider.GetCurrentUser();
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        }

        var roles = principal.Claims
            .Where(c => c.Type is ClaimTypes.Role or System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Task.FromResult<IReadOnlyList<string>>(roles);
    }

    /// <inheritdoc/>
    public Task<bool> IsInRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roleName);

        var principal = _currentUserProvider.GetCurrentUser();
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(false);
        }

        var isInRole = principal.IsInRole(roleName);
        return Task.FromResult(isInRole);
    }

    /// <inheritdoc/>
    public Task<bool> HasClaimAsync(string claimType, string claimValue, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        ArgumentException.ThrowIfNullOrWhiteSpace(claimValue);

        var principal = _currentUserProvider.GetCurrentUser();
        
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(false);
        }

        var hasClaim = principal.HasClaim(claimType, claimValue);
        return Task.FromResult(hasClaim);
    }

    /// <summary>
    /// Maps claims from a ClaimsPrincipal to an ApplicationUser domain model.
    /// </summary>
    /// <param name="principal">The claims principal to map.</param>
    /// <returns>An ApplicationUser populated from the claims.</returns>
    private static ApplicationUser MapClaimsToApplicationUser(ClaimsPrincipal principal)
    {
        var claims = principal.Claims.ToList();
        
        var id = GetClaimValue(claims, ClaimTypes.ObjectIdentifier)
              ?? GetClaimValue(claims, ClaimTypes.NameIdentifier)
              ?? string.Empty;

        var email = GetClaimValue(claims, ClaimTypes.Email)
                 ?? GetClaimValue(claims, ClaimTypes.PreferredUsername)
                 ?? string.Empty;

        var displayName = GetClaimValue(claims, ClaimTypes.DisplayName)
                       ?? GetClaimValue(claims, ClaimTypes.Name)
                       ?? email;

        var givenName = GetClaimValue(claims, ClaimTypes.GivenName);
        var familyName = GetClaimValue(claims, ClaimTypes.FamilyName);

        var roles = claims
            .Where(c => c.Type is ClaimTypes.Role or System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var customClaims = claims
            .Where(c => !IsStandardClaimType(c.Type))
            .GroupBy(c => c.Type)
            .ToDictionary(
                g => g.Key,
                g => g.First().Value);

        return new ApplicationUser
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            GivenName = givenName,
            FamilyName = familyName,
            Roles = roles,
            Claims = customClaims,
            IsActive = true,
            LastLoginDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Gets the value of a claim by type.
    /// </summary>
    /// <param name="claims">The collection of claims to search.</param>
    /// <param name="claimType">The type of claim to find.</param>
    /// <returns>The claim value, or null if not found.</returns>
    private static string? GetClaimValue(IEnumerable<Claim> claims, string claimType)
    {
        return claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }

    /// <summary>
    /// Determines whether a claim type is a standard claim type (not custom).
    /// </summary>
    /// <param name="claimType">The claim type to check.</param>
    /// <returns>True if the claim type is standard; otherwise, false.</returns>
    private static bool IsStandardClaimType(string claimType)
    {
        return claimType is ClaimTypes.NameIdentifier
            or ClaimTypes.ObjectIdentifier
            or ClaimTypes.Email
            or ClaimTypes.PreferredUsername
            or ClaimTypes.Name
            or ClaimTypes.DisplayName
            or ClaimTypes.GivenName
            or ClaimTypes.FamilyName
            or ClaimTypes.Role
            or System.Security.Claims.ClaimTypes.Role
            or System.Security.Claims.ClaimTypes.NameIdentifier
            or System.Security.Claims.ClaimTypes.Email
            or System.Security.Claims.ClaimTypes.Name
            or System.Security.Claims.ClaimTypes.GivenName
            or System.Security.Claims.ClaimTypes.Surname;
    }
}
