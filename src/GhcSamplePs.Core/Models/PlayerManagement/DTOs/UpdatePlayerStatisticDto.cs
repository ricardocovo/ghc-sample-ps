using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing player statistic.
/// Contains all updatable properties for player statistic modification with validation attributes.
/// </summary>
public sealed record UpdatePlayerStatisticDto
{
    /// <summary>
    /// Gets the unique identifier of the player statistic to update. Required.
    /// </summary>
    [Required(ErrorMessage = "Player Statistic ID is required.")]
    public required int PlayerStatisticId { get; init; }

    /// <summary>
    /// Gets the foreign key to the associated TeamPlayer entity. Required.
    /// </summary>
    [Required(ErrorMessage = "Team Player ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Team Player ID must be greater than 0.")]
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the date of the game for which statistics are recorded. Required.
    /// </summary>
    [Required(ErrorMessage = "Game date is required.")]
    public required DateTime GameDate { get; init; }

    /// <summary>
    /// Gets the number of minutes the player played in the game. Required, must be non-negative.
    /// </summary>
    [Required(ErrorMessage = "Minutes played is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Minutes played must be non-negative.")]
    public required int MinutesPlayed { get; init; }

    /// <summary>
    /// Gets a value indicating whether the player started the game. Required.
    /// </summary>
    [Required(ErrorMessage = "Is starter is required.")]
    public required bool IsStarter { get; init; }

    /// <summary>
    /// Gets the player's jersey number for this game. Required, must be greater than 0.
    /// </summary>
    [Required(ErrorMessage = "Jersey number is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Jersey number must be greater than 0.")]
    public required int JerseyNumber { get; init; }

    /// <summary>
    /// Gets the number of goals scored by the player in the game. Required, must be non-negative.
    /// </summary>
    [Required(ErrorMessage = "Goals is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Goals must be non-negative.")]
    public required int Goals { get; init; }

    /// <summary>
    /// Gets the number of assists made by the player in the game. Required, must be non-negative.
    /// </summary>
    [Required(ErrorMessage = "Assists is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Assists must be non-negative.")]
    public required int Assists { get; init; }

    /// <summary>
    /// Creates a new PlayerStatistic entity from this DTO values for updating.
    /// </summary>
    /// <param name="currentUserId">The identifier of the user making the update.</param>
    /// <returns>A new PlayerStatistic entity with updated values.</returns>
    /// <exception cref="ArgumentException">Thrown when currentUserId is null, empty, or whitespace.</exception>
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
    /// var entity = updateDto.ToEntity("admin-user");
    /// </code>
    /// </example>
    public PlayerStatistic ToEntity(string currentUserId)
    {
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new ArgumentException("CurrentUserId cannot be null, empty, or whitespace.", nameof(currentUserId));
        }

        var entity = new PlayerStatistic
        {
            PlayerStatisticId = PlayerStatisticId,
            TeamPlayerId = TeamPlayerId,
            GameDate = GameDate,
            MinutesPlayed = MinutesPlayed,
            IsStarter = IsStarter,
            JerseyNumber = JerseyNumber,
            Goals = Goals,
            Assists = Assists,
            CreatedBy = currentUserId
        };

        entity.UpdateLastModified(currentUserId);

        return entity;
    }
}
