using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Validation;

namespace GhcSamplePs.Core.Tests.Validation;

public class PlayerValidatorTests
{
    #region ValidateCreatePlayer Tests

    [Fact(DisplayName = "ValidateCreatePlayer returns valid for valid CreatePlayerDto")]
    public void ValidateCreatePlayer_WithValidDto_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateCreatePlayer returns valid with minimal required fields")]
    public void ValidateCreatePlayer_WithMinimalRequiredFields_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateCreatePlayer throws when dto is null")]
    public void ValidateCreatePlayer_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerValidator.ValidateCreatePlayer(null!));
    }

    #endregion

    #region ValidateUpdatePlayer Tests

    [Fact(DisplayName = "ValidateUpdatePlayer returns valid for valid UpdatePlayerDto")]
    public void ValidateUpdatePlayer_WithValidDto_ReturnsValid()
    {
        var dto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "John Doe Updated",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Female",
            PhotoUrl = "https://example.com/updated-photo.jpg"
        };

        var result = PlayerValidator.ValidateUpdatePlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateUpdatePlayer returns valid with minimal required fields")]
    public void ValidateUpdatePlayer_WithMinimalRequiredFields_ReturnsValid()
    {
        var dto = new UpdatePlayerDto
        {
            Id = 1,
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateUpdatePlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateUpdatePlayer throws when dto is null")]
    public void ValidateUpdatePlayer_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerValidator.ValidateUpdatePlayer(null!));
    }

    #endregion

    #region ValidatePlayer Tests

    [Fact(DisplayName = "ValidatePlayer returns valid for valid Player entity")]
    public void ValidatePlayer_WithValidPlayer_ReturnsValid()
    {
        var player = new Player
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photo.jpg",
            CreatedBy = "system"
        };

        var result = PlayerValidator.ValidatePlayer(player);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidatePlayer throws when player is null")]
    public void ValidatePlayer_WhenPlayerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerValidator.ValidatePlayer(null!));
    }

    #endregion

    #region Name Validation Tests

    [Fact(DisplayName = "Name validation returns error when name is null")]
    public void ValidateCreatePlayer_WhenNameIsNull_ReturnsNameRequiredError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = null!,
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Name"));
        Assert.Contains("Name is required", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Name validation returns error when name is empty")]
    public void ValidateCreatePlayer_WhenNameIsEmpty_ReturnsNameRequiredError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Name"));
        Assert.Contains("Name is required", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Name validation returns error when name is whitespace only")]
    public void ValidateCreatePlayer_WhenNameIsWhitespace_ReturnsNameRequiredError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "   ",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Name"));
        Assert.Contains("Name is required", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Name validation returns error when name exceeds 200 characters")]
    public void ValidateCreatePlayer_WhenNameExceeds200Characters_ReturnsNameTooLongError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = new string('A', 201),
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Name"));
        Assert.Contains("Name must not exceed 200 characters", result.Errors["Name"]);
    }

    [Fact(DisplayName = "Name validation succeeds when name is exactly 200 characters")]
    public void ValidateCreatePlayer_WhenNameIsExactly200Characters_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = new string('A', 200),
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Name validation succeeds when name is 1 character")]
    public void ValidateCreatePlayer_WhenNameIsOneCharacter_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "A",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Name validation trims whitespace before checking length")]
    public void ValidateCreatePlayer_NameWithPaddingWhitespace_TrimsBeforeValidation()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "  " + new string('A', 200) + "  ",
            DateOfBirth = new DateTime(1990, 6, 15)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region DateOfBirth Validation Tests

    [Fact(DisplayName = "DateOfBirth validation returns error when DateOfBirth is default")]
    public void ValidateCreatePlayer_WhenDateOfBirthIsDefault_ReturnsDateOfBirthRequiredError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = default
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("DateOfBirth"));
        Assert.Contains("Date of birth is required", result.Errors["DateOfBirth"]);
    }

    [Fact(DisplayName = "DateOfBirth validation returns error when DateOfBirth is today")]
    public void ValidateCreatePlayer_WhenDateOfBirthIsToday_ReturnsDateOfBirthMustBePastError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("DateOfBirth"));
        Assert.Contains("Date of birth must be in the past", result.Errors["DateOfBirth"]);
    }

    [Fact(DisplayName = "DateOfBirth validation returns error when DateOfBirth is in the future")]
    public void ValidateCreatePlayer_WhenDateOfBirthIsFuture_ReturnsDateOfBirthMustBePastError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date.AddDays(1)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("DateOfBirth"));
        Assert.Contains("Date of birth must be in the past", result.Errors["DateOfBirth"]);
    }

    [Fact(DisplayName = "DateOfBirth validation returns error when DateOfBirth is more than 100 years ago")]
    public void ValidateCreatePlayer_WhenDateOfBirthMoreThan100YearsAgo_ReturnsDateOfBirthTooOldError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date.AddYears(-101)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("DateOfBirth"));
        Assert.Contains("Date of birth cannot be more than 100 years ago", result.Errors["DateOfBirth"]);
    }

    [Fact(DisplayName = "DateOfBirth validation succeeds when DateOfBirth is exactly 100 years ago")]
    public void ValidateCreatePlayer_WhenDateOfBirthIsExactly100YearsAgo_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date.AddYears(-100)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "DateOfBirth validation succeeds when DateOfBirth is yesterday")]
    public void ValidateCreatePlayer_WhenDateOfBirthIsYesterday_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = DateTime.UtcNow.Date.AddDays(-1)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Gender Validation Tests

    [Fact(DisplayName = "Gender validation succeeds when Gender is null")]
    public void ValidateCreatePlayer_WhenGenderIsNull_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = null
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Gender validation succeeds when Gender is empty")]
    public void ValidateCreatePlayer_WhenGenderIsEmpty_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = ""
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Theory(DisplayName = "Gender validation succeeds for valid gender options")]
    [InlineData("Male")]
    [InlineData("Female")]
    [InlineData("Non-binary")]
    [InlineData("Prefer not to say")]
    public void ValidateCreatePlayer_WithValidGenderOption_ReturnsValid(string gender)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = gender
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Theory(DisplayName = "Gender validation is case-insensitive")]
    [InlineData("male")]
    [InlineData("MALE")]
    [InlineData("MaLe")]
    [InlineData("female")]
    [InlineData("FEMALE")]
    [InlineData("non-binary")]
    [InlineData("NON-BINARY")]
    [InlineData("prefer not to say")]
    [InlineData("PREFER NOT TO SAY")]
    public void ValidateCreatePlayer_WithCaseInsensitiveGender_ReturnsValid(string gender)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = gender
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Gender validation returns error for invalid gender option")]
    public void ValidateCreatePlayer_WithInvalidGender_ReturnsGenderInvalidError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "InvalidGender"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Gender"));
        Assert.Contains("Gender must be Male, Female, Non-binary, or Prefer not to say", result.Errors["Gender"]);
    }

    [Theory(DisplayName = "Gender validation returns error for invalid gender values")]
    [InlineData("M")]
    [InlineData("F")]
    [InlineData("Other")]
    [InlineData("Unknown")]
    [InlineData("Not specified")]
    public void ValidateCreatePlayer_WithInvalidGenderValues_ReturnsGenderInvalidError(string gender)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = gender
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("Gender"));
        Assert.Contains("Gender must be Male, Female, Non-binary, or Prefer not to say", result.Errors["Gender"]);
    }

    [Fact(DisplayName = "Gender validation trims whitespace before validation")]
    public void ValidateCreatePlayer_GenderWithWhitespace_TrimsBeforeValidation()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "  Male  "
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region PhotoUrl Validation Tests

    [Fact(DisplayName = "PhotoUrl validation succeeds when PhotoUrl is null")]
    public void ValidateCreatePlayer_WhenPhotoUrlIsNull_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = null
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "PhotoUrl validation succeeds when PhotoUrl is empty")]
    public void ValidateCreatePlayer_WhenPhotoUrlIsEmpty_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = ""
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "PhotoUrl validation succeeds for valid HTTPS URL")]
    public void ValidateCreatePlayer_WithValidHttpsUrl_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = "https://example.com/photos/player.jpg"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "PhotoUrl validation succeeds for valid HTTP URL")]
    public void ValidateCreatePlayer_WithValidHttpUrl_ReturnsValid()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = "http://example.com/photos/player.jpg"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "PhotoUrl validation returns error when PhotoUrl exceeds 500 characters")]
    public void ValidateCreatePlayer_WhenPhotoUrlExceeds500Characters_ReturnsPhotoUrlTooLongError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = "https://example.com/" + new string('a', 490)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("PhotoUrl"));
        Assert.Contains("Photo URL must not exceed 500 characters", result.Errors["PhotoUrl"]);
    }

    [Fact(DisplayName = "PhotoUrl validation succeeds when PhotoUrl is exactly 500 characters")]
    public void ValidateCreatePlayer_WhenPhotoUrlIsExactly500Characters_ReturnsValid()
    {
        var baseUrl = "https://example.com/";
        var padding = new string('a', 500 - baseUrl.Length);
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = baseUrl + padding
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "PhotoUrl validation returns error for invalid URL format")]
    public void ValidateCreatePlayer_WithInvalidUrlFormat_ReturnsPhotoUrlInvalidError()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = "not-a-valid-url"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("PhotoUrl", out var photoUrlErrors), "Expected 'PhotoUrl' key in errors.");
        Assert.Contains("Photo URL must be a valid HTTP or HTTPS URL", photoUrlErrors);
    }

    [Theory(DisplayName = "PhotoUrl validation returns error for non-HTTP(S) schemes")]
    [InlineData("ftp://example.com/photo.jpg")]
    [InlineData("file:///path/to/photo.jpg")]
    [InlineData("mailto:test@example.com")]
    public void ValidateCreatePlayer_WithNonHttpScheme_ReturnsPhotoUrlInvalidError(string photoUrl)
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = photoUrl
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("PhotoUrl"));
        Assert.Contains("Photo URL must be a valid HTTP or HTTPS URL", result.Errors["PhotoUrl"]);
    }

    [Fact(DisplayName = "PhotoUrl validation can return multiple errors")]
    public void ValidateCreatePlayer_WithLongInvalidUrl_ReturnsBothPhotoUrlErrors()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = new string('a', 501)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.ContainsKey("PhotoUrl"));
        Assert.Equal(2, result.Errors["PhotoUrl"].Length);
        Assert.Contains("Photo URL must not exceed 500 characters", result.Errors["PhotoUrl"]);
        Assert.Contains("Photo URL must be a valid HTTP or HTTPS URL", result.Errors["PhotoUrl"]);
    }

    #endregion

    #region Multiple Errors Tests

    [Fact(DisplayName = "Validation returns all errors together")]
    public void ValidateCreatePlayer_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "",
            DateOfBirth = DateTime.UtcNow.Date.AddDays(1),
            Gender = "InvalidGender",
            PhotoUrl = "invalid-url"
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
        Assert.True(result.Errors.ContainsKey("Name"));
        Assert.True(result.Errors.ContainsKey("DateOfBirth"));
        Assert.True(result.Errors.ContainsKey("Gender"));
        Assert.True(result.Errors.ContainsKey("PhotoUrl"));
    }

    [Fact(DisplayName = "Validation collects all errors for a single field")]
    public void ValidateCreatePlayer_WithMultipleErrorsOnSameField_CollectsAllErrors()
    {
        var dto = new CreatePlayerDto
        {
            UserId = "user-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            PhotoUrl = "ftp://" + new string('a', 500)
        };

        var result = PlayerValidator.ValidateCreatePlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("PhotoUrl", out var photoUrlErrors));
        Assert.Equal(2, photoUrlErrors.Length);
    }

    #endregion

    #region Constants Tests

    [Fact(DisplayName = "MaxNameLength constant is 200")]
    public void MaxNameLength_IsCorrectValue()
    {
        Assert.Equal(200, PlayerValidator.MaxNameLength);
    }

    [Fact(DisplayName = "MaxPhotoUrlLength constant is 500")]
    public void MaxPhotoUrlLength_IsCorrectValue()
    {
        Assert.Equal(500, PlayerValidator.MaxPhotoUrlLength);
    }

    [Fact(DisplayName = "MaxAgeYears constant is 100")]
    public void MaxAgeYears_IsCorrectValue()
    {
        Assert.Equal(100, PlayerValidator.MaxAgeYears);
    }

    [Fact(DisplayName = "ValidGenderOptions contains all expected options")]
    public void ValidGenderOptions_ContainsExpectedOptions()
    {
        var expectedOptions = new[] { "Male", "Female", "Non-binary", "Prefer not to say" };

        Assert.Equal(expectedOptions.Length, PlayerValidator.ValidGenderOptions.Length);
        foreach (var option in expectedOptions)
        {
            Assert.Contains(option, PlayerValidator.ValidGenderOptions);
        }
    }

    #endregion
}
