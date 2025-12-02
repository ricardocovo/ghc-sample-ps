namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for displaying player statistic information.
/// Contains all player statistic properties including team context for read operations.
/// </summary>
public sealed record PlayerStatisticDto
{
    /// <summary>
    /// Gets the unique identifier for the player statistic record.
    /// </summary>
    public required int PlayerStatisticId { get; init; }

    /// <summary>
    /// Gets the foreign key to the associated TeamPlayer entity.
    /// </summary>
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the name of the team.
    /// </summary>
    public string? TeamName { get; init; }

    /// <summary>
    /// Gets the name of the championship.
    /// </summary>
    public string? ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date of the game for which statistics are recorded.
    /// </summary>
    public required DateTime GameDate { get; init; }

    /// <summary>
    /// Gets the number of minutes the player played in the game.
    /// </summary>
    public required int MinutesPlayed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player started the game.
    /// </summary>
    public required bool IsStarter { get; init; }

    /// <summary>
    /// Gets the player's jersey number for this game.
    /// </summary>
    public required int JerseyNumber { get; init; }

    /// <summary>
    /// Gets the number of goals scored by the player in the game.
    /// </summary>
    public required int Goals { get; init; }

    /// <summary>
    /// Gets the number of assists made by the player in the game.
    /// </summary>
    public required int Assists { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player statistic record was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the identifier of the user who created this player statistic record.
    /// </summary>
    public required string CreatedBy { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when the player statistic record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the identifier of the user who last updated this player statistic record.
    /// </summary>
    public string? UpdatedBy { get; init; }

    /// <summary>
    /// Creates a PlayerStatisticDto from a PlayerStatistic entity.
    /// </summary>
    /// <param name="statistic">The player statistic entity to map from.</param>
    /// <returns>A new PlayerStatisticDto instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when statistic is null.</exception>
    /// <example>
    /// <code>
    /// var statistic = new PlayerStatistic
    /// {
    ///     PlayerStatisticId = 1,
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 2,
    ///     Assists = 1,
    ///     CreatedBy = "system"
    /// };
    /// var dto = PlayerStatisticDto.FromEntity(statistic);
    /// </code>
    /// </example>
    public static PlayerStatisticDto FromEntity(PlayerStatistic statistic)
    {
        ArgumentNullException.ThrowIfNull(statistic);

        return new PlayerStatisticDto
        {
            PlayerStatisticId = statistic.PlayerStatisticId,
            TeamPlayerId = statistic.TeamPlayerId,
            TeamName = statistic.TeamPlayer?.TeamName?.Trim(),
            ChampionshipName = statistic.TeamPlayer?.ChampionshipName?.Trim(),
            GameDate = statistic.GameDate,
            MinutesPlayed = statistic.MinutesPlayed,
            IsStarter = statistic.IsStarter,
            JerseyNumber = statistic.JerseyNumber,
            Goals = statistic.Goals,
            Assists = statistic.Assists,
            CreatedAt = statistic.CreatedAt,
            CreatedBy = statistic.CreatedBy,
            UpdatedAt = statistic.UpdatedAt,
            UpdatedBy = statistic.UpdatedBy
        };
    }
}
