using GhcSamplePs.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GhcSamplePs.Core.Services.Implementations;

/// <summary>
/// Service for performing authorization checks and determining user permissions.
/// </summary>
public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthorizationService> _logger;

    /// <summary>
    /// Well-known policy names used in the application.
    /// </summary>
    public static class Policies
    {
        /// <summary>
        /// Requires the user to be authenticated.
        /// </summary>
        public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";

        /// <summary>
        /// Requires the user to have the Admin role.
        /// </summary>
        public const string RequireAdminRole = "RequireAdminRole";

        /// <summary>
        /// Requires the user to have the User role (or Admin).
        /// </summary>
        public const string RequireUserRole = "RequireUserRole";
    }

    /// <summary>
    /// Well-known role names used in the application.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Administrator role with full access.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Standard user role with basic access.
        /// </summary>
        public const string User = "User";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationService"/> class.
    /// </summary>
    /// <param name="authenticationService">The authentication service for getting user information.</param>
    /// <param name="logger">Logger for authorization events.</param>
    public AuthorizationService(
        IAuthenticationService authenticationService,
        ILogger<AuthorizationService> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<AuthorizationResult> AuthorizeAsync(string policyName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(policyName);

        var user = await _authenticationService.GetCurrentUserAsync(cancellationToken);

        if (user is null)
        {
            _logger.LogDebug("Authorization failed for policy {PolicyName}: No authenticated user", policyName);
            return AuthorizationResult.Failure("User is not authenticated", [policyName]);
        }

        var result = policyName switch
        {
            Policies.RequireAuthenticatedUser => AuthorizationResult.Success(),
            Policies.RequireAdminRole => EvaluateRolePolicy(user, Roles.Admin),
            Policies.RequireUserRole => EvaluateUserOrAdminPolicy(user),
            _ => EvaluateCustomPolicy(user, policyName)
        };

        LogAuthorizationResult(policyName, user.Id, result);
        return result;
    }

    /// <inheritdoc/>
    public async Task<AuthorizationResult> AuthorizeAsync(object resource, string policyName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(policyName);

        // For resource-based authorization, first check the policy
        var policyResult = await AuthorizeAsync(policyName, cancellationToken);
        
        if (!policyResult.Succeeded)
        {
            return policyResult;
        }

        // Additional resource-based checks can be added here based on resource type
        // For now, if policy passes, resource access is granted
        _logger.LogDebug("Resource-based authorization granted for policy {PolicyName}", policyName);
        return AuthorizationResult.Success();
    }

    /// <inheritdoc/>
    public async Task<bool> CanAccessAsync(string resourceId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        var user = await _authenticationService.GetCurrentUserAsync(cancellationToken);
        
        if (user is null)
        {
            _logger.LogDebug("Access denied to resource {ResourceId}: No authenticated user", resourceId);
            return false;
        }

        // Admins can access any resource
        if (user.IsInRole(Roles.Admin))
        {
            _logger.LogDebug("Admin user {UserId} granted access to resource {ResourceId}", user.Id, resourceId);
            return true;
        }

        // Users can only access resources they own (resource ID matches user ID)
        var canAccess = string.Equals(resourceId, user.Id, StringComparison.OrdinalIgnoreCase);
        
        _logger.LogDebug(
            "User {UserId} {AccessResult} access to resource {ResourceId}",
            user.Id,
            canAccess ? "granted" : "denied",
            resourceId);

        return canAccess;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var user = await _authenticationService.GetCurrentUserAsync(cancellationToken);
        
        if (user is null)
        {
            return [];
        }

        var permissions = new List<string>();

        // Add permissions based on roles
        if (user.IsInRole(Roles.Admin))
        {
            permissions.AddRange([
                "read",
                "write",
                "delete",
                "admin.users",
                "admin.settings"
            ]);
        }
        else if (user.IsInRole(Roles.User))
        {
            permissions.AddRange([
                "read",
                "write"
            ]);
        }

        return permissions;
    }

    /// <summary>
    /// Evaluates a role-based policy for a user.
    /// </summary>
    private static AuthorizationResult EvaluateRolePolicy(Models.Identity.ApplicationUser user, string requiredRole)
    {
        if (user.IsInRole(requiredRole))
        {
            return AuthorizationResult.Success();
        }

        return AuthorizationResult.Failure(
            $"User does not have the required role: {requiredRole}",
            [requiredRole]);
    }

    /// <summary>
    /// Evaluates the User or Admin policy - user must have either role.
    /// </summary>
    private static AuthorizationResult EvaluateUserOrAdminPolicy(Models.Identity.ApplicationUser user)
    {
        if (user.IsInRole(Roles.Admin) || user.IsInRole(Roles.User))
        {
            return AuthorizationResult.Success();
        }

        return AuthorizationResult.Failure(
            "User does not have the required role: User or Admin",
            [Roles.User, Roles.Admin]);
    }

    /// <summary>
    /// Evaluates a custom policy that maps to a role requirement.
    /// </summary>
    private static AuthorizationResult EvaluateCustomPolicy(Models.Identity.ApplicationUser user, string policyName)
    {
        // Custom policies can be treated as role checks
        if (user.IsInRole(policyName))
        {
            return AuthorizationResult.Success();
        }

        return AuthorizationResult.Failure(
            $"User does not meet the requirements for policy: {policyName}",
            [policyName]);
    }

    /// <summary>
    /// Logs the result of an authorization check.
    /// </summary>
    private void LogAuthorizationResult(string policyName, string userId, AuthorizationResult result)
    {
        if (result.Succeeded)
        {
            _logger.LogDebug("Authorization succeeded for user {UserId} against policy {PolicyName}", userId, policyName);
        }
        else
        {
            _logger.LogWarning(
                "Authorization failed for user {UserId} against policy {PolicyName}: {Reason}",
                userId,
                policyName,
                result.FailureReason);
        }
    }
}
