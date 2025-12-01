using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Tests.TestHelpers;

/// <summary>
/// Test helper class for creating TeamPlayer instances for testing.
/// </summary>
public static class TestTeamPlayerFactory
{
    /// <summary>
    /// Creates a valid test team player with default values.
    /// </summary>
    /// <returns>A valid TeamPlayer instance.</returns>
    public static TeamPlayer CreateValidTeamPlayer()
    {
        return new TeamPlayer
        {
            TeamPlayerId = 1,
            PlayerId = 1,
            TeamName = "Team Alpha",
            ChampionshipName = "Championship 2024",
            JoinedDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user",
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
    }

    /// <summary>
    /// Creates a team player with a specific joined date for testing.
    /// </summary>
    /// <param name="joinedDate">The date when the player joined the team.</param>
    /// <returns>A TeamPlayer instance with the specified joined date.</returns>
    public static TeamPlayer CreateTeamPlayerWithJoinedDate(DateTime joinedDate)
    {
        return new TeamPlayer
        {
            TeamPlayerId = 1,
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = joinedDate,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player who has left the team.
    /// </summary>
    /// <param name="joinedDate">The date when the player joined the team.</param>
    /// <param name="leftDate">The date when the player left the team.</param>
    /// <returns>A TeamPlayer instance with LeftDate set.</returns>
    public static TeamPlayer CreateInactiveTeamPlayer(DateTime joinedDate, DateTime leftDate)
    {
        var teamPlayer = new TeamPlayer
        {
            TeamPlayerId = 1,
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = joinedDate,
            CreatedBy = "test-user"
        };
        teamPlayer.MarkAsLeft(leftDate, "test-user");
        return teamPlayer;
    }

    /// <summary>
    /// Creates a team player with minimal required properties.
    /// </summary>
    /// <returns>A TeamPlayer instance with only required properties.</returns>
    public static TeamPlayer CreateMinimalTeamPlayer()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Minimal Team",
            ChampionshipName = "Minimal Championship",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player with custom properties for specific test scenarios.
    /// </summary>
    /// <param name="teamName">Team name.</param>
    /// <param name="championshipName">Championship name.</param>
    /// <param name="joinedDate">Date when the player joined.</param>
    /// <param name="playerId">Player ID (foreign key).</param>
    /// <param name="createdBy">Created by user ID.</param>
    /// <returns>A customized TeamPlayer instance.</returns>
    public static TeamPlayer CreateCustomTeamPlayer(
        string teamName,
        string championshipName,
        DateTime joinedDate,
        int playerId = 1,
        string createdBy = "test-user")
    {
        return new TeamPlayer
        {
            PlayerId = playerId,
            TeamName = teamName,
            ChampionshipName = championshipName,
            JoinedDate = joinedDate,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Creates a team player with a team name that exceeds the maximum length.
    /// </summary>
    /// <returns>A TeamPlayer instance with an invalid team name length.</returns>
    public static TeamPlayer CreateTeamPlayerWithLongTeamName()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = new string('A', 201),
            ChampionshipName = "Test Championship",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player with a championship name that exceeds the maximum length.
    /// </summary>
    /// <returns>A TeamPlayer instance with an invalid championship name length.</returns>
    public static TeamPlayer CreateTeamPlayerWithLongChampionshipName()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = new string('A', 201),
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player with an empty team name.
    /// </summary>
    /// <returns>A TeamPlayer instance with an empty team name.</returns>
    public static TeamPlayer CreateTeamPlayerWithEmptyTeamName()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "",
            ChampionshipName = "Test Championship",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player with an empty championship name.
    /// </summary>
    /// <returns>A TeamPlayer instance with an empty championship name.</returns>
    public static TeamPlayer CreateTeamPlayerWithEmptyChampionshipName()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a team player with an empty CreatedBy.
    /// </summary>
    /// <returns>A TeamPlayer instance with an empty CreatedBy.</returns>
    public static TeamPlayer CreateTeamPlayerWithEmptyCreatedBy()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = ""
        };
    }

    /// <summary>
    /// Creates a team player with default JoinedDate (invalid).
    /// </summary>
    /// <returns>A TeamPlayer instance with default JoinedDate.</returns>
    public static TeamPlayer CreateTeamPlayerWithDefaultJoinedDate()
    {
        return new TeamPlayer
        {
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = default,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a valid player entity for use with TeamPlayer tests.
    /// </summary>
    /// <param name="id">The player ID.</param>
    /// <returns>A valid Player instance.</returns>
    public static Player CreateValidPlayerForTeamPlayer(int id = 1)
    {
        return new Player
        {
            Id = id,
            UserId = "test-owner-id",
            Name = "Test Player",
            DateOfBirth = new DateTime(1990, 6, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }
}
