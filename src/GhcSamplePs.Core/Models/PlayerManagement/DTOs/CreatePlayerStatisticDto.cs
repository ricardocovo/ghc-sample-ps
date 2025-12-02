using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for creating a new player statistic record.
/// Contains all required properties for player statistic creation with validation attributes.
/// </summary>
public sealed record CreatePlayerStatisticDto
{
    /// <summary>
    /// Gets the foreign key to the associated TeamPlayer entity. Required, must be positive.
    /// </summary>
    [Required(ErrorMessage = "Team Player ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Team Player ID must be a positive integer.")]
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the date of the game for which statistics are recorded. Required.
    /// </summary>
    [Required(ErrorMessage = "Game date is required.")]
    public required DateTime GameDate { get; init; }

    /// <summary>
    /// Gets the number of minutes the player played in the game. Required, must be 0-120.
    /// </summary>
    [Required(ErrorMessage = "Minutes played is required.")]
    [Range(0, 120, ErrorMessage = "Minutes played must be between 0 and 120.")]
    public required int MinutesPlayed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player started the game. Required.
    /// </summary>
    [Required(ErrorMessage = "Is starter is required.")]
    public required bool IsStarter { get; init; }

    /// <summary>
    /// Gets the player's jersey number for this game. Required, must be 1-99.
    /// </summary>
    [Required(ErrorMessage = "Jersey number is required.")]
    [Range(1, 99, ErrorMessage = "Jersey number must be between 1 and 99.")]
    public required int JerseyNumber { get; init; }

    /// <summary>
    /// Gets the number of goals scored by the player in the game. Required, must be non-negative.
    /// </summary>
    [Required(ErrorMessage = "Goals is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Goals must be a non-negative integer.")]
    public required int Goals { get; init; }

    /// <summary>
    /// Gets the number of assists made by the player in the game. Required, must be non-negative.
    /// </summary>
    [Required(ErrorMessage = "Assists is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Assists must be a non-negative integer.")]
    public required int Assists { get; init; }

    /// <summary>
    /// Creates a PlayerStatistic entity from this DTO.
    /// </summary>
    /// <param name="createdBy">The identifier of the user creating the player statistic record.</param>
    /// <returns>A new PlayerStatistic entity.</returns>
    /// <exception cref="ArgumentException">Thrown when createdBy is null, empty, or whitespace.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerStatisticDto
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 2,
    ///     Assists = 1
    /// };
    /// var playerStatistic = createDto.ToEntity("admin-user");
    /// </code>
    /// </example>
    public PlayerStatistic ToEntity(string createdBy)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy cannot be null, empty, or whitespace.", nameof(createdBy));
        }

        return new PlayerStatistic
        {
            TeamPlayerId = TeamPlayerId,
            GameDate = GameDate,
            MinutesPlayed = MinutesPlayed,
            IsStarter = IsStarter,
            JerseyNumber = JerseyNumber,
            Goals = Goals,
            Assists = Assists,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
