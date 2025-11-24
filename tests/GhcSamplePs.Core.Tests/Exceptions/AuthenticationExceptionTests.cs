using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class AuthenticationExceptionTests
{
    [Fact(DisplayName = "AuthenticationException can be created with default message")]
    public void AuthenticationException_WithDefaultConstructor_HasDefaultMessage()
    {
        var exception = new AuthenticationException();

        Assert.NotNull(exception);
        Assert.Equal("Authentication failed. Please try signing in again.", exception.Message);
    }

    [Fact(DisplayName = "AuthenticationException can be created with custom message")]
    public void AuthenticationException_WithCustomMessage_HasCustomMessage()
    {
        var customMessage = "Custom authentication error";

        var exception = new AuthenticationException(customMessage);

        Assert.Equal(customMessage, exception.Message);
    }

    [Fact(DisplayName = "AuthenticationException can be created with message and inner exception")]
    public void AuthenticationException_WithMessageAndInnerException_PreservesInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var message = "Authentication failed";

        var exception = new AuthenticationException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "AuthenticationException is derived from Exception")]
    public void AuthenticationException_IsDerivedFromException()
    {
        var exception = new AuthenticationException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "AuthenticationException can be thrown and caught")]
    public void AuthenticationException_CanBeThrownAndCaught()
    {
        void ThrowException() => throw new AuthenticationException();

        var exception = Assert.Throws<AuthenticationException>(ThrowException);

        Assert.NotNull(exception);
    }
}
