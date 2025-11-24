namespace GhcSamplePs.Core.Exceptions;

/// <summary>
/// Exception thrown when token validation fails.
/// </summary>
public sealed class TokenValidationException : AuthenticationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationException"/> class.
    /// </summary>
    public TokenValidationException()
        : base("Invalid or expired authentication token.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TokenValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenValidationException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TokenValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
