using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class PlayerNotFoundExceptionTests
{
    [Fact(DisplayName = "PlayerNotFoundException can be created with default message")]
    public void PlayerNotFoundException_WithDefaultConstructor_HasDefaultMessage()
    {
        var exception = new PlayerNotFoundException();

        Assert.NotNull(exception);
        Assert.Equal("The requested player could not be found.", exception.Message);
        Assert.Null(exception.PlayerId);
    }

    [Fact(DisplayName = "PlayerNotFoundException can be created with player ID")]
    public void PlayerNotFoundException_WithPlayerId_HasPlayerIdInMessage()
    {
        var exception = new PlayerNotFoundException(42);

        Assert.NotNull(exception);
        Assert.Contains("42", exception.Message);
        Assert.Equal(42, exception.PlayerId);
    }

    [Fact(DisplayName = "PlayerNotFoundException can be created with custom message")]
    public void PlayerNotFoundException_WithCustomMessage_HasCustomMessage()
    {
        var customMessage = "Custom not found error";

        var exception = new PlayerNotFoundException(customMessage);

        Assert.Equal(customMessage, exception.Message);
        Assert.Null(exception.PlayerId);
    }

    [Fact(DisplayName = "PlayerNotFoundException can be created with message and inner exception")]
    public void PlayerNotFoundException_WithMessageAndInnerException_PreservesInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var message = "Player not found";

        var exception = new PlayerNotFoundException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "PlayerNotFoundException is derived from Exception")]
    public void PlayerNotFoundException_IsDerivedFromException()
    {
        var exception = new PlayerNotFoundException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "PlayerNotFoundException can be thrown and caught")]
    public void PlayerNotFoundException_CanBeThrownAndCaught()
    {
        void ThrowException() => throw new PlayerNotFoundException(1);

        var exception = Assert.Throws<PlayerNotFoundException>(ThrowException);

        Assert.NotNull(exception);
        Assert.Equal(1, exception.PlayerId);
    }
}
