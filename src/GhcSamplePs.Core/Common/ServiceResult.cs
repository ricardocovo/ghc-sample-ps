namespace GhcSamplePs.Core.Common;

/// <summary>
/// Represents the result of a service operation, containing the operation outcome,
/// optional data, error messages, and validation errors.
/// </summary>
/// <typeparam name="T">The type of data returned by the operation.</typeparam>
/// <example>
/// <code>
/// // Success with data
/// var player = new Player { Id = 1, Name = "John" };
/// var successResult = ServiceResult&lt;Player&gt;.Ok(player);
/// 
/// // Failure with error message
/// var failResult = ServiceResult&lt;Player&gt;.Fail("Player not found");
/// 
/// // Failure with validation errors
/// var validationErrors = new Dictionary&lt;string, string[]&gt;
/// {
///     { "Name", new[] { "Name is required" } }
/// };
/// var validationResult = ServiceResult&lt;Player&gt;.ValidationFailed(validationErrors);
/// 
/// // Using the result
/// if (result.Success)
/// {
///     Console.WriteLine($"Player: {result.Data!.Name}");
/// }
/// else if (result.ValidationErrors.Count > 0)
/// {
///     foreach (var (field, errors) in result.ValidationErrors)
///     {
///         Console.WriteLine($"{field}: {string.Join(", ", errors)}");
///     }
/// }
/// else
/// {
///     Console.WriteLine($"Error: {string.Join(", ", result.ErrorMessages)}");
/// }
/// </code>
/// </example>
public sealed class ServiceResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the data returned by the operation. 
    /// Will be the default value of <typeparamref name="T"/> when <see cref="Success"/> is false.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets the list of general error messages from the operation.
    /// </summary>
    public IReadOnlyList<string> ErrorMessages { get; }

    /// <summary>
    /// Gets the dictionary of field-specific validation errors.
    /// Keys represent field names, and values are arrays of error messages for each field.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; }

    private ServiceResult(
        bool success,
        T? data = default,
        IReadOnlyList<string>? errorMessages = null,
        IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        Success = success;
        Data = data;
        ErrorMessages = errorMessages ?? [];
        ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Creates a successful result with the specified data.
    /// </summary>
    /// <param name="data">The data to return in the result.</param>
    /// <returns>A successful <see cref="ServiceResult{T}"/> containing the data.</returns>
    /// <example>
    /// <code>
    /// var player = new Player { Id = 1, Name = "John" };
    /// var result = ServiceResult&lt;Player&gt;.Ok(player);
    /// Console.WriteLine(result.Success); // True
    /// Console.WriteLine(result.Data!.Name); // "John"
    /// </code>
    /// </example>
    public static ServiceResult<T> Ok(T data) => new(true, data);

    /// <summary>
    /// Creates a successful result with no data.
    /// </summary>
    /// <returns>A successful <see cref="ServiceResult{T}"/> with default data.</returns>
    /// <example>
    /// <code>
    /// var result = ServiceResult&lt;bool&gt;.Ok();
    /// Console.WriteLine(result.Success); // True
    /// </code>
    /// </example>
    public static ServiceResult<T> Ok() => new(true);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with the specified error message.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <example>
    /// <code>
    /// var result = ServiceResult&lt;Player&gt;.Fail("Player not found");
    /// Console.WriteLine(result.Success); // False
    /// Console.WriteLine(result.ErrorMessages[0]); // "Player not found"
    /// </code>
    /// </example>
    public static ServiceResult<T> Fail(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be null or whitespace.", nameof(errorMessage));
        }

        return new ServiceResult<T>(false, errorMessages: [errorMessage]);
    }

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    /// <param name="errorMessages">The collection of error messages describing the failures.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with the specified error messages.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessages"/> is empty.</exception>
    /// <example>
    /// <code>
    /// var result = ServiceResult&lt;Player&gt;.Fail(new[] { "Error 1", "Error 2" });
    /// Console.WriteLine(result.ErrorMessages.Count); // 2
    /// </code>
    /// </example>
    public static ServiceResult<T> Fail(IEnumerable<string> errorMessages)
    {
        ArgumentNullException.ThrowIfNull(errorMessages);

        var messagesList = errorMessages.ToList();
        if (messagesList.Count == 0)
        {
            throw new ArgumentException("Error messages collection cannot be empty.", nameof(errorMessages));
        }

        return new ServiceResult<T>(false, errorMessages: messagesList);
    }

    /// <summary>
    /// Creates a failed result with validation errors.
    /// </summary>
    /// <param name="validationErrors">A dictionary of field names to their error messages.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with the specified validation errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="validationErrors"/> is null.</exception>
    /// <example>
    /// <code>
    /// var errors = new Dictionary&lt;string, string[]&gt;
    /// {
    ///     { "Name", new[] { "Name is required" } }
    /// };
    /// var result = ServiceResult&lt;Player&gt;.ValidationFailed(errors);
    /// Console.WriteLine(result.Success); // False
    /// Console.WriteLine(result.ValidationErrors["Name"][0]); // "Name is required"
    /// </code>
    /// </example>
    public static ServiceResult<T> ValidationFailed(IDictionary<string, string[]> validationErrors)
    {
        ArgumentNullException.ThrowIfNull(validationErrors);
        return new ServiceResult<T>(false, validationErrors: new Dictionary<string, string[]>(validationErrors));
    }

    /// <summary>
    /// Creates a failed result with validation errors from a <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">The validation result containing the errors.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with the validation errors from the <paramref name="validationResult"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="validationResult"/> is null.</exception>
    /// <example>
    /// <code>
    /// var validationResult = ValidationResult.Invalid("Name", "Name is required");
    /// var result = ServiceResult&lt;Player&gt;.ValidationFailed(validationResult);
    /// Console.WriteLine(result.ValidationErrors["Name"][0]); // "Name is required"
    /// </code>
    /// </example>
    public static ServiceResult<T> ValidationFailed(ValidationResult validationResult)
    {
        ArgumentNullException.ThrowIfNull(validationResult);
        return new ServiceResult<T>(false, validationErrors: validationResult.Errors);
    }

    /// <summary>
    /// Creates a failed result with a single validation error.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="errorMessage">The error message for the field.</param>
    /// <returns>A failed <see cref="ServiceResult{T}"/> with the specified validation error.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <example>
    /// <code>
    /// var result = ServiceResult&lt;Player&gt;.ValidationFailed("Email", "Invalid email format");
    /// Console.WriteLine(result.ValidationErrors["Email"][0]); // "Invalid email format"
    /// </code>
    /// </example>
    public static ServiceResult<T> ValidationFailed(string fieldName, string errorMessage)
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

        return new ServiceResult<T>(false, validationErrors: errors);
    }
}
