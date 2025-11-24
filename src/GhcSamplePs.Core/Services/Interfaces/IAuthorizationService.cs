namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Result of an authorization check indicating whether access is granted and why.
/// </summary>
public sealed class AuthorizationResult
{
    /// <summary>
    /// Gets a value indicating whether authorization succeeded.
    /// </summary>
    public required bool Succeeded { get; init; }

    /// <summary>
    /// Gets the reason why authorization failed, if applicable.
    /// </summary>
    public string? FailureReason { get; init; }

    /// <summary>
    /// Gets the collection of required permissions that were not met.
    /// </summary>
    public IReadOnlyList<string> MissingPermissions { get; init; } = [];

    /// <summary>
    /// Creates a successful authorization result.
    /// </summary>
    /// <returns>An authorization result indicating success.</returns>
    public static AuthorizationResult Success() => new() { Succeeded = true };

    /// <summary>
    /// Creates a failed authorization result with a reason.
    /// </summary>
    /// <param name="reason">The reason why authorization failed.</param>
    /// <param name="missingPermissions">Optional list of missing permissions.</param>
    /// <returns>An authorization result indicating failure.</returns>
    public static AuthorizationResult Failure(string reason, IReadOnlyList<string>? missingPermissions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        return new()
        {
            Succeeded = false,
            FailureReason = reason,
            MissingPermissions = missingPermissions ?? []
        };
    }
}

/// <summary>
/// Service for performing authorization checks and determining user permissions.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Checks whether the current user meets the requirements of the specified policy.
    /// </summary>
    /// <param name="policyName">The name of the authorization policy to evaluate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An authorization result indicating success or failure.</returns>
    Task<AuthorizationResult> AuthorizeAsync(string policyName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the current user is authorized to access the specified resource using the given policy.
    /// </summary>
    /// <param name="resource">The resource to authorize access to.</param>
    /// <param name="policyName">The name of the authorization policy to evaluate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An authorization result indicating success or failure.</returns>
    Task<AuthorizationResult> AuthorizeAsync(object resource, string policyName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the current user can access the specified resource.
    /// </summary>
    /// <param name="resourceId">The identifier of the resource to check access for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user can access the resource; otherwise, false.</returns>
    Task<bool> CanAccessAsync(string resourceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions granted to the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of permission identifiers.</returns>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(CancellationToken cancellationToken = default);
}
