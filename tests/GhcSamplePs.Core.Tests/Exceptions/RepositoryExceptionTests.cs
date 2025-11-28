using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class RepositoryExceptionTests
{
    [Fact(DisplayName = "RepositoryException can be created with default constructor")]
    public void Constructor_WithDefaultConstructor_CreatesExceptionWithDefaultMessage()
    {
        var exception = new RepositoryException();

        Assert.Equal("A repository operation failed.", exception.Message);
        Assert.Null(exception.Operation);
        Assert.Null(exception.EntityType);
        Assert.Null(exception.EntityId);
    }

    [Fact(DisplayName = "RepositoryException can be created with custom message")]
    public void Constructor_WithMessage_CreatesExceptionWithMessage()
    {
        var exception = new RepositoryException("Custom error message");

        Assert.Equal("Custom error message", exception.Message);
    }

    [Fact(DisplayName = "RepositoryException can be created with message and inner exception")]
    public void Constructor_WithMessageAndInnerException_CreatesExceptionWithInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var exception = new RepositoryException("Custom error message", innerException);

        Assert.Equal("Custom error message", exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "RepositoryException can be created with operation context")]
    public void Constructor_WithOperationContext_SetsAllProperties()
    {
        var innerException = new InvalidOperationException("Database error");
        var exception = new RepositoryException(
            "Failed to retrieve player",
            "GetByIdAsync",
            "Player",
            42,
            innerException);

        Assert.Equal("Failed to retrieve player", exception.Message);
        Assert.Equal("GetByIdAsync", exception.Operation);
        Assert.Equal("Player", exception.EntityType);
        Assert.Equal(42, exception.EntityId);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "RepositoryException can be created with partial context")]
    public void Constructor_WithPartialContext_SetsAvailableProperties()
    {
        var exception = new RepositoryException(
            "Operation failed",
            "UpdateAsync",
            "Player");

        Assert.Equal("Operation failed", exception.Message);
        Assert.Equal("UpdateAsync", exception.Operation);
        Assert.Equal("Player", exception.EntityType);
        Assert.Null(exception.EntityId);
        Assert.Null(exception.InnerException);
    }

    [Fact(DisplayName = "RepositoryException is derived from Exception")]
    public void RepositoryException_IsDerivedFromException()
    {
        var exception = new RepositoryException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "RepositoryException can be thrown and caught")]
    public void RepositoryException_CanBeThrownAndCaught()
    {
        static void ThrowException()
        {
            throw new RepositoryException("Test exception");
        }

        var exception = Assert.Throws<RepositoryException>(ThrowException);

        Assert.Equal("Test exception", exception.Message);
    }

    [Fact(DisplayName = "RepositoryException EntityId can be any object type")]
    public void EntityId_CanBeAnyObjectType()
    {
        var guidId = Guid.NewGuid();
        var exception = new RepositoryException(
            "Failed operation",
            "GetByIdAsync",
            "Entity",
            guidId);

        Assert.Equal(guidId, exception.EntityId);
    }
}
