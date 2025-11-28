using GhcSamplePs.Core.Common;

namespace GhcSamplePs.Core.Tests.Common;

public class ServiceResultTests
{
    [Fact(DisplayName = "Ok with data creates a successful result")]
    public void Ok_WithData_ReturnsSuccessfulResult()
    {
        var data = "test data";

        var result = ServiceResult<string>.Ok(data);

        Assert.True(result.Success);
        Assert.Equal(data, result.Data);
        Assert.Empty(result.ErrorMessages);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact(DisplayName = "Ok without data creates a successful result")]
    public void Ok_WithoutData_ReturnsSuccessfulResult()
    {
        var result = ServiceResult<string>.Ok();

        Assert.True(result.Success);
        Assert.Null(result.Data);
        Assert.Empty(result.ErrorMessages);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact(DisplayName = "Ok with null data creates a successful result")]
    public void Ok_WithNullData_ReturnsSuccessfulResult()
    {
        var result = ServiceResult<string>.Ok(null!);

        Assert.True(result.Success);
        Assert.Null(result.Data);
    }

    [Fact(DisplayName = "Fail with single message creates a failed result")]
    public void Fail_WithSingleMessage_ReturnsFailedResult()
    {
        var errorMessage = "Something went wrong";

        var result = ServiceResult<string>.Fail(errorMessage);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Single(result.ErrorMessages);
        Assert.Equal(errorMessage, result.ErrorMessages[0]);
        Assert.Empty(result.ValidationErrors);
    }

    [Fact(DisplayName = "Fail throws when errorMessage is null")]
    public void Fail_WhenErrorMessageNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.Fail((string)null!));
    }

    [Fact(DisplayName = "Fail throws when errorMessage is whitespace")]
    public void Fail_WhenErrorMessageWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.Fail("   "));
    }

    [Fact(DisplayName = "Fail throws when errorMessage is empty")]
    public void Fail_WhenErrorMessageEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.Fail(string.Empty));
    }

    [Fact(DisplayName = "Fail with multiple messages creates a failed result")]
    public void Fail_WithMultipleMessages_ReturnsFailedResult()
    {
        var errorMessages = new[] { "Error 1", "Error 2" };

        var result = ServiceResult<string>.Fail(errorMessages);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(2, result.ErrorMessages.Count);
        Assert.Contains("Error 1", result.ErrorMessages);
        Assert.Contains("Error 2", result.ErrorMessages);
    }

    [Fact(DisplayName = "Fail with collection throws when collection is null")]
    public void Fail_WithCollectionNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceResult<string>.Fail((IEnumerable<string>)null!));
    }

    [Fact(DisplayName = "Fail with collection throws when collection is empty")]
    public void Fail_WithCollectionEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.Fail(Enumerable.Empty<string>()));
    }

    [Fact(DisplayName = "ValidationFailed with dictionary creates a failed result")]
    public void ValidationFailed_WithDictionary_ReturnsFailedResult()
    {
        var validationErrors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "Email", new[] { "Invalid email format" } }
        };

        var result = ServiceResult<string>.ValidationFailed(validationErrors);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Empty(result.ErrorMessages);
        Assert.Equal(2, result.ValidationErrors.Count);
        Assert.Contains("Name is required", result.ValidationErrors["Name"]);
        Assert.Contains("Invalid email format", result.ValidationErrors["Email"]);
    }

    [Fact(DisplayName = "ValidationFailed with dictionary throws when null")]
    public void ValidationFailed_WithDictionaryNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceResult<string>.ValidationFailed((IDictionary<string, string[]>)null!));
    }

    [Fact(DisplayName = "ValidationFailed with ValidationResult creates a failed result")]
    public void ValidationFailed_WithValidationResult_ReturnsFailedResult()
    {
        var validationResult = ValidationResult.Invalid("Name", "Name is required");

        var result = ServiceResult<string>.ValidationFailed(validationResult);

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Empty(result.ErrorMessages);
        Assert.Single(result.ValidationErrors);
        Assert.Contains("Name is required", result.ValidationErrors["Name"]);
    }

    [Fact(DisplayName = "ValidationFailed with ValidationResult throws when null")]
    public void ValidationFailed_WithValidationResultNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ServiceResult<string>.ValidationFailed((ValidationResult)null!));
    }

    [Fact(DisplayName = "ValidationFailed with single error creates a failed result")]
    public void ValidationFailed_WithSingleError_ReturnsFailedResult()
    {
        var result = ServiceResult<string>.ValidationFailed("Name", "Name is required");

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Empty(result.ErrorMessages);
        Assert.Single(result.ValidationErrors);
        Assert.Contains("Name is required", result.ValidationErrors["Name"]);
    }

    [Fact(DisplayName = "ValidationFailed with single error throws when fieldName is null")]
    public void ValidationFailed_WithSingleError_WhenFieldNameNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.ValidationFailed(null!, "Error"));
    }

    [Fact(DisplayName = "ValidationFailed with single error throws when fieldName is whitespace")]
    public void ValidationFailed_WithSingleError_WhenFieldNameWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.ValidationFailed("   ", "Error"));
    }

    [Fact(DisplayName = "ValidationFailed with single error throws when errorMessage is null")]
    public void ValidationFailed_WithSingleError_WhenErrorMessageNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.ValidationFailed("Name", null!));
    }

    [Fact(DisplayName = "ValidationFailed with single error throws when errorMessage is whitespace")]
    public void ValidationFailed_WithSingleError_WhenErrorMessageWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceResult<string>.ValidationFailed("Name", "   "));
    }

    [Fact(DisplayName = "ServiceResult works with value types")]
    public void ServiceResult_WithValueType_WorksCorrectly()
    {
        var result = ServiceResult<int>.Ok(42);

        Assert.True(result.Success);
        Assert.Equal(42, result.Data);
    }

    [Fact(DisplayName = "ServiceResult works with complex types")]
    public void ServiceResult_WithComplexType_WorksCorrectly()
    {
        var data = new TestEntity { Id = 1, Name = "Test" };

        var result = ServiceResult<TestEntity>.Ok(data);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
        Assert.Equal("Test", result.Data.Name);
    }

    [Fact(DisplayName = "ValidationErrors dictionary is read-only")]
    public void ValidationErrors_WhenModificationAttempted_OriginalUnchanged()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Error" } }
        };
        var result = ServiceResult<string>.ValidationFailed(errors);

        // Verify the original dictionary modification doesn't affect result
        errors.Add("NewField", new[] { "New error" });

        Assert.Single(result.ValidationErrors);
    }

    [Fact(DisplayName = "Multiple Ok calls create distinct instances")]
    public void Ok_MultipleCalls_CreateDistinctInstances()
    {
        var result1 = ServiceResult<string>.Ok("test");
        var result2 = ServiceResult<string>.Ok("test");

        Assert.NotSame(result1, result2);
    }

    private sealed class TestEntity
    {
        public int Id { get; init; }
        public required string Name { get; init; }
    }
}
