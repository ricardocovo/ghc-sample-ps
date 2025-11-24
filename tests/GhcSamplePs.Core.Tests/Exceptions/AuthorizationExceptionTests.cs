using GhcSamplePs.Core.Exceptions;

namespace GhcSamplePs.Core.Tests.Exceptions;

public class AuthorizationExceptionTests
{
    [Fact(DisplayName = "AuthorizationException can be created with default message")]
    public void AuthorizationException_WithDefaultConstructor_HasDefaultMessage()
    {
        var exception = new AuthorizationException();

        Assert.NotNull(exception);
        Assert.Equal("You do not have permission to access this resource.", exception.Message);
    }

    [Fact(DisplayName = "AuthorizationException can be created with custom message")]
    public void AuthorizationException_WithCustomMessage_HasCustomMessage()
    {
        var customMessage = "Custom authorization error";

        var exception = new AuthorizationException(customMessage);

        Assert.Equal(customMessage, exception.Message);
    }

    [Fact(DisplayName = "AuthorizationException can be created with message and inner exception")]
    public void AuthorizationException_WithMessageAndInnerException_PreservesInnerException()
    {
        var innerException = new InvalidOperationException("Inner error");
        var message = "Authorization failed";

        var exception = new AuthorizationException(message, innerException);

        Assert.Equal(message, exception.Message);
        Assert.Same(innerException, exception.InnerException);
    }

    [Fact(DisplayName = "AuthorizationException can have Resource property set")]
    public void AuthorizationException_WithResource_StoresResourceName()
    {
        var exception = new AuthorizationException
        {
            Resource = "SecureDocument"
        };

        Assert.Equal("SecureDocument", exception.Resource);
    }

    [Fact(DisplayName = "AuthorizationException can have RequiredPermission property set")]
    public void AuthorizationException_WithRequiredPermission_StoresPermission()
    {
        var exception = new AuthorizationException
        {
            RequiredPermission = "Admin"
        };

        Assert.Equal("Admin", exception.RequiredPermission);
    }

    [Fact(DisplayName = "AuthorizationException can have both Resource and RequiredPermission set")]
    public void AuthorizationException_WithResourceAndPermission_StoresBoth()
    {
        var exception = new AuthorizationException
        {
            Resource = "SecureDocument",
            RequiredPermission = "Admin"
        };

        Assert.Equal("SecureDocument", exception.Resource);
        Assert.Equal("Admin", exception.RequiredPermission);
    }

    [Fact(DisplayName = "AuthorizationException is derived from Exception")]
    public void AuthorizationException_IsDerivedFromException()
    {
        var exception = new AuthorizationException();

        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact(DisplayName = "AuthorizationException can be thrown and caught")]
    public void AuthorizationException_CanBeThrownAndCaught()
    {
        void ThrowException() => throw new AuthorizationException();

        var exception = Assert.Throws<AuthorizationException>(ThrowException);

        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "AuthorizationException Resource defaults to null")]
    public void AuthorizationException_ResourceDefaultsToNull()
    {
        var exception = new AuthorizationException();

        Assert.Null(exception.Resource);
    }

    [Fact(DisplayName = "AuthorizationException RequiredPermission defaults to null")]
    public void AuthorizationException_RequiredPermissionDefaultsToNull()
    {
        var exception = new AuthorizationException();

        Assert.Null(exception.RequiredPermission);
    }
}
