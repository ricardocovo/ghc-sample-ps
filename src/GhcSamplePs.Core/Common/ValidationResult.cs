namespace GhcSamplePs.Core.Common;

/// <summary>
/// Represents the result of a validation operation, containing validation status and field-specific errors.
/// </summary>
/// <example>
/// <code>
/// // Create a successful validation result
/// var successResult = ValidationResult.Valid();
/// 
/// // Create a failed validation result with errors
/// var errors = new Dictionary&lt;string, string[]&gt;
/// {
///     { "Name", new[] { "Name is required", "Name cannot exceed 200 characters" } },
///     { "Email", new[] { "Invalid email format" } }
/// };
/// var failedResult = ValidationResult.Invalid(errors);
/// 
/// // Check validation status
/// if (!failedResult.IsValid)
/// {
///     foreach (var (field, messages) in failedResult.Errors)
///     {
///         Console.WriteLine($"{field}: {string.Join(", ", messages)}");
///     }
/// }
/// </code>
/// </example>
public sealed class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation passed.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the dictionary of field-specific validation errors.
    /// Keys represent field names, and values are arrays of error messages for each field.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyDictionary<string, string[]>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Creates a successful validation result with no errors.
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> indicating successful validation.</returns>
    /// <example>
    /// <code>
    /// var result = ValidationResult.Valid();
    /// Console.WriteLine(result.IsValid); // True
    /// Console.WriteLine(result.Errors.Count); // 0
    /// </code>
    /// </example>
    public static ValidationResult Valid() => new(true);

    /// <summary>
    /// Creates a failed validation result with the specified field errors.
    /// </summary>
    /// <param name="errors">A dictionary of field names to their error messages.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating failed validation with field-specific errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is null.</exception>
    /// <example>
    /// <code>
    /// var errors = new Dictionary&lt;string, string[]&gt;
    /// {
    ///     { "Name", new[] { "Name is required" } }
    /// };
    /// var result = ValidationResult.Invalid(errors);
    /// Console.WriteLine(result.IsValid); // False
    /// </code>
    /// </example>
    public static ValidationResult Invalid(IDictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new ValidationResult(false, new Dictionary<string, string[]>(errors));
    }

    /// <summary>
    /// Creates a failed validation result with a single field error.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="errorMessage">The error message for the field.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating failed validation with the specified error.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <example>
    /// <code>
    /// var result = ValidationResult.Invalid("Email", "Invalid email format");
    /// Console.WriteLine(result.Errors["Email"][0]); // "Invalid email format"
    /// </code>
    /// </example>
    public static ValidationResult Invalid(string fieldName, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Field name cannot be null or whitespace.", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));
        }

        var errors = new Dictionary<string, string[]>
        {
            { fieldName, [errorMessage] }
        };

        return new ValidationResult(false, errors);
    }

    /// <summary>
    /// Creates a failed validation result with multiple error messages for a single field.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="errorMessages">The error messages for the field.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating failed validation with the specified errors.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessages"/> is empty.</exception>
    /// <example>
    /// <code>
    /// var result = ValidationResult.Invalid("Name", new[] { "Name is required", "Name cannot exceed 200 characters" });
    /// Console.WriteLine(result.Errors["Name"].Length); // 2
    /// </code>
    /// </example>
    public static ValidationResult Invalid(string fieldName, string[] errorMessages)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Field name cannot be null or whitespace.", nameof(fieldName));
        }

        ArgumentNullException.ThrowIfNull(errorMessages);

        if (errorMessages.Length == 0)
        {
            throw new ArgumentException("Error messages array cannot be empty.", nameof(errorMessages));
        }

        var errors = new Dictionary<string, string[]>
        {
            { fieldName, errorMessages }
        };

        return new ValidationResult(false, errors);
    }
}
