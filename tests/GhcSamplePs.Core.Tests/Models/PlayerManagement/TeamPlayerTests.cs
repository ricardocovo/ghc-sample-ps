using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Tests.TestHelpers;

namespace GhcSamplePs.Core.Tests.Models.PlayerManagement;

public class TeamPlayerTests
{
    #region Constructor and Property Tests

    [Fact(DisplayName = "TeamPlayer can be created with required properties")]
    public void Constructor_WithRequiredProperties_CreatesSuccessfully()
    {
        var teamPlayer = new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };

        Assert.NotNull(teamPlayer);
        Assert.Equal(1, teamPlayer.PlayerId);
        Assert.Equal("Team Alpha", teamPlayer.TeamName);
        Assert.Equal("Championship 2024", teamPlayer.ChampionshipName);
        Assert.Equal(new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), teamPlayer.JoinedDate);
        Assert.Equal("test-user", teamPlayer.CreatedBy);
    }

    [Fact(DisplayName = "TeamPlayer has default TeamPlayerId of zero")]
    public void DefaultTeamPlayerId_WhenNotSet_IsZero()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateMinimalTeamPlayer();

        Assert.Equal(0, teamPlayer.TeamPlayerId);
    }

    [Fact(DisplayName = "TeamPlayer CreatedAt defaults to current UTC time")]
    public void DefaultCreatedAt_WhenNotSet_IsCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var teamPlayer = TestTeamPlayerFactory.CreateMinimalTeamPlayer();
        var afterCreation = DateTime.UtcNow;

        Assert.True(teamPlayer.CreatedAt >= beforeCreation);
        Assert.True(teamPlayer.CreatedAt <= afterCreation);
    }

    [Fact(DisplayName = "TeamPlayer optional properties can be null")]
    public void Constructor_WithoutOptionalProperties_HasNullValues()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateMinimalTeamPlayer();

        Assert.Null(teamPlayer.LeftDate);
        Assert.Null(teamPlayer.UpdatedAt);
        Assert.Null(teamPlayer.UpdatedBy);
        Assert.Null(teamPlayer.Player);
    }

    [Fact(DisplayName = "TeamPlayer IsActive returns true when LeftDate is null")]
    public void IsActive_WhenLeftDateIsNull_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.True(teamPlayer.IsActive);
    }

    [Fact(DisplayName = "TeamPlayer IsActive returns false when LeftDate is set")]
    public void IsActive_WhenLeftDateIsSet_ReturnsFalse()
    {
        var joinedDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);
        var leftDate = new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc);
        var teamPlayer = TestTeamPlayerFactory.CreateInactiveTeamPlayer(joinedDate, leftDate);

        Assert.False(teamPlayer.IsActive);
    }

    [Fact(DisplayName = "TeamPlayer Player navigation property can be set")]
    public void Player_WhenSet_ReturnsSetValue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var player = TestTeamPlayerFactory.CreateValidPlayerForTeamPlayer();

        teamPlayer.Player = player;

        Assert.NotNull(teamPlayer.Player);
        Assert.Equal(player.Id, teamPlayer.Player.Id);
    }

    #endregion

    #region Validate Tests

    [Fact(DisplayName = "Validate returns true for valid team player")]
    public void Validate_WithValidTeamPlayer_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        var result = teamPlayer.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns true for team player with minimal required properties")]
    public void Validate_WithMinimalRequiredProperties_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateMinimalTeamPlayer();

        var result = teamPlayer.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when TeamName is empty")]
    public void Validate_WhenTeamNameIsEmpty_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithEmptyTeamName();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when TeamName is whitespace")]
    public void Validate_WhenTeamNameIsWhitespace_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "   ",
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when TeamName exceeds 200 characters")]
    public void Validate_WhenTeamNameExceeds200Characters_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithLongTeamName();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when TeamName is exactly 200 characters")]
    public void Validate_WhenTeamNameIsExactly200Characters_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: new string('A', 200),
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when ChampionshipName is empty")]
    public void Validate_WhenChampionshipNameIsEmpty_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithEmptyChampionshipName();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when ChampionshipName is whitespace")]
    public void Validate_WhenChampionshipNameIsWhitespace_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: "   ",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when ChampionshipName exceeds 200 characters")]
    public void Validate_WhenChampionshipNameExceeds200Characters_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithLongChampionshipName();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when ChampionshipName is exactly 200 characters")]
    public void Validate_WhenChampionshipNameIsExactly200Characters_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: new string('A', 200),
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when JoinedDate is default")]
    public void Validate_WhenJoinedDateIsDefault_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithDefaultJoinedDate();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when LeftDate is before JoinedDate")]
    public void Validate_WhenLeftDateIsBeforeJoinedDate_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        
        // Manually set LeftDate before JoinedDate via reflection to test validation
        var leftDateProperty = typeof(TeamPlayer).GetProperty(nameof(TeamPlayer.LeftDate));
        leftDateProperty!.SetValue(teamPlayer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when LeftDate equals JoinedDate")]
    public void Validate_WhenLeftDateEqualsJoinedDate_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        
        // Manually set LeftDate equal to JoinedDate via reflection to test validation
        var leftDateProperty = typeof(TeamPlayer).GetProperty(nameof(TeamPlayer.LeftDate));
        leftDateProperty!.SetValue(teamPlayer, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when LeftDate is in the future")]
    public void Validate_WhenLeftDateIsInFuture_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        
        // Manually set LeftDate in the future via reflection to test validation
        var leftDateProperty = typeof(TeamPlayer).GetProperty(nameof(TeamPlayer.LeftDate));
        leftDateProperty!.SetValue(teamPlayer, DateTime.UtcNow.AddDays(30));

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns true when LeftDate is valid")]
    public void Validate_WhenLeftDateIsValid_ReturnsTrue()
    {
        var joinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var leftDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var teamPlayer = TestTeamPlayerFactory.CreateInactiveTeamPlayer(joinedDate, leftDate);

        var result = teamPlayer.Validate();

        Assert.True(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is empty")]
    public void Validate_WhenCreatedByIsEmpty_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithEmptyCreatedBy();

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    [Fact(DisplayName = "Validate returns false when CreatedBy is whitespace")]
    public void Validate_WhenCreatedByIsWhitespace_ReturnsFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateCustomTeamPlayer(
            teamName: "Test Team",
            championshipName: "Test Championship",
            joinedDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            createdBy: "   ");

        var result = teamPlayer.Validate();

        Assert.False(result);
    }

    #endregion

    #region MarkAsLeft Tests

    [Fact(DisplayName = "MarkAsLeft sets LeftDate correctly")]
    public void MarkAsLeft_WhenCalled_SetsLeftDate()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var leftDate = DateTime.UtcNow.AddDays(-1);

        teamPlayer.MarkAsLeft(leftDate, "admin-user");

        Assert.Equal(leftDate, teamPlayer.LeftDate);
    }

    [Fact(DisplayName = "MarkAsLeft updates audit fields")]
    public void MarkAsLeft_WhenCalled_UpdatesAuditFields()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var leftDate = DateTime.UtcNow.AddDays(-1);
        var beforeUpdate = DateTime.UtcNow;

        teamPlayer.MarkAsLeft(leftDate, "admin-user");

        var afterUpdate = DateTime.UtcNow;
        Assert.NotNull(teamPlayer.UpdatedAt);
        Assert.True(teamPlayer.UpdatedAt >= beforeUpdate);
        Assert.True(teamPlayer.UpdatedAt <= afterUpdate);
        Assert.Equal("admin-user", teamPlayer.UpdatedBy);
    }

    [Fact(DisplayName = "MarkAsLeft throws when userId is null")]
    public void MarkAsLeft_WhenUserIdIsNull_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), null!));
    }

    [Fact(DisplayName = "MarkAsLeft throws when userId is empty")]
    public void MarkAsLeft_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), ""));
    }

    [Fact(DisplayName = "MarkAsLeft throws when userId is whitespace")]
    public void MarkAsLeft_WhenUserIdIsWhitespace_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), "   "));
    }

    [Fact(DisplayName = "MarkAsLeft throws when player has already left")]
    public void MarkAsLeft_WhenPlayerHasAlreadyLeft_ThrowsInvalidOperationException()
    {
        var joinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var leftDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var teamPlayer = TestTeamPlayerFactory.CreateInactiveTeamPlayer(joinedDate, leftDate);

        Assert.Throws<InvalidOperationException>(() => teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), "admin-user"));
    }

    [Fact(DisplayName = "MarkAsLeft throws when leftDate is before JoinedDate")]
    public void MarkAsLeft_WhenLeftDateIsBeforeJoinedDate_ThrowsInvalidOperationException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithJoinedDate(new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        var invalidLeftDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        Assert.Throws<InvalidOperationException>(() => teamPlayer.MarkAsLeft(invalidLeftDate, "admin-user"));
    }

    [Fact(DisplayName = "MarkAsLeft throws when leftDate equals JoinedDate")]
    public void MarkAsLeft_WhenLeftDateEqualsJoinedDate_ThrowsInvalidOperationException()
    {
        var joinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var teamPlayer = TestTeamPlayerFactory.CreateTeamPlayerWithJoinedDate(joinedDate);

        Assert.Throws<InvalidOperationException>(() => teamPlayer.MarkAsLeft(joinedDate, "admin-user"));
    }

    [Fact(DisplayName = "MarkAsLeft throws when leftDate is in the future")]
    public void MarkAsLeft_WhenLeftDateIsInFuture_ThrowsInvalidOperationException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var futureDate = DateTime.UtcNow.AddDays(30);

        Assert.Throws<InvalidOperationException>(() => teamPlayer.MarkAsLeft(futureDate, "admin-user"));
    }

    [Fact(DisplayName = "MarkAsLeft changes IsActive to false")]
    public void MarkAsLeft_WhenCalled_ChangesIsActiveToFalse()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        Assert.True(teamPlayer.IsActive);

        teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), "admin-user");

        Assert.False(teamPlayer.IsActive);
    }

    #endregion

    #region IsCurrentlyActive Tests

    [Fact(DisplayName = "IsCurrentlyActive returns true when LeftDate is null")]
    public void IsCurrentlyActive_WhenLeftDateIsNull_ReturnsTrue()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        var result = teamPlayer.IsCurrentlyActive();

        Assert.True(result);
    }

    [Fact(DisplayName = "IsCurrentlyActive returns false when LeftDate is set")]
    public void IsCurrentlyActive_WhenLeftDateIsSet_ReturnsFalse()
    {
        var joinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var leftDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var teamPlayer = TestTeamPlayerFactory.CreateInactiveTeamPlayer(joinedDate, leftDate);

        var result = teamPlayer.IsCurrentlyActive();

        Assert.False(result);
    }

    [Fact(DisplayName = "IsCurrentlyActive matches IsActive property")]
    public void IsCurrentlyActive_WhenCalled_MatchesIsActiveProperty()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Equal(teamPlayer.IsActive, teamPlayer.IsCurrentlyActive());

        teamPlayer.MarkAsLeft(DateTime.UtcNow.AddDays(-1), "admin-user");

        Assert.Equal(teamPlayer.IsActive, teamPlayer.IsCurrentlyActive());
    }

    #endregion

    #region UpdateLastModified Tests

    [Fact(DisplayName = "UpdateLastModified sets UpdatedAt to current UTC time")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedAtToCurrentUtcTime()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();
        var beforeUpdate = DateTime.UtcNow;

        teamPlayer.UpdateLastModified("update-user");

        var afterUpdate = DateTime.UtcNow;
        Assert.NotNull(teamPlayer.UpdatedAt);
        Assert.True(teamPlayer.UpdatedAt >= beforeUpdate);
        Assert.True(teamPlayer.UpdatedAt <= afterUpdate);
    }

    [Fact(DisplayName = "UpdateLastModified sets UpdatedBy to provided userId")]
    public void UpdateLastModified_WhenCalled_SetsUpdatedByToProvidedUserId()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        teamPlayer.UpdateLastModified("update-user");

        Assert.Equal("update-user", teamPlayer.UpdatedBy);
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is null")]
    public void UpdateLastModified_WhenUserIdIsNull_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.UpdateLastModified(null!));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is empty")]
    public void UpdateLastModified_WhenUserIdIsEmpty_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.UpdateLastModified(""));
    }

    [Fact(DisplayName = "UpdateLastModified throws when userId is whitespace")]
    public void UpdateLastModified_WhenUserIdIsWhitespace_ThrowsArgumentException()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        Assert.Throws<ArgumentException>(() => teamPlayer.UpdateLastModified("   "));
    }

    [Fact(DisplayName = "UpdateLastModified can be called multiple times")]
    public void UpdateLastModified_CalledMultipleTimes_UpdatesEachTime()
    {
        var teamPlayer = TestTeamPlayerFactory.CreateValidTeamPlayer();

        teamPlayer.UpdateLastModified("first-user");
        var firstUpdate = teamPlayer.UpdatedAt;
        var firstUser = teamPlayer.UpdatedBy;

        System.Threading.Thread.Sleep(10);

        teamPlayer.UpdateLastModified("second-user");

        Assert.True(teamPlayer.UpdatedAt > firstUpdate);
        Assert.Equal("second-user", teamPlayer.UpdatedBy);
        Assert.NotEqual(firstUser, teamPlayer.UpdatedBy);
    }

    #endregion
}
