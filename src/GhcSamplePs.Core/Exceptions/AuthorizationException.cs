namespace GhcSamplePs.Core.Exceptions;

/// <summary>
/// Exception thrown when a user lacks required permissions to access a resource or perform an operation.
/// </summary>
public sealed class AuthorizationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
    /// </summary>
    public AuthorizationException()
        : base("You do not have permission to access this resource.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AuthorizationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public AuthorizationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets or sets the resource that the user attempted to access.
    /// </summary>
    public string? Resource { get; init; }

    /// <summary>
    /// Gets or sets the required permission that the user lacks.
    /// </summary>
    public string? RequiredPermission { get; init; }
}
