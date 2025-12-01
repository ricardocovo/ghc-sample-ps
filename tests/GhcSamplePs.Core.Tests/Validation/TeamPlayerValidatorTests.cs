using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;
using GhcSamplePs.Core.Validation;

namespace GhcSamplePs.Core.Tests.Validation;

public class TeamPlayerValidatorTests
{
    #region ValidateCreateTeamPlayer Tests

    [Fact(DisplayName = "ValidateCreateTeamPlayer returns valid for valid CreateTeamPlayerDto")]
    public void ValidateCreateTeamPlayer_WithValidDto_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateCreateTeamPlayer throws when dto is null")]
    public void ValidateCreateTeamPlayer_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TeamPlayerValidator.ValidateCreateTeamPlayer(null!));
    }

    #endregion

    #region ValidateUpdateTeamPlayer Tests

    [Fact(DisplayName = "ValidateUpdateTeamPlayer returns valid for valid UpdateTeamPlayerDto")]
    public void ValidateUpdateTeamPlayer_WithValidDto_ReturnsValid()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Beta",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-60),
            LeftDate = DateTime.UtcNow.Date.AddDays(-10)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateUpdateTeamPlayer returns valid with null LeftDate")]
    public void ValidateUpdateTeamPlayer_WithNullLeftDate_ReturnsValid()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Beta",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = null
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateUpdateTeamPlayer throws when dto is null")]
    public void ValidateUpdateTeamPlayer_WhenDtoIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TeamPlayerValidator.ValidateUpdateTeamPlayer(null!));
    }

    #endregion

    #region ValidateTeamPlayer Tests

    [Fact(DisplayName = "ValidateTeamPlayer returns valid for valid TeamPlayer entity")]
    public void ValidateTeamPlayer_WithValidTeamPlayer_ReturnsValid()
    {
        var teamPlayer = new TeamPlayer
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            CreatedBy = "system"
        };

        var result = TeamPlayerValidator.ValidateTeamPlayer(teamPlayer);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "ValidateTeamPlayer throws when teamPlayer is null")]
    public void ValidateTeamPlayer_WhenTeamPlayerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => TeamPlayerValidator.ValidateTeamPlayer(null!));
    }

    #endregion

    #region PlayerId Validation Tests

    [Fact(DisplayName = "PlayerId validation returns error when PlayerId is zero")]
    public void ValidateCreateTeamPlayer_WhenPlayerIdIsZero_ReturnsPlayerIdError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 0,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("PlayerId", out var playerIdErrors));
        Assert.Contains("Player ID must be a positive integer", playerIdErrors);
    }

    [Fact(DisplayName = "PlayerId validation returns error when PlayerId is negative")]
    public void ValidateCreateTeamPlayer_WhenPlayerIdIsNegative_ReturnsPlayerIdError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = -5,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("PlayerId", out var playerIdErrors));
        Assert.Contains("Player ID must be a positive integer", playerIdErrors);
    }

    [Fact(DisplayName = "PlayerId validation succeeds when PlayerId is positive")]
    public void ValidateCreateTeamPlayer_WhenPlayerIdIsPositive_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region TeamName Validation Tests

    [Fact(DisplayName = "TeamName validation returns error when TeamName is null")]
    public void ValidateCreateTeamPlayer_WhenTeamNameIsNull_ReturnsTeamNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = null!,
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamName", out var teamNameErrors));
        Assert.Contains("Team name is required", teamNameErrors);
    }

    [Fact(DisplayName = "TeamName validation returns error when TeamName is empty")]
    public void ValidateCreateTeamPlayer_WhenTeamNameIsEmpty_ReturnsTeamNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamName", out var teamNameErrors));
        Assert.Contains("Team name is required", teamNameErrors);
    }

    [Fact(DisplayName = "TeamName validation returns error when TeamName is whitespace only")]
    public void ValidateCreateTeamPlayer_WhenTeamNameIsWhitespace_ReturnsTeamNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "   ",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamName", out var teamNameErrors));
        Assert.Contains("Team name is required", teamNameErrors);
    }

    [Fact(DisplayName = "TeamName validation returns error when TeamName exceeds 200 characters")]
    public void ValidateCreateTeamPlayer_WhenTeamNameExceeds200Characters_ReturnsTeamNameTooLongError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = new string('A', 201),
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("TeamName", out var teamNameErrors));
        Assert.Contains("Team name must not exceed 200 characters", teamNameErrors);
    }

    [Fact(DisplayName = "TeamName validation succeeds when TeamName is exactly 200 characters")]
    public void ValidateCreateTeamPlayer_WhenTeamNameIsExactly200Characters_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = new string('A', 200),
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "TeamName validation succeeds when TeamName is 1 character")]
    public void ValidateCreateTeamPlayer_WhenTeamNameIsOneCharacter_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "A",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "TeamName validation trims whitespace before checking length")]
    public void ValidateCreateTeamPlayer_TeamNameWithPaddingWhitespace_TrimsBeforeValidation()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "  " + new string('A', 200) + "  ",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region ChampionshipName Validation Tests

    [Fact(DisplayName = "ChampionshipName validation returns error when ChampionshipName is null")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameIsNull_ReturnsChampionshipNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = null!,
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("ChampionshipName", out var championshipNameErrors));
        Assert.Contains("Championship name is required", championshipNameErrors);
    }

    [Fact(DisplayName = "ChampionshipName validation returns error when ChampionshipName is empty")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameIsEmpty_ReturnsChampionshipNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("ChampionshipName", out var championshipNameErrors));
        Assert.Contains("Championship name is required", championshipNameErrors);
    }

    [Fact(DisplayName = "ChampionshipName validation returns error when ChampionshipName is whitespace only")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameIsWhitespace_ReturnsChampionshipNameRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "   ",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("ChampionshipName", out var championshipNameErrors));
        Assert.Contains("Championship name is required", championshipNameErrors);
    }

    [Fact(DisplayName = "ChampionshipName validation returns error when ChampionshipName exceeds 200 characters")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameExceeds200Characters_ReturnsChampionshipNameTooLongError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = new string('B', 201),
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("ChampionshipName", out var championshipNameErrors));
        Assert.Contains("Championship name must not exceed 200 characters", championshipNameErrors);
    }

    [Fact(DisplayName = "ChampionshipName validation succeeds when ChampionshipName is exactly 200 characters")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameIsExactly200Characters_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = new string('B', 200),
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "ChampionshipName validation succeeds when ChampionshipName is 1 character")]
    public void ValidateCreateTeamPlayer_WhenChampionshipNameIsOneCharacter_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "B",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "ChampionshipName validation trims whitespace before checking length")]
    public void ValidateCreateTeamPlayer_ChampionshipNameWithPaddingWhitespace_TrimsBeforeValidation()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "  " + new string('B', 200) + "  ",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region JoinedDate Validation Tests

    [Fact(DisplayName = "JoinedDate validation returns error when JoinedDate is default")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsDefault_ReturnsJoinedDateRequiredError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = default
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JoinedDate", out var joinedDateErrors));
        Assert.Contains("Joined date is required", joinedDateErrors);
    }

    [Fact(DisplayName = "JoinedDate validation returns error when JoinedDate is more than 1 year in the future")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsMoreThan1YearFuture_ReturnsJoinedDateFutureLimitError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddYears(1).AddDays(1)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JoinedDate", out var joinedDateErrors));
        Assert.Contains("Joined date cannot be more than 1 year in the future", joinedDateErrors);
    }

    [Fact(DisplayName = "JoinedDate validation succeeds when JoinedDate is exactly 1 year in the future")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsExactly1YearFuture_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddYears(1)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "JoinedDate validation returns error when JoinedDate is more than 100 years in the past")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsMoreThan100YearsPast_ReturnsJoinedDatePastLimitError()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddYears(-101)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("JoinedDate", out var joinedDateErrors));
        Assert.Contains("Joined date cannot be more than 100 years in the past", joinedDateErrors);
    }

    [Fact(DisplayName = "JoinedDate validation succeeds when JoinedDate is exactly 100 years in the past")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsExactly100YearsPast_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddYears(-100)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "JoinedDate validation succeeds when JoinedDate is today")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsToday_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "JoinedDate validation succeeds when JoinedDate is in the past")]
    public void ValidateCreateTeamPlayer_WhenJoinedDateIsInPast_ReturnsValid()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-365)
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    #endregion

    #region LeftDate Validation Tests

    [Fact(DisplayName = "LeftDate validation succeeds when LeftDate is null")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsNull_ReturnsValid()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = null
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "LeftDate validation returns error when LeftDate is in the future")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsFuture_ReturnsLeftDateFutureError()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = DateTime.UtcNow.Date.AddDays(1)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("LeftDate", out var leftDateErrors));
        Assert.Contains("Left date cannot be in the future", leftDateErrors);
    }

    [Fact(DisplayName = "LeftDate validation returns error when LeftDate is before JoinedDate")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsBeforeJoinedDate_ReturnsLeftDateBeforeJoinedError()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = DateTime.UtcNow.Date.AddDays(-60)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("LeftDate", out var leftDateErrors));
        Assert.Contains("Left date must be after joined date", leftDateErrors);
    }

    [Fact(DisplayName = "LeftDate validation returns error when LeftDate is same as JoinedDate")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsSameAsJoinedDate_ReturnsLeftDateBeforeJoinedError()
    {
        var joinedDate = DateTime.UtcNow.Date.AddDays(-30);
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = joinedDate,
            LeftDate = joinedDate
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("LeftDate", out var leftDateErrors));
        Assert.Contains("Left date must be after joined date", leftDateErrors);
    }

    [Fact(DisplayName = "LeftDate validation succeeds when LeftDate is after JoinedDate and in the past")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsAfterJoinedDateAndInPast_ReturnsValid()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-60),
            LeftDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "LeftDate validation succeeds when LeftDate is today")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsToday_ReturnsValid()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = DateTime.UtcNow.Date
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "LeftDate validation can return multiple errors")]
    public void ValidateUpdateTeamPlayer_WhenLeftDateIsInFutureAndBeforeJoinedDate_ReturnsBothErrors()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(10),
            LeftDate = DateTime.UtcNow.Date.AddDays(5)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.TryGetValue("LeftDate", out var leftDateErrors));
        Assert.Equal(2, leftDateErrors.Length);
        Assert.Contains("Left date cannot be in the future", leftDateErrors);
        Assert.Contains("Left date must be after joined date", leftDateErrors);
    }

    #endregion

    #region Multiple Errors Tests

    [Fact(DisplayName = "Validation returns all errors together")]
    public void ValidateCreateTeamPlayer_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 0,
            TeamName = "",
            ChampionshipName = "",
            JoinedDate = default
        };

        var result = TeamPlayerValidator.ValidateCreateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
        Assert.True(result.Errors.ContainsKey("PlayerId"));
        Assert.True(result.Errors.ContainsKey("TeamName"));
        Assert.True(result.Errors.ContainsKey("ChampionshipName"));
        Assert.True(result.Errors.ContainsKey("JoinedDate"));
    }

    [Fact(DisplayName = "Validation collects all errors for UpdateTeamPlayer")]
    public void ValidateUpdateTeamPlayer_WithMultipleInvalidFields_ReturnsAllErrors()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "",
            ChampionshipName = "",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = DateTime.UtcNow.Date.AddDays(1)
        };

        var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(dto);

        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);
        Assert.True(result.Errors.ContainsKey("TeamName"));
        Assert.True(result.Errors.ContainsKey("ChampionshipName"));
        Assert.True(result.Errors.ContainsKey("LeftDate"));
    }

    #endregion

    #region Constants Tests

    [Fact(DisplayName = "MaxNameLength constant is 200")]
    public void MaxNameLength_IsCorrectValue()
    {
        Assert.Equal(200, TeamPlayerValidator.MaxNameLength);
    }

    [Fact(DisplayName = "MaxPastYears constant is 100")]
    public void MaxPastYears_IsCorrectValue()
    {
        Assert.Equal(100, TeamPlayerValidator.MaxPastYears);
    }

    [Fact(DisplayName = "MaxFutureYears constant is 1")]
    public void MaxFutureYears_IsCorrectValue()
    {
        Assert.Equal(1, TeamPlayerValidator.MaxFutureYears);
    }

    #endregion

    #region Alias Methods Tests

    [Fact(DisplayName = "Validate alias for CreateTeamPlayerDto works correctly")]
    public void Validate_CreateTeamPlayerDto_ReturnsCorrectResult()
    {
        var dto = new CreateTeamPlayerDto
        {
            PlayerId = 123,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30)
        };

        var result = TeamPlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    [Fact(DisplayName = "Validate alias for UpdateTeamPlayerDto works correctly")]
    public void Validate_UpdateTeamPlayerDto_ReturnsCorrectResult()
    {
        var dto = new UpdateTeamPlayerDto
        {
            TeamPlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = DateTime.UtcNow.Date.AddDays(-30),
            LeftDate = null
        };

        var result = TeamPlayerValidator.Validate(dto);

        Assert.True(result.IsValid);
    }

    #endregion
}
