namespace GhcSamplePs.Core.Exceptions;

/// <summary>
/// Exception thrown when a repository operation fails due to database-related issues.
/// </summary>
public class RepositoryException : Exception
{
    /// <summary>
    /// Gets the name of the operation that failed.
    /// </summary>
    public string? Operation { get; }

    /// <summary>
    /// Gets the entity type involved in the failed operation.
    /// </summary>
    public string? EntityType { get; }

    /// <summary>
    /// Gets the identifier of the entity involved in the failed operation, if applicable.
    /// </summary>
    public object? EntityId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with a default message.
    /// </summary>
    public RepositoryException()
        : base("A repository operation failed.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RepositoryException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryException"/> class with operation context.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="operation">The name of the operation that failed.</param>
    /// <param name="entityType">The entity type involved in the failed operation.</param>
    /// <param name="entityId">The identifier of the entity involved, if applicable.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RepositoryException(
        string message,
        string? operation,
        string? entityType,
        object? entityId = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Operation = operation;
        EntityType = entityType;
        EntityId = entityId;
    }
}
