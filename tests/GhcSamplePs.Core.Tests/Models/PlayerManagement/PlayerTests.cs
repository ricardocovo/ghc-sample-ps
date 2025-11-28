using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.PlayerManagement;

public class PlayerTests
{
    #region Constructor and Property Tests

    [Fact(DisplayName = "Player can be created with required properties")]
    public void Constructor_WithRequiredProperties_CreatesSuccessfully()
    {
        var player = new Player
        {
            UserId = "owner-123",
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            CreatedBy = "test-user"
        };

        Assert.NotNull(player);
        Assert.Equal("owner-123", player.UserId);
        Assert.Equal("John Doe", player.Name);
        Assert.Equal(new DateTime(1990, 6, 15), player.DateOfBirth);
        Assert.Equal("test-user", player.CreatedBy);
    }

    [Fact(DisplayName = "Player has default Id of zero")]
    public void DefaultId_WhenNotSet_IsZero()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();

        Assert.Equal(0, player.Id);
    }

    [Fact(DisplayName = "Player CreatedAt defaults to current UTC time")]
    public void DefaultCreatedAt_WhenNotSet_IsCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var player = TestPlayerFactory.CreateMinimalPlayer();
        var afterCreation = DateTime.UtcNow;

        Assert.True(player.CreatedAt >= beforeCreation);
        Assert.True(player.CreatedAt <= afterCreation);
    }

    [Fact(DisplayName = "Player optional properties can be null")]
    public void Constructor_WithoutOptionalProperties_HasNullValues()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();

        Assert.Null(player.Gender);
        Assert.Null(player.PhotoUrl);
        Assert.Null(player.UpdatedAt);
        Assert.Null(player.UpdatedBy);
    }

    #endregion

    #region CalculateAge Tests

    [Fact(DisplayName = "CalculateAge returns correct age when birthday has passed this year")]
    public void CalculateAge_WhenBirthdayHasPassedThisYear_ReturnsCorrectAge()
    {
        var today = DateTime.UtcNow.Date;
        var dateOfBirth = today.AddYears(-30).AddDays(-10);
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(dateOfBirth);

        var age = player.CalculateAge();

        Assert.Equal(30, age);
    }

    [Fact(DisplayName = "CalculateAge returns correct age when birthday has not passed this year")]
    public void CalculateAge_WhenBirthdayHasNotPassedThisYear_ReturnsCorrectAge()
    {
        var today = DateTime.UtcNow.Date;
        var dateOfBirth = today.AddYears(-30).AddDays(10);
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(dateOfBirth);

        var age = player.CalculateAge();

        Assert.Equal(29, age);
    }

    [Fact(DisplayName = "CalculateAge returns correct age on birthday")]
    public void CalculateAge_OnBirthday_ReturnsCorrectAge()
    {
        var today = DateTime.UtcNow.Date;
        var dateOfBirth = today.AddYears(-25);
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(dateOfBirth);

        var age = player.CalculateAge();

        Assert.Equal(25, age);
    }

    [Fact(DisplayName = "CalculateAge handles leap year birthday on non-leap year")]
    public void CalculateAge_LeapYearBirthdayOnNonLeapYear_ReturnsCorrectAge()
    {
        var player = TestPlayerFactory.CreateLeapYearPlayer(2000);
        var expectedAge = DateTime.UtcNow.Year - 2000;
        
        var birthdayThisYear = new DateTime(DateTime.UtcNow.Year, 3, 1);
        if (DateTime.UtcNow.Date < birthdayThisYear)
        {
            expectedAge--;
        }

        var age = player.CalculateAge();

        Assert.Equal(expectedAge, age);
    }

    [Fact(DisplayName = "CalculateAge handles very old birth dates")]
    public void CalculateAge_VeryOldDateOfBirth_ReturnsCorrectAge()
    {
        var dateOfBirth = new DateTime(1920, 1, 1);
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(dateOfBirth);
        var expectedAge = DateTime.UtcNow.Year - 1920;
        
        var birthdayThisYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
        if (DateTime.UtcNow.Date < birthdayThisYear)
        {
            expectedAge--;
        }

        var age = player.CalculateAge();

        Assert.Equal(expectedAge, age);
    }

    [Fact(DisplayName = "Age property returns calculated age")]
    public void Age_WhenAccessed_ReturnsCalculatedAge()
    {
        var today = DateTime.UtcNow.Date;
        var dateOfBirth = today.AddYears(-35).AddDays(-1);
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(dateOfBirth);

        var age = player.Age;

        Assert.Equal(35, age);
    }

    #endregion

    #region Validate Tests

    [Fact(DisplayName = "Validate returns true for valid player")]
    public void Validate_WithValidPlayer_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns true for player with minimal required properties")]
    public void Validate_WithMinimalRequiredProperties_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateMinimalPlayer();

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when UserId is empty")]
    public void Validate_WhenUserIdIsEmpty_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            userId: "");

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when UserId is whitespace")]
    public void Validate_WhenUserIdIsWhitespace_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            userId: "   ");

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when name is empty")]
    public void Validate_WhenNameIsEmpty_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "",
            dateOfBirth: new DateTime(1990, 1, 1));

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when name is whitespace")]
    public void Validate_WhenNameIsWhitespace_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "   ",
            dateOfBirth: new DateTime(1990, 1, 1));

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when name exceeds 200 characters")]
    public void Validate_WhenNameExceeds200Characters_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreatePlayerWithLongName();

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when name is exactly 200 characters")]
    public void Validate_WhenNameIsExactly200Characters_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: new string('A', 200),
            dateOfBirth: new DateTime(1990, 1, 1));

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when DateOfBirth is today")]
    public void Validate_WhenDateOfBirthIsToday_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreatePlayerWithDateOfBirth(DateTime.UtcNow.Date);

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when DateOfBirth is in the future")]
    public void Validate_WhenDateOfBirthIsInFuture_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreatePlayerWithFutureDateOfBirth();

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when Gender exceeds 50 characters")]
    public void Validate_WhenGenderExceeds50Characters_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            gender: new string('A', 51));

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when Gender is exactly 50 characters")]
    public void Validate_WhenGenderIsExactly50Characters_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            gender: new string('A', 50));

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when PhotoUrl exceeds 500 characters")]
    public void Validate_WhenPhotoUrlExceeds500Characters_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            photoUrl: "https://example.com/" + new string('a', 490));

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when PhotoUrl is not a valid URL")]
    public void Validate_WhenPhotoUrlIsNotValidUrl_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreatePlayerWithInvalidPhotoUrl();

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when PhotoUrl has invalid scheme")]
    public void Validate_WhenPhotoUrlHasInvalidScheme_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            photoUrl: "ftp://example.com/photo.jpg");

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when PhotoUrl uses HTTPS")]
    public void Validate_WhenPhotoUrlUsesHttps_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            photoUrl: "https://example.com/photo.jpg");

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns true when PhotoUrl uses HTTP")]
    public void Validate_WhenPhotoUrlUsesHttp_ReturnsTrue()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            photoUrl: "http://example.com/photo.jpg");

        var result = player.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is empty")]
    public void Validate_WhenCreatedByIsEmpty_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            createdBy: "");

        var result = player.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is whitespace")]
    public void Validate_WhenCreatedByIsWhitespace_ReturnsFalse()
    {
        var player = TestPlayerFactory.CreateCustomPlayer(
            name: "Test Player",
            dateOfBirth: new DateTime(1990, 1, 1),
            createdBy: "   ");

        var result = player.Validate();

        Assert.False(result);
    }

    #endregion

    #region UpdateLastModified Tests

    [Fact(DisplayName = "UpdateLastModified sets UpdatedAt to current UTC time")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedAtToCurrentUtcTime()
    {
        var player = TestPlayerFactory.CreateValidPlayer();
        var beforeUpdate = DateTime.UtcNow;

        player.UpdateLastModified("update-user");

        var afterUpdate = DateTime.UtcNow;
        Assert.NotNull(player.UpdatedAt);
        Assert.True(player.UpdatedAt >= beforeUpdate);
        Assert.True(player.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateLastModified sets UpdatedBy to provided userId")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedByToProvidedUserId()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        player.UpdateLastModified("update-user");

        Assert.Equal("update-user", player.UpdatedBy);
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is null")]
    public void UpdateLastModified_WhenUserIdIsNull_ThrowsArgumentException()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        Assert.Throws<ArgumentException>(() => player.UpdateLastModified(null!));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is empty")]
    public void UpdateLastModified_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        Assert.Throws<ArgumentException>(() => player.UpdateLastModified(""));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is whitespace")]
    public void UpdateLastModified_WhenUserIdIsWhitespace_ThrowsArgumentException()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        Assert.Throws<ArgumentException>(() => player.UpdateLastModified("   "));
    }

    [Fact(DisplayName = "UpdateLastModified can be called multiple times")]
    public void UpdateLastModified_CalledMultipleTimes_UpdatesEachTime()
    {
        var player = TestPlayerFactory.CreateValidPlayer();

        player.UpdateLastModified("first-user");
        var firstUpdate = player.UpdatedAt;
        var firstUser = player.UpdatedBy;

        System.Threading.Thread.Sleep(10);

        player.UpdateLastModified("second-user");

        Assert.True(player.UpdatedAt > firstUpdate);
        Assert.Equal("second-user", player.UpdatedBy);
        Assert.NotEqual(firstUser, player.UpdatedBy);
    }

    #endregion
}
