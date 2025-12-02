using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing player statistic record.
/// Contains all updatable properties for player statistic modification with validation attributes.
/// </summary>
public sealed record UpdatePlayerStatisticDto
{
    /// <summary>
    /// Gets the unique identifier of the player statistic record to update. Required.
    /// </summary>
    [Required(ErrorMessage = "Player Statistic ID is required.")]
    public required int PlayerStatisticId { get; init; }

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
    /// Applies the updates from this DTO to an existing PlayerStatistic entity.
    /// </summary>
    /// <param name="existingStatistic">The existing player statistic entity to update.</param>
    /// <param name="updatedBy">The identifier of the user making the update.</param>
    /// <returns>A new PlayerStatistic entity with updated values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when existingStatistic is null.</exception>
    /// <exception cref="ArgumentException">Thrown when updatedBy is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This method creates a new PlayerStatistic entity with the updated values.
    /// The CreatedAt and CreatedBy fields are preserved from the original entity.
    /// </remarks>
    /// <example>
    /// <code>
    /// var updateDto = new UpdatePlayerStatisticDto
    /// {
    ///     PlayerStatisticId = 1,
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 3,
    ///     Assists = 1
    /// };
    /// var updatedStatistic = updateDto.ApplyTo(existingStatistic, "admin-user");
    /// </code>
    /// </example>
    public PlayerStatistic ApplyTo(PlayerStatistic existingStatistic, string updatedBy)
    {
        ArgumentNullException.ThrowIfNull(existingStatistic);

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy cannot be null, empty, or whitespace.", nameof(updatedBy));
        }

        var statistic = new PlayerStatistic
        {
            PlayerStatisticId = existingStatistic.PlayerStatisticId,
            TeamPlayerId = TeamPlayerId,
            GameDate = GameDate,
            MinutesPlayed = MinutesPlayed,
            IsStarter = IsStarter,
            JerseyNumber = JerseyNumber,
            Goals = Goals,
            Assists = Assists,
            CreatedAt = existingStatistic.CreatedAt,
            CreatedBy = existingStatistic.CreatedBy
        };

        statistic.UpdateLastModified(updatedBy);

        return statistic;
    }
}
