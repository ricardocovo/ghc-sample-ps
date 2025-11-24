using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class TokenValidationExceptionTests
{
    [Fact(DisplayName = "TokenValidationException can be created with default message")]
    public void TokenValidationException_WithDefaultConstructor_HasDefaultMessage()
    {
        var exception = new TokenValidationException();

        Assert.NotNull(exception);
        Assert.Equal("Invalid or expired authentication token.", exception.Message);
    }

    [Fact(DisplayName = "TokenValidationException can be created with custom message")]
    public void TokenValidationException_WithCustomMessage_HasCustomMessage()
    {
        var customMessage = "Custom token validation error";

        var exception = new TokenValidationException(customMessage);

        Assert.Equal(customMessage, exception.Message);
    }

    [Fact(DisplayName = "TokenValidationException can be created with message and inner exception")]
    public void TokenValidationException_WithMessageAndInnerException_PreservesInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var message = "Token validation failed";

        var exception = new TokenValidationException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "TokenValidationException is derived from AuthenticationException")]
    public void TokenValidationException_IsDerivedFromAuthenticationException()
    {
        var exception = new TokenValidationException();

        Assert.IsAssignableFrom<AuthenticationException>(exception);
    }

    [Fact(DisplayName = "TokenValidationException is derived from Exception")]
    public void TokenValidationException_IsDerivedFromException()
    {
        var exception = new TokenValidationException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "TokenValidationException can be thrown and caught")]
    public void TokenValidationException_CanBeThrownAndCaught()
    {
        void ThrowException() => throw new TokenValidationException();

        var exception = Assert.Throws<TokenValidationException>(ThrowException);

        Assert.NotNull(exception);
    }
}
