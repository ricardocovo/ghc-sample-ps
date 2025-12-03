using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace GhcSamplePs.Web.Services;

/// <summary>
/// Implementation of authentication telemetry service using Application Insights.
/// Tracks authentication, authorization, and Entra ID connectivity events.
/// </summary>
public sealed class AuthenticationTelemetryService : IAuthenticationTelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    private readonly ILogger<AuthenticationTelemetryService> _logger;

    // Custom event names for tracking
    private const string AuthenticationSuccessEvent = "AuthenticationSuccess";
    private const string AuthenticationFailureEvent = "AuthenticationFailure";
    private const string AuthorizationSuccessEvent = "AuthorizationSuccess";
    private const string AuthorizationFailureEvent = "AuthorizationFailure";
    private const string EntraIdConnectivityFailureEvent = "EntraIdConnectivityFailure";
    private const string SignOutEvent = "UserSignOut";

    // Custom metric names
    private const string AuthSuccessMetric = "AuthenticationSuccessCount";
    private const string AuthFailureMetric = "AuthenticationFailureCount";
    private const string AuthorizationFailureMetric = "AuthorizationFailureCount";
    private const string EntraIdConnectivityFailureMetric = "EntraIdConnectivityFailureCount";

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationTelemetryService"/> class.
    /// </summary>
    /// <param name="telemetryClient">The Application Insights telemetry client.</param>
    /// <param name="logger">Logger for authentication telemetry events.</param>
    public AuthenticationTelemetryService(
        TelemetryClient telemetryClient,
        ILogger<AuthenticationTelemetryService> logger)
    {
        _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public void TrackAuthenticationSuccess(string userId, string authenticationMethod)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationMethod);

        var properties = new Dictionary<string, string>
        {
            ["UserId"] = userId,
            ["AuthenticationMethod"] = authenticationMethod,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        _telemetryClient.TrackEvent(AuthenticationSuccessEvent, properties);
        _telemetryClient.GetMetric(AuthSuccessMetric).TrackValue(1);

        _logger.LogInformation(
            "Authentication success for user {UserId} using {AuthenticationMethod}",
            userId,
            authenticationMethod);
    }

    /// <inheritdoc/>
    public void TrackAuthenticationFailure(string reason, string authenticationMethod)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(authenticationMethod);

        var properties = new Dictionary<string, string>
        {
            ["Reason"] = reason,
            ["AuthenticationMethod"] = authenticationMethod,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        _telemetryClient.TrackEvent(AuthenticationFailureEvent, properties);
        _telemetryClient.GetMetric(AuthFailureMetric).TrackValue(1);

        _logger.LogWarning(
            "Authentication failure: {Reason} using {AuthenticationMethod}",
            reason,
            authenticationMethod);
    }

    /// <inheritdoc/>
    public void TrackAuthorizationSuccess(string userId, string resource, string policy)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(policy);

        var properties = new Dictionary<string, string>
        {
            ["UserId"] = userId,
            ["Resource"] = resource,
            ["Policy"] = policy,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        _telemetryClient.TrackEvent(AuthorizationSuccessEvent, properties);

        _logger.LogDebug(
            "Authorization success for user {UserId} on resource {Resource} with policy {Policy}",
            userId,
            resource,
            policy);
    }

    /// <inheritdoc/>
    public void TrackAuthorizationFailure(string userId, string resource, string policy, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        ArgumentException.ThrowIfNullOrWhiteSpace(policy);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        var properties = new Dictionary<string, string>
        {
            ["UserId"] = userId,
            ["Resource"] = resource,
            ["Policy"] = policy,
            ["Reason"] = reason,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        _telemetryClient.TrackEvent(AuthorizationFailureEvent, properties);
        _telemetryClient.GetMetric(AuthorizationFailureMetric).TrackValue(1);

        _logger.LogWarning(
            "Authorization failure for user {UserId} on resource {Resource} with policy {Policy}: {Reason}",
            userId,
            resource,
            policy,
            reason);
    }

    /// <inheritdoc/>
    public void TrackEntraIdConnectivityFailure(string operation, string errorMessage, Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        var properties = new Dictionary<string, string>
        {
            ["Operation"] = operation,
            ["ErrorMessage"] = errorMessage,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        if (exception is not null)
        {
            properties["ExceptionType"] = exception.GetType().Name;
            properties["ExceptionMessage"] = exception.Message;
            
            _telemetryClient.TrackException(exception, properties);
        }

        _telemetryClient.TrackEvent(EntraIdConnectivityFailureEvent, properties);
        _telemetryClient.GetMetric(EntraIdConnectivityFailureMetric).TrackValue(1);

        _logger.LogError(
            exception,
            "Entra ID connectivity failure during {Operation}: {ErrorMessage}",
            operation,
            errorMessage);
    }

    /// <inheritdoc/>
    public void TrackSignOut(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var properties = new Dictionary<string, string>
        {
            ["UserId"] = userId,
            ["EventTime"] = DateTime.UtcNow.ToString("O")
        };

        _telemetryClient.TrackEvent(SignOutEvent, properties);

        _logger.LogInformation("User {UserId} signed out", userId);
    }
}
