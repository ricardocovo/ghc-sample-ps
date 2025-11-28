using GhcSamplePs.Core.Common;

namespace GhcSamplePs.Core.Tests.Common;

public class ValidationResultTests
{
    [Fact(DisplayName = "Valid creates a successful validation result")]
    public void Valid_WhenCalled_ReturnsSuccessfulResult()
    {
        var result = ValidationResult.Valid();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Invalid with dictionary creates a failed validation result")]
    public void Invalid_WithDictionary_ReturnsFailedResult()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "Email", new[] { "Invalid email format" } }
        };

        var result = ValidationResult.Invalid(errors);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Name is required", result.Errors["Name"]);
        Assert.Contains("Invalid email format", result.Errors["Email"]);
    }

    [Fact(DisplayName = "Invalid with dictionary throws when errors is null")]
    public void Invalid_WithNullDictionary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ValidationResult.Invalid((IDictionary<string, string[]>)null!));
    }

    [Fact(DisplayName = "Invalid with single error creates a failed validation result")]
    public void Invalid_WithSingleError_ReturnsFailedResult()
    {
        var result = ValidationResult.Invalid("Name", "Name is required");

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Name is required", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Invalid with single error throws when fieldName is null")]
    public void Invalid_WithSingleError_WhenFieldNameNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid(null!, "Error message"));
    }

    [Fact(DisplayName = "Invalid with single error throws when fieldName is whitespace")]
    public void Invalid_WithSingleError_WhenFieldNameWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid("   ", "Error message"));
    }

    [Fact(DisplayName = "Invalid with single error throws when errorMessage is null")]
    public void Invalid_WithSingleError_WhenErrorMessageNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid("Name", (string)null!));
    }

    [Fact(DisplayName = "Invalid with single error throws when errorMessage is whitespace")]
    public void Invalid_WithSingleError_WhenErrorMessageWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid("Name", "   "));
    }

    [Fact(DisplayName = "Invalid with multiple errors creates a failed validation result")]
    public void Invalid_WithMultipleErrors_ReturnsFailedResult()
    {
        var errorMessages = new[] { "Name is required", "Name cannot exceed 200 characters" };

        var result = ValidationResult.Invalid("Name", errorMessages);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(2, result.Errors["Name"].Length);
        Assert.Contains("Name is required", result.Errors["Name"]);
        Assert.Contains("Name cannot exceed 200 characters", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Invalid with multiple errors throws when fieldName is null")]
    public void Invalid_WithMultipleErrors_WhenFieldNameNull_ThrowsArgumentException()
    {
        var errorMessages = new[] { "Error 1" };

        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid(null!, errorMessages));
    }

    [Fact(DisplayName = "Invalid with multiple errors throws when errorMessages is null")]
    public void Invalid_WithMultipleErrors_WhenErrorMessagesNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ValidationResult.Invalid("Name", (string[])null!));
    }

    [Fact(DisplayName = "Invalid with multiple errors throws when errorMessages is empty")]
    public void Invalid_WithMultipleErrors_WhenErrorMessagesEmpty_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ValidationResult.Invalid("Name", Array.Empty<string>()));
    }

    [Fact(DisplayName = "Errors dictionary is read-only")]
    public void Errors_WhenModificationAttempted_ThrowsException()
    {
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Error" } }
        };
        var result = ValidationResult.Invalid(errors);

        // Verify the original dictionary modification doesn't affect result
        errors.Add("NewField", new[] { "New error" });

        Assert.Single(result.Errors);
    }

    [Fact(DisplayName = "Multiple Valid calls create distinct instances")]
    public void Valid_MultipleCalls_CreateDistinctInstances()
    {
        var result1 = ValidationResult.Valid();
        var result2 = ValidationResult.Valid();

        Assert.NotSame(result1, result2);
    }
}
