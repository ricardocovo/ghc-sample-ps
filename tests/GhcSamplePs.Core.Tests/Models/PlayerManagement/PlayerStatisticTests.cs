using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.PlayerManagement;

public class PlayerStatisticTests
{
    #region Constructor and Property Tests

    [Fact(DisplayName = "PlayerStatistic can be created with required properties")]
    public void Constructor_WithRequiredProperties_CreatesSuccessfully()
    {
        var statistic = new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1,
            CreatedBy = "test-user"
        };

        Assert.NotNull(statistic);
        Assert.Equal(1, statistic.TeamPlayerId);
        Assert.Equal(new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc), statistic.GameDate);
        Assert.Equal(90, statistic.MinutesPlayed);
        Assert.True(statistic.IsStarter);
        Assert.Equal(10, statistic.JerseyNumber);
        Assert.Equal(2, statistic.Goals);
        Assert.Equal(1, statistic.Assists);
        Assert.Equal("test-user", statistic.CreatedBy);
    }

    [Fact(DisplayName = "PlayerStatistic has default PlayerStatisticId of zero")]
    public void DefaultPlayerStatisticId_WhenNotSet_IsZero()
    {
        var statistic = TestPlayerStatisticFactory.CreateMinimalPlayerStatistic();

        Assert.Equal(0, statistic.PlayerStatisticId);
    }

    [Fact(DisplayName = "PlayerStatistic CreatedAt defaults to current UTC time")]
    public void DefaultCreatedAt_WhenNotSet_IsCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var statistic = TestPlayerStatisticFactory.CreateMinimalPlayerStatistic();
        var afterCreation = DateTime.UtcNow;

        Assert.True(statistic.CreatedAt >= beforeCreation);
        Assert.True(statistic.CreatedAt <= afterCreation);
    }

    [Fact(DisplayName = "PlayerStatistic optional properties can be null")]
    public void Constructor_WithoutOptionalProperties_HasNullValues()
    {
        var statistic = TestPlayerStatisticFactory.CreateMinimalPlayerStatistic();

        Assert.Null(statistic.UpdatedAt);
        Assert.Null(statistic.UpdatedBy);
        Assert.Null(statistic.TeamPlayer);
    }

    [Fact(DisplayName = "PlayerStatistic TeamPlayer navigation property can be set")]
    public void TeamPlayer_WhenSet_ReturnsSetValue()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();
        var teamPlayer = TestPlayerStatisticFactory.CreateValidTeamPlayerForStatistic();

        statistic.TeamPlayer = teamPlayer;

        Assert.NotNull(statistic.TeamPlayer);
        Assert.Equal(teamPlayer.TeamPlayerId, statistic.TeamPlayer.TeamPlayerId);
    }

    #endregion

    #region Validate Tests

    [Fact(DisplayName = "Validate returns true for valid player statistic")]
    public void Validate_WithValidPlayerStatistic_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns true for player statistic with minimal required properties")]
    public void Validate_WithMinimalRequiredProperties_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateMinimalPlayerStatistic();

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when TeamPlayerId is zero")]
    public void Validate_WhenTeamPlayerIdIsZero_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithInvalidTeamPlayerId();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when TeamPlayerId is negative")]
    public void Validate_WhenTeamPlayerIdIsNegative_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(teamPlayerId: -1);

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when GameDate is default")]
    public void Validate_WhenGameDateIsDefault_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithDefaultGameDate();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when MinutesPlayed is negative")]
    public void Validate_WhenMinutesPlayedIsNegative_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithNegativeMinutesPlayed();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when MinutesPlayed is zero")]
    public void Validate_WhenMinutesPlayedIsZero_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(minutesPlayed: 0);

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when JerseyNumber is zero")]
    public void Validate_WhenJerseyNumberIsZero_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithInvalidJerseyNumber();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when JerseyNumber is negative")]
    public void Validate_WhenJerseyNumberIsNegative_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(jerseyNumber: -1);

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when JerseyNumber is one")]
    public void Validate_WhenJerseyNumberIsOne_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(jerseyNumber: 1);

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when Goals is negative")]
    public void Validate_WhenGoalsIsNegative_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithNegativeGoals();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when Goals is zero")]
    public void Validate_WhenGoalsIsZero_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(goals: 0);

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when Assists is negative")]
    public void Validate_WhenAssistsIsNegative_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithNegativeAssists();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when Assists is zero")]
    public void Validate_WhenAssistsIsZero_ReturnsTrue()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(assists: 0);

        var result = statistic.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is empty")]
    public void Validate_WhenCreatedByIsEmpty_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreatePlayerStatisticWithEmptyCreatedBy();

        var result = statistic.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is whitespace")]
    public void Validate_WhenCreatedByIsWhitespace_ReturnsFalse()
    {
        var statistic = TestPlayerStatisticFactory.CreateCustomPlayerStatistic(createdBy: "   ");

        var result = statistic.Validate();

        Assert.False(result);
    }

    #endregion

    #region UpdateLastModified Tests

    [Fact(DisplayName = "UpdateLastModified sets UpdatedAt to current UTC time")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedAtToCurrentUtcTime()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();
        var beforeUpdate = DateTime.UtcNow;

        statistic.UpdateLastModified("update-user");

        var afterUpdate = DateTime.UtcNow;
        Assert.NotNull(statistic.UpdatedAt);
        Assert.True(statistic.UpdatedAt >= beforeUpdate);
        Assert.True(statistic.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateLastModified sets UpdatedBy to provided userId")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedByToProvidedUserId()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        statistic.UpdateLastModified("update-user");

        Assert.Equal("update-user", statistic.UpdatedBy);
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is null")]
    public void UpdateLastModified_WhenUserIdIsNull_ThrowsArgumentException()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        Assert.Throws<ArgumentException>(() => statistic.UpdateLastModified(null!));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is empty")]
    public void UpdateLastModified_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        Assert.Throws<ArgumentException>(() => statistic.UpdateLastModified(""));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is whitespace")]
    public void UpdateLastModified_WhenUserIdIsWhitespace_ThrowsArgumentException()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        Assert.Throws<ArgumentException>(() => statistic.UpdateLastModified("   "));
    }

    [Fact(DisplayName = "UpdateLastModified can be called multiple times")]
    public void UpdateLastModified_CalledMultipleTimes_UpdatesEachTime()
    {
        var statistic = TestPlayerStatisticFactory.CreateValidPlayerStatistic();

        statistic.UpdateLastModified("first-user");
        var firstUpdate = statistic.UpdatedAt;
        var firstUser = statistic.UpdatedBy;

        System.Threading.Thread.Sleep(10);

        statistic.UpdateLastModified("second-user");

        Assert.True(statistic.UpdatedAt > firstUpdate);
        Assert.Equal("second-user", statistic.UpdatedBy);
        Assert.NotEqual(firstUser, statistic.UpdatedBy);
    }

    #endregion

    #region CalculateTotalMinutes Tests

    [Fact(DisplayName = "CalculateTotalMinutes returns correct sum for list of statistics")]
    public void CalculateTotalMinutes_WithValidList_ReturnsCorrectSum()
    {
        var statistics = TestPlayerStatisticFactory.CreatePlayerStatisticList();

        var result = PlayerStatistic.CalculateTotalMinutes(statistics);

        Assert.Equal(195, result); // 90 + 45 + 60
    }

    [Fact(DisplayName = "CalculateTotalMinutes returns zero for empty list")]
    public void CalculateTotalMinutes_WithEmptyList_ReturnsZero()
    {
        var statistics = new List<PlayerStatistic>();

        var result = PlayerStatistic.CalculateTotalMinutes(statistics);

        Assert.Equal(0, result);
    }

    [Fact(DisplayName = "CalculateTotalMinutes throws when statistics is null")]
    public void CalculateTotalMinutes_WhenStatisticsIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatistic.CalculateTotalMinutes(null!));
    }

    #endregion

    #region CalculateTotalGoals Tests

    [Fact(DisplayName = "CalculateTotalGoals returns correct sum for list of statistics")]
    public void CalculateTotalGoals_WithValidList_ReturnsCorrectSum()
    {
        var statistics = TestPlayerStatisticFactory.CreatePlayerStatisticList();

        var result = PlayerStatistic.CalculateTotalGoals(statistics);

        Assert.Equal(3, result); // 2 + 1 + 0
    }

    [Fact(DisplayName = "CalculateTotalGoals returns zero for empty list")]
    public void CalculateTotalGoals_WithEmptyList_ReturnsZero()
    {
        var statistics = new List<PlayerStatistic>();

        var result = PlayerStatistic.CalculateTotalGoals(statistics);

        Assert.Equal(0, result);
    }

    [Fact(DisplayName = "CalculateTotalGoals throws when statistics is null")]
    public void CalculateTotalGoals_WhenStatisticsIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatistic.CalculateTotalGoals(null!));
    }

    #endregion

    #region CalculateTotalAssists Tests

    [Fact(DisplayName = "CalculateTotalAssists returns correct sum for list of statistics")]
    public void CalculateTotalAssists_WithValidList_ReturnsCorrectSum()
    {
        var statistics = TestPlayerStatisticFactory.CreatePlayerStatisticList();

        var result = PlayerStatistic.CalculateTotalAssists(statistics);

        Assert.Equal(6, result); // 1 + 2 + 3
    }

    [Fact(DisplayName = "CalculateTotalAssists returns zero for empty list")]
    public void CalculateTotalAssists_WithEmptyList_ReturnsZero()
    {
        var statistics = new List<PlayerStatistic>();

        var result = PlayerStatistic.CalculateTotalAssists(statistics);

        Assert.Equal(0, result);
    }

    [Fact(DisplayName = "CalculateTotalAssists throws when statistics is null")]
    public void CalculateTotalAssists_WhenStatisticsIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PlayerStatistic.CalculateTotalAssists(null!));
    }

    #endregion
}
