using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Tests.TestHelpers;

/// <summary>
/// Test helper class for creating Player instances for testing.
/// </summary>
public static class TestPlayerFactory
{
    /// <summary>
    /// Creates a valid test player with default values.
    /// </summary>
    /// <returns>A valid Player instance.</returns>
    public static Player CreateValidPlayer()
    {
        return new Player
        {
            Id = 1,
            Name = "John Doe",
            DateOfBirth = new DateTime(1990, 6, 15),
            Gender = "Male",
            PhotoUrl = "https://example.com/photos/johndoe.jpg",
            CreatedBy = "test-user",
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
    }

    /// <summary>
    /// Creates a player with a specific date of birth for age testing.
    /// </summary>
    /// <param name="dateOfBirth">The date of birth to use.</param>
    /// <returns>A Player instance with the specified date of birth.</returns>
    public static Player CreatePlayerWithDateOfBirth(DateTime dateOfBirth)
    {
        return new Player
        {
            Id = 1,
            Name = "Test Player",
            DateOfBirth = dateOfBirth,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player born on a leap year birthday (February 29).
    /// </summary>
    /// <param name="year">The leap year to use for the birthday.</param>
    /// <returns>A Player instance with a Feb 29 birthday.</returns>
    public static Player CreateLeapYearPlayer(int year = 2000)
    {
        return new Player
        {
            Id = 1,
            Name = "Leap Year Player",
            DateOfBirth = new DateTime(year, 2, 29),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player with minimal required properties.
    /// </summary>
    /// <returns>A Player instance with only required properties.</returns>
    public static Player CreateMinimalPlayer()
    {
        return new Player
        {
            Name = "Minimal Player",
            DateOfBirth = new DateTime(1985, 1, 1),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player with custom properties for specific test scenarios.
    /// </summary>
    /// <param name="name">Player name.</param>
    /// <param name="dateOfBirth">Date of birth.</param>
    /// <param name="gender">Gender (optional).</param>
    /// <param name="photoUrl">Photo URL (optional).</param>
    /// <param name="createdBy">Created by user ID.</param>
    /// <returns>A customized Player instance.</returns>
    public static Player CreateCustomPlayer(
        string name,
        DateTime dateOfBirth,
        string? gender = null,
        string? photoUrl = null,
        string createdBy = "test-user")
    {
        return new Player
        {
            Name = name,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            PhotoUrl = photoUrl,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Creates a player with an invalid (future) date of birth.
    /// </summary>
    /// <returns>A Player instance with an invalid date of birth.</returns>
    public static Player CreatePlayerWithFutureDateOfBirth()
    {
        return new Player
        {
            Name = "Future Player",
            DateOfBirth = DateTime.UtcNow.AddYears(1),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player with a name that exceeds the maximum length.
    /// </summary>
    /// <returns>A Player instance with an invalid name length.</returns>
    public static Player CreatePlayerWithLongName()
    {
        return new Player
        {
            Name = new string('A', 201),
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player with an invalid photo URL.
    /// </summary>
    /// <returns>A Player instance with an invalid photo URL.</returns>
    public static Player CreatePlayerWithInvalidPhotoUrl()
    {
        return new Player
        {
            Name = "Test Player",
            DateOfBirth = new DateTime(1990, 1, 1),
            PhotoUrl = "not-a-valid-url",
            CreatedBy = "test-user"
        };
    }
}
