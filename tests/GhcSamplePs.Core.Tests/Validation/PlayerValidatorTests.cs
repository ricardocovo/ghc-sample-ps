using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Validation;

namespace GhcSamplePs.Core.Tests.Validation;

public class PlayerValidatorTests
{
    [Fact(DisplayName = "Validate returns success for valid CreatePlayerDto")]
    public void Validate_WithValidCreatePlayerDto_ReturnsSuccess()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Validate returns success for valid UpdatePlayerDto")]
    public void Validate_WithValidUpdatePlayerDto_ReturnsSuccess()
    {
        var dto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Female"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Validate returns error when UserId is empty")]
    public void Validate_WithEmptyUserId_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.UserId)));
    }

    [Fact(DisplayName = "Validate returns error when UserId is whitespace")]
    public void Validate_WithWhitespaceUserId_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "   ",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.UserId)));
    }

    [Fact(DisplayName = "Validate returns error when Name is empty")]
    public void Validate_WithEmptyName_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.Name)));
    }

    [Fact(DisplayName = "Validate returns error when Name exceeds max length")]
    public void Validate_WithNameTooLong_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = new string('A', 201),
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.Name)));
        Assert.Contains("200", result.Errors[nameof(CreatePlayerDto.Name)][0]);
    }

    [Fact(DisplayName = "Validate returns error when DateOfBirth is in the future")]
    public void Validate_WithFutureDateOfBirth_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(1)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.DateOfBirth)));
        Assert.Contains("past", result.Errors[nameof(CreatePlayerDto.DateOfBirth)][0]);
    }

    [Fact(DisplayName = "Validate returns error when DateOfBirth is today")]
    public void Validate_WithTodayDateOfBirth_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.DateOfBirth)));
    }

    [Fact(DisplayName = "Validate returns error when DateOfBirth is more than 100 years ago")]
    public void Validate_WithDateOfBirthTooOld_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.AddYears(-101)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.DateOfBirth)));
        Assert.Contains("100 years", result.Errors[nameof(CreatePlayerDto.DateOfBirth)][0]);
    }

    [Fact(DisplayName = "Validate returns error when Gender is invalid")]
    public void Validate_WithInvalidGender_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = "Invalid Gender"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.Gender)));
        Assert.Contains("must be one of", result.Errors[nameof(CreatePlayerDto.Gender)][0]);
    }

    [Theory(DisplayName = "Validate accepts all valid gender options")]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Non-binary")]
    [InlineData("Prefer not to say")]
    public void Validate_WithValidGender_ReturnsSuccess(string gender)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = gender
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate accepts null gender")]
    public void Validate_WithNullGender_ReturnsSuccess()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            Gender = null
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate returns error when PhotoUrl exceeds max length")]
    public void Validate_WithPhotoUrlTooLong_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            PhotoUrl = "https://example.com/" + new string('a', 500)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.PhotoUrl)));
        Assert.Contains("500", result.Errors[nameof(CreatePlayerDto.PhotoUrl)][0]);
    }

    [Fact(DisplayName = "Validate returns error when PhotoUrl is not a valid URL")]
    public void Validate_WithInvalidPhotoUrl_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            PhotoUrl = "not-a-valid-url"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.PhotoUrl)));
        Assert.Contains("valid HTTP or HTTPS URL", result.Errors[nameof(CreatePlayerDto.PhotoUrl)][0]);
    }

    [Fact(DisplayName = "Validate returns error when PhotoUrl uses non-HTTP scheme")]
    public void Validate_WithNonHttpPhotoUrl_ReturnsError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            PhotoUrl = "ftp://example.com/photo.jpg"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey(nameof(CreatePlayerDto.PhotoUrl)));
    }

    [Theory(DisplayName = "Validate accepts valid HTTP and HTTPS URLs")]
    [InlineData("http://example.com/photo.jpg")]
    [InlineData("https://example.com/photo.jpg")]
    [InlineData("https://subdomain.example.com/path/to/photo.jpg")]
    public void Validate_WithValidPhotoUrl_ReturnsSuccess(string photoUrl)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            PhotoUrl = photoUrl
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate accepts null PhotoUrl")]
    public void Validate_WithNullPhotoUrl_ReturnsSuccess()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(2010, 5, 15),
            PhotoUrl = null
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate throws when CreatePlayerDto is null")]
    public void Validate_WithNullCreatePlayerDto_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PlayerValidator.Validate((CreatePlayerDto)null!));
    }

    [Fact(DisplayName = "Validate throws when UpdatePlayerDto is null")]
    public void Validate_WithNullUpdatePlayerDto_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PlayerValidator.Validate((UpdatePlayerDto)null!));
    }

    [Fact(DisplayName = "Validate returns multiple errors when multiple fields are invalid")]
    public void Validate_WithMultipleInvalidFields_ReturnsMultipleErrors()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "",
            Name = "",
            DateOfBirth = DateTime.UtcNow.AddYears(1),
            Gender = "Invalid",
            PhotoUrl = "invalid-url"
        };

        var result = PlayerValidator.Validate(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4);
    }

    [Fact(DisplayName = "Validate trims whitespace from name when checking length")]
    public void Validate_WithWhitespacePaddedName_TrimsAndValidates()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "   " + new string('A', 200) + "   ",
            DateOfBirth = new DateTime(2010, 5, 15)
        };

        var result = PlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "ValidGenderOptions constant contains expected values")]
    public void ValidGenderOptions_ContainsExpectedValues()
    {
        Assert.Equal(4, PlayerValidator.ValidGenderOptions.Count);
        Assert.Contains("Male", PlayerValidator.ValidGenderOptions);
        Assert.Contains("Female", PlayerValidator.ValidGenderOptions);
        Assert.Contains("Non-binary", PlayerValidator.ValidGenderOptions);
        Assert.Contains("Prefer not to say", PlayerValidator.ValidGenderOptions);
    }

    [Fact(DisplayName = "MaxNameLength constant has expected value")]
    public void MaxNameLength_HasExpectedValue()
    {
        Assert.Equal(200, PlayerValidator.MaxNameLength);
    }

    [Fact(DisplayName = "MaxPhotoUrlLength constant has expected value")]
    public void MaxPhotoUrlLength_HasExpectedValue()
    {
        Assert.Equal(500, PlayerValidator.MaxPhotoUrlLength);
    }

    [Fact(DisplayName = "MaxAgeInYears constant has expected value")]
    public void MaxAgeInYears_HasExpectedValue()
    {
        Assert.Equal(100, PlayerValidator.MaxAgeInYears);
    }
}
