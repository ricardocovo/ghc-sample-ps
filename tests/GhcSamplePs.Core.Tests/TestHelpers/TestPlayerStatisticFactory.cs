using GhcSamplePs.Core.Models.PlayerManagement;

namespace GhcSamplePs.Core.Tests.TestHelpers;

/// <summary>
/// Test helper class for creating PlayerStatistic instances for testing.
/// </summary>
public static class TestPlayerStatisticFactory
{
    /// <summary>
    /// Creates a valid test player statistic with default values.
    /// </summary>
    /// <returns>A valid PlayerStatistic instance.</returns>
    public static PlayerStatistic CreateValidPlayerStatistic()
    {
        return new PlayerStatistic
        {
            PlayerStatisticId = 1,
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 2,
            Assists = 1,
            CreatedBy = "test-user",
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
    }

    /// <summary>
    /// Creates a player statistic with minimal required properties.
    /// </summary>
    /// <returns>A PlayerStatistic instance with only required properties.</returns>
    public static PlayerStatistic CreateMinimalPlayerStatistic()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 0,
            IsStarter = false,
            JerseyNumber = 1,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with custom properties for specific test scenarios.
    /// </summary>
    /// <param name="teamPlayerId">Team player ID (foreign key).</param>
    /// <param name="gameDate">Date of the game.</param>
    /// <param name="minutesPlayed">Minutes played in the game.</param>
    /// <param name="isStarter">Whether the player started the game.</param>
    /// <param name="jerseyNumber">Jersey number for the game.</param>
    /// <param name="goals">Goals scored.</param>
    /// <param name="assists">Assists made.</param>
    /// <param name="createdBy">Created by user ID.</param>
    /// <returns>A customized PlayerStatistic instance.</returns>
    public static PlayerStatistic CreateCustomPlayerStatistic(
        int teamPlayerId = 1,
        DateTime? gameDate = null,
        int minutesPlayed = 90,
        bool isStarter = true,
        int jerseyNumber = 10,
        int goals = 0,
        int assists = 0,
        string createdBy = "test-user")
    {
        return new PlayerStatistic
        {
            TeamPlayerId = teamPlayerId,
            GameDate = gameDate ?? new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = minutesPlayed,
            IsStarter = isStarter,
            JerseyNumber = jerseyNumber,
            Goals = goals,
            Assists = assists,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Creates a player statistic with invalid TeamPlayerId (zero).
    /// </summary>
    /// <returns>A PlayerStatistic instance with invalid TeamPlayerId.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithInvalidTeamPlayerId()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 0,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with default GameDate (invalid).
    /// </summary>
    /// <returns>A PlayerStatistic instance with default GameDate.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithDefaultGameDate()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = default,
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with negative MinutesPlayed (invalid).
    /// </summary>
    /// <returns>A PlayerStatistic instance with negative MinutesPlayed.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithNegativeMinutesPlayed()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = -1,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with invalid JerseyNumber (zero).
    /// </summary>
    /// <returns>A PlayerStatistic instance with invalid JerseyNumber.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithInvalidJerseyNumber()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 0,
            Goals = 0,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with negative Goals (invalid).
    /// </summary>
    /// <returns>A PlayerStatistic instance with negative Goals.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithNegativeGoals()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = -1,
            Assists = 0,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with negative Assists (invalid).
    /// </summary>
    /// <returns>A PlayerStatistic instance with negative Assists.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithNegativeAssists()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = -1,
            CreatedBy = "test-user"
        };
    }

    /// <summary>
    /// Creates a player statistic with an empty CreatedBy (invalid).
    /// </summary>
    /// <returns>A PlayerStatistic instance with an empty CreatedBy.</returns>
    public static PlayerStatistic CreatePlayerStatisticWithEmptyCreatedBy()
    {
        return new PlayerStatistic
        {
            TeamPlayerId = 1,
            GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
            MinutesPlayed = 90,
            IsStarter = true,
            JerseyNumber = 10,
            Goals = 0,
            Assists = 0,
            CreatedBy = ""
        };
    }

    /// <summary>
    /// Creates a list of player statistics for aggregate testing.
    /// </summary>
    /// <returns>A list of PlayerStatistic instances.</returns>
    public static List<PlayerStatistic> CreatePlayerStatisticList()
    {
        return
        [
            new PlayerStatistic
            {
                PlayerStatisticId = 1,
                TeamPlayerId = 1,
                GameDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                MinutesPlayed = 90,
                IsStarter = true,
                JerseyNumber = 10,
                Goals = 2,
                Assists = 1,
                CreatedBy = "test-user"
            },
            new PlayerStatistic
            {
                PlayerStatisticId = 2,
                TeamPlayerId = 1,
                GameDate = new DateTime(2024, 3, 22, 0, 0, 0, DateTimeKind.Utc),
                MinutesPlayed = 45,
                IsStarter = false,
                JerseyNumber = 10,
                Goals = 1,
                Assists = 2,
                CreatedBy = "test-user"
            },
            new PlayerStatistic
            {
                PlayerStatisticId = 3,
                TeamPlayerId = 1,
                GameDate = new DateTime(2024, 3, 29, 0, 0, 0, DateTimeKind.Utc),
                MinutesPlayed = 60,
                IsStarter = true,
                JerseyNumber = 10,
                Goals = 0,
                Assists = 3,
                CreatedBy = "test-user"
            }
        ];
    }

    /// <summary>
    /// Creates a valid TeamPlayer entity for use with PlayerStatistic tests.
    /// </summary>
    /// <param name="id">The team player ID.</param>
    /// <returns>A valid TeamPlayer instance.</returns>
    public static TeamPlayer CreateValidTeamPlayerForStatistic(int id = 1)
    {
        return new TeamPlayer
        {
            TeamPlayerId = id,
            PlayerId = 1,
            TeamName = "Test Team",
            ChampionshipName = "Test Championship",
            JoinedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = "test-user"
        };
    }
}
