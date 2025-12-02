using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Validation;

namespace GhcSamplePs.Core.Tests.Validation;

public class PlayerStatisticValidatorTests
{
    #region ValidateCreatePlayerStatistic Tests

    [Fact(DisplayName = "ValidateCreatePlayerStatistic returns valid for valid CreatePlayerStatisticDto")]
    public void ValidateCreatePlayerStatistic_WithValidDto_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateCreatePlayerStatistic throws when dto is null")]
    public void ValidateCreatePlayerStatistic_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatisticValidator.ValidateCreatePlayerStatistic(null!));
    }

    #endregion

    #region ValidateUpdatePlayerStatistic Tests

    [Fact(DisplayName = "ValidateUpdatePlayerStatistic returns valid for valid UpdatePlayerStatisticDto")]
    public void ValidateUpdatePlayerStatistic_WithValidDto_ReturnsValid()
    {
        var dto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 3,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateUpdatePlayerStatistic(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateUpdatePlayerStatistic throws when dto is null")]
    public void ValidateUpdatePlayerStatistic_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatisticValidator.ValidateUpdatePlayerStatistic(null!));
    }

    #endregion

    #region ValidatePlayerStatistic Tests

    [Fact(DisplayName = "ValidatePlayerStatistic returns valid for valid PlayerStatistic entity")]
    public void ValidatePlayerStatistic_WithValidPlayerStatistic_ReturnsValid()
    {
        var playerStatistic = new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1,
            CreatedBy = "system"
        };

        var result = PlayerStatisticValidator.ValidatePlayerStatistic(playerStatistic);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidatePlayerStatistic throws when playerStatistic is null")]
    public void ValidatePlayerStatistic_WhenPlayerStatisticIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatisticValidator.ValidatePlayerStatistic(null!));
    }

    #endregion

    #region TeamPlayerId Validation Tests

    [Fact(DisplayName = "TeamPlayerId validation returns error when TeamPlayerId is zero")]
    public void ValidateCreatePlayerStatistic_WhenTeamPlayerIdIsZero_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 0,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamPlayerId", out var teamPlayerIdErrors));
        Assert.Contains("Team player ID must be greater than 0", teamPlayerIdErrors);
    }

    [Fact(DisplayName = "TeamPlayerId validation returns error when TeamPlayerId is negative")]
    public void ValidateCreatePlayerStatistic_WhenTeamPlayerIdIsNegative_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = -1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamPlayerId", out var teamPlayerIdErrors));
        Assert.Contains("Team player ID must be greater than 0", teamPlayerIdErrors);
    }

    [Fact(DisplayName = "TeamPlayerId validation succeeds when TeamPlayerId is positive")]
    public void ValidateCreatePlayerStatistic_WhenTeamPlayerIdIsPositive_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region GameDate Validation Tests

    [Fact(DisplayName = "GameDate validation returns error when GameDate is default")]
    public void ValidateCreatePlayerStatistic_WhenGameDateIsDefault_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = default,
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("GameDate", out var gameDateErrors));
        Assert.Contains("Game date is required", gameDateErrors);
    }

    [Fact(DisplayName = "GameDate validation returns error when GameDate is in the future")]
    public void ValidateCreatePlayerStatistic_WhenGameDateIsFuture_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("GameDate", out var gameDateErrors));
        Assert.Contains("Game date cannot be in the future", gameDateErrors);
    }

    [Fact(DisplayName = "GameDate validation succeeds when GameDate is today")]
    public void ValidateCreatePlayerStatistic_WhenGameDateIsToday_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date,
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "GameDate validation succeeds when GameDate is in the past")]
    public void ValidateCreatePlayerStatistic_WhenGameDateIsInPast_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-365),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region MinutesPlayed Validation Tests

    [Fact(DisplayName = "MinutesPlayed validation returns error when MinutesPlayed is negative")]
    public void ValidateCreatePlayerStatistic_WhenMinutesPlayedIsNegative_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = -1,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("MinutesPlayed", out var minutesPlayedErrors));
        Assert.Contains("Minutes played must be non-negative", minutesPlayedErrors);
    }

    [Fact(DisplayName = "MinutesPlayed validation returns error when MinutesPlayed exceeds 120")]
    public void ValidateCreatePlayerStatistic_WhenMinutesPlayedExceeds120_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 121,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("MinutesPlayed", out var minutesPlayedErrors));
        Assert.Contains("Minutes played should not exceed 120", minutesPlayedErrors);
    }

    [Fact(DisplayName = "MinutesPlayed validation succeeds when MinutesPlayed is zero")]
    public void ValidateCreatePlayerStatistic_WhenMinutesPlayedIsZero_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 0,
            IsStarter = false,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "MinutesPlayed validation succeeds when MinutesPlayed is exactly 120")]
    public void ValidateCreatePlayerStatistic_WhenMinutesPlayedIsExactly120_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 120,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region JerseyNumber Validation Tests

    [Fact(DisplayName = "JerseyNumber validation returns error when JerseyNumber is zero")]
    public void ValidateCreatePlayerStatistic_WhenJerseyNumberIsZero_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 0,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JerseyNumber", out var jerseyNumberErrors));
        Assert.Contains("Jersey number must be greater than 0", jerseyNumberErrors);
    }

    [Fact(DisplayName = "JerseyNumber validation returns error when JerseyNumber is negative")]
    public void ValidateCreatePlayerStatistic_WhenJerseyNumberIsNegative_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = -1,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JerseyNumber", out var jerseyNumberErrors));
        Assert.Contains("Jersey number must be greater than 0", jerseyNumberErrors);
    }

    [Fact(DisplayName = "JerseyNumber validation returns error when JerseyNumber exceeds 99")]
    public void ValidateCreatePlayerStatistic_WhenJerseyNumberExceeds99_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 100,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JerseyNumber", out var jerseyNumberErrors));
        Assert.Contains("Jersey number should not exceed 99", jerseyNumberErrors);
    }

    [Fact(DisplayName = "JerseyNumber validation succeeds when JerseyNumber is 1")]
    public void ValidateCreatePlayerStatistic_WhenJerseyNumberIsOne_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 1,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "JerseyNumber validation succeeds when JerseyNumber is exactly 99")]
    public void ValidateCreatePlayerStatistic_WhenJerseyNumberIsExactly99_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 99,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Goals Validation Tests

    [Fact(DisplayName = "Goals validation returns error when Goals is negative")]
    public void ValidateCreatePlayerStatistic_WhenGoalsIsNegative_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = -1,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("Goals", out var goalsErrors));
        Assert.Contains("Goals must be non-negative", goalsErrors);
    }

    [Fact(DisplayName = "Goals validation succeeds when Goals is zero")]
    public void ValidateCreatePlayerStatistic_WhenGoalsIsZero_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Goals validation succeeds when Goals is positive")]
    public void ValidateCreatePlayerStatistic_WhenGoalsIsPositive_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 5,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Assists Validation Tests

    [Fact(DisplayName = "Assists validation returns error when Assists is negative")]
    public void ValidateCreatePlayerStatistic_WhenAssistsIsNegative_ReturnsError()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = -1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("Assists", out var assistsErrors));
        Assert.Contains("Assists must be non-negative", assistsErrors);
    }

    [Fact(DisplayName = "Assists validation succeeds when Assists is zero")]
    public void ValidateCreatePlayerStatistic_WhenAssistsIsZero_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 0
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Assists validation succeeds when Assists is positive")]
    public void ValidateCreatePlayerStatistic_WhenAssistsIsPositive_ReturnsValid()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 3
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Multiple Errors Tests

    [Fact(DisplayName = "Validation returns all errors together")]
    public void ValidateCreatePlayerStatistic_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 0,
            GameDate = DateTime.UtcNow.Date.AddDays(1),
            MinutesPlayed = -1,
            IsStarter = true,
            JerseyNumber = 0,
            Goals = -1,
            Assists = -1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.False(result.IsValid);
        Assert.Equal(6, result.Errors.Count);
        Assert.True(result.Errors.ContainsKey("TeamPlayerId"));
        Assert.True(result.Errors.ContainsKey("GameDate"));
        Assert.True(result.Errors.ContainsKey("MinutesPlayed"));
        Assert.True(result.Errors.ContainsKey("JerseyNumber"));
        Assert.True(result.Errors.ContainsKey("Goals"));
        Assert.True(result.Errors.ContainsKey("Assists"));
    }

    [Fact(DisplayName = "All fields valid passes validation")]
    public void ValidateCreatePlayerStatistic_AllFieldsValid_PassesValidation()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 123,
            GameDate = DateTime.UtcNow.Date,
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.ValidateCreatePlayerStatistic(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Constants Tests

    [Fact(DisplayName = "MaxMinutesPlayed constant is 120")]
    public void MaxMinutesPlayed_IsCorrectValue()
    {
        Assert.Equal(120, PlayerStatisticValidator.MaxMinutesPlayed);
    }

    [Fact(DisplayName = "MaxJerseyNumber constant is 99")]
    public void MaxJerseyNumber_IsCorrectValue()
    {
        Assert.Equal(99, PlayerStatisticValidator.MaxJerseyNumber);
    }

    #endregion

    #region Validate Alias Tests

    [Fact(DisplayName = "Validate alias for CreatePlayerStatisticDto works correctly")]
    public void Validate_WithCreateDto_CallsValidateCreatePlayerStatistic()
    {
        var dto = new CreatePlayerStatisticDto
        {
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1
        };

        var result = PlayerStatisticValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate alias for UpdatePlayerStatisticDto works correctly")]
    public void Validate_WithUpdateDto_CallsValidateUpdatePlayerStatistic()
    {
        var dto = new UpdatePlayerStatisticDto
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = DateTime.UtcNow.Date.AddDays(-1),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 3,
            Assists = 1
        };

        var result = PlayerStatisticValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    #endregion
}
