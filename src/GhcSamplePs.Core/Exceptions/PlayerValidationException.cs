namespace GhcSamplePs.Core.Exceptions;

/// <summary>
/// Exception thrown when player data fails validation.
/// </summary>
public class PlayerValidationException : Exception
{
    /// <summary>
    /// Gets the dictionary of field-specific validation errors.
    /// Keys represent field names, and values are arrays of error messages for each field.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerValidationException"/> class with a default message.
    /// </summary>
    public PlayerValidationException()
        : base("Player validation failed.")
    {
        ValidationErrors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerValidationException"/> class with validation errors.
    /// </summary>
    /// <param name="validationErrors">The dictionary of field-specific validation errors.</param>
    public PlayerValidationException(IDictionary<string, string[]> validationErrors)
        : base("Player validation failed. See ValidationErrors for details.")
    {
        ArgumentNullException.ThrowIfNull(validationErrors);
        ValidationErrors = new Dictionary<string, string[]>(validationErrors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PlayerValidationException(string message)
        : base(message)
    {
        ValidationErrors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerValidationException"/> class with a specified error message
    /// and validation errors.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="validationErrors">The dictionary of field-specific validation errors.</param>
    public PlayerValidationException(string message, IDictionary<string, string[]> validationErrors)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(validationErrors);
        ValidationErrors = new Dictionary<string, string[]>(validationErrors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerValidationException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PlayerValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        ValidationErrors = new Dictionary<string, string[]>();
    }
}
