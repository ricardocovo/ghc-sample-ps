using GhcSamplePs.Core.Services.Interfaces;

namespace GhcSamplePs.Core.Tests.Services;

public class AuthorizationResultTests
{
    [Fact(DisplayName = "Success creates a successful authorization result")]
    public void Success_CreatesSuccessfulResult()
    {
        var result = AuthorizationResult.Success();

        Assert.True(result.Succeeded);
        Assert.Null(result.FailureReason);
        Assert.Empty(result.MissingPermissions);
    }

    [Fact(DisplayName = "Failure creates a failed authorization result with reason")]
    public void Failure_WithReason_CreatesFailedResult()
    {
        var reason = "User lacks required role";

        var result = AuthorizationResult.Failure(reason);

        Assert.False(result.Succeeded);
        Assert.Equal(reason, result.FailureReason);
        Assert.Empty(result.MissingPermissions);
    }

    [Fact(DisplayName = "Failure creates a failed authorization result with missing permissions")]
    public void Failure_WithMissingPermissions_CreatesFailedResultWithPermissions()
    {
        var reason = "User lacks required permissions";
        var missingPermissions = new List<string> { "Admin", "Write" };

        var result = AuthorizationResult.Failure(reason, missingPermissions);

        Assert.False(result.Succeeded);
        Assert.Equal(reason, result.FailureReason);
        Assert.Equal(2, result.MissingPermissions.Count);
        Assert.Contains("Admin", result.MissingPermissions);
        Assert.Contains("Write", result.MissingPermissions);
    }

    [Fact(DisplayName = "Failure throws when reason is invalid")]
    public void Failure_WhenReasonIsInvalid_ThrowsArgumentException()
    {
        Assert.ThrowsAny<ArgumentException>(() => AuthorizationResult.Failure(null!));
        Assert.ThrowsAny<ArgumentException>(() => AuthorizationResult.Failure(string.Empty));
        Assert.ThrowsAny<ArgumentException>(() => AuthorizationResult.Failure("   "));
    }

    [Fact(DisplayName = "Failure with null missing permissions creates empty list")]
    public void Failure_WithNullMissingPermissions_CreatesEmptyList()
    {
        var reason = "Test reason";

        var result = AuthorizationResult.Failure(reason, null);

        Assert.Empty(result.MissingPermissions);
    }

    [Fact(DisplayName = "AuthorizationResult MissingPermissions is read-only")]
    public void AuthorizationResult_MissingPermissions_IsReadOnly()
    {
        var result = AuthorizationResult.Success();

        Assert.IsAssignableFrom<IReadOnlyList<string>>(result.MissingPermissions);
    }

    [Fact(DisplayName = "Multiple Failure calls create distinct instances")]
    public void Failure_MultipleCallsCreateDistinctInstances()
    {
        var result1 = AuthorizationResult.Failure("Reason 1");
        var result2 = AuthorizationResult.Failure("Reason 2");

        Assert.NotSame(result1, result2);
        Assert.Equal("Reason 1", result1.FailureReason);
        Assert.Equal("Reason 2", result2.FailureReason);
    }

    [Fact(DisplayName = "Multiple Success calls create distinct instances")]
    public void Success_MultipleCallsCreateDistinctInstances()
    {
        var result1 = AuthorizationResult.Success();
        var result2 = AuthorizationResult.Success();

        Assert.NotSame(result1, result2);
    }
}
