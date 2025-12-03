namespace GhcSamplePs.Web.Services;

/// <summary>
/// Service for tracking authentication and authorization events to Application Insights.
/// </summary>
public interface IAuthenticationTelemetryService
{
    /// <summary>
    /// Tracks a successful authentication event.
    /// </summary>
    /// <param name="userId">The user ID that authenticated.</param>
    /// <param name="authenticationMethod">The authentication method used (e.g., OpenIdConnect).</param>
    void TrackAuthenticationSuccess(string userId, string authenticationMethod);

    /// <summary>
    /// Tracks a failed authentication event.
    /// </summary>
    /// <param name="reason">The reason for the authentication failure.</param>
    /// <param name="authenticationMethod">The authentication method used (e.g., OpenIdConnect).</param>
    void TrackAuthenticationFailure(string reason, string authenticationMethod);

    /// <summary>
    /// Tracks a successful authorization event.
    /// </summary>
    /// <param name="userId">The user ID that was authorized.</param>
    /// <param name="resource">The resource being accessed.</param>
    /// <param name="policy">The authorization policy applied.</param>
    void TrackAuthorizationSuccess(string userId, string resource, string policy);

    /// <summary>
    /// Tracks a failed authorization event.
    /// </summary>
    /// <param name="userId">The user ID that was denied access.</param>
    /// <param name="resource">The resource that was attempted to access.</param>
    /// <param name="policy">The authorization policy that failed.</param>
    /// <param name="reason">The reason for the authorization failure.</param>
    void TrackAuthorizationFailure(string userId, string resource, string policy, string reason);

    /// <summary>
    /// Tracks an Entra ID connectivity failure.
    /// </summary>
    /// <param name="operation">The operation that failed.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">Optional exception details.</param>
    void TrackEntraIdConnectivityFailure(string operation, string errorMessage, Exception? exception = null);

    /// <summary>
    /// Tracks a sign-out event.
    /// </summary>
    /// <param name="userId">The user ID that signed out.</param>
    void TrackSignOut(string userId);
}
