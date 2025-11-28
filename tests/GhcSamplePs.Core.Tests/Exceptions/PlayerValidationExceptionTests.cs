using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class PlayerValidationExceptionTests
{
    [Fact(DisplayName = "PlayerValidationException can be created with default constructor")]
    public void PlayerValidationException_WithDefaultConstructor_HasDefaultMessage()
    {
        var exception = new PlayerValidationException();

        Assert.NotNull(exception);
        Assert.Equal("Player validation failed.", exception.Message);
        Assert.NotNull(exception.ValidationErrors);
        Assert.Empty(exception.ValidationErrors);
    }

    [Fact(DisplayName = "PlayerValidationException can be created with validation errors")]
    public void PlayerValidationException_WithValidationErrors_ContainsErrors()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "DateOfBirth", new[] { "Date of birth must be in the past" } }
        };

        var exception = new PlayerValidationException(errors);

        Assert.NotNull(exception);
        Assert.Equal(2, exception.ValidationErrors.Count);
        Assert.Contains("Name", exception.ValidationErrors.Keys);
        Assert.Contains("DateOfBirth", exception.ValidationErrors.Keys);
    }

    [Fact(DisplayName = "PlayerValidationException can be created with custom message")]
    public void PlayerValidationException_WithCustomMessage_HasCustomMessage()
    {
        var customMessage = "Custom validation error";

        var exception = new PlayerValidationException(customMessage);

        Assert.Equal(customMessage, exception.Message);
        Assert.Empty(exception.ValidationErrors);
    }

    [Fact(DisplayName = "PlayerValidationException can be created with message and validation errors")]
    public void PlayerValidationException_WithMessageAndErrors_HasBoth()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } }
        };
        var message = "Custom validation message";

        var exception = new PlayerValidationException(message, errors);

        Assert.Equal(message, exception.Message);
        Assert.Single(exception.ValidationErrors);
    }

    [Fact(DisplayName = "PlayerValidationException can be created with message and inner exception")]
    public void PlayerValidationException_WithMessageAndInnerException_PreservesInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var message = "Validation failed";

        var exception = new PlayerValidationException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
        Assert.Empty(exception.ValidationErrors);
    }

    [Fact(DisplayName = "PlayerValidationException throws when validation errors is null")]
    public void PlayerValidationException_WithNullErrors_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerValidationException((IDictionary<string, string[]>)null!));
    }

    [Fact(DisplayName = "PlayerValidationException throws when validation errors with message is null")]
    public void PlayerValidationException_WithNullErrorsAndMessage_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new PlayerValidationException("message", (IDictionary<string, string[]>)null!));
    }

    [Fact(DisplayName = "PlayerValidationException is derived from Exception")]
    public void PlayerValidationException_IsDerivedFromException()
    {
        var exception = new PlayerValidationException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "PlayerValidationException can be thrown and caught")]
    public void PlayerValidationException_CanBeThrownAndCaught()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } }
        };

        void ThrowException() => throw new PlayerValidationException(errors);

        var exception = Assert.Throws<PlayerValidationException>(ThrowException);

        Assert.NotNull(exception);
        Assert.Single(exception.ValidationErrors);
    }

    [Fact(DisplayName = "PlayerValidationException validation errors are immutable")]
    public void PlayerValidationException_ValidationErrors_AreImmutable()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } }
        };

        var exception = new PlayerValidationException(errors);

        errors["NewField"] = new[] { "New error" };

        Assert.Single(exception.ValidationErrors);
        Assert.False(exception.ValidationErrors.ContainsKey("NewField"));
    }
}
