using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing team player assignment.
/// Contains all updatable properties for team player modification with validation attributes.
/// </summary>
public sealed record UpdateTeamPlayerDto
{
    /// <summary>
    /// Gets the unique identifier of the team player to update. Required.
    /// </summary>
    [Required(ErrorMessage = "Team Player ID is required.")]
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the name of the team. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Team name is required.")]
    [StringLength(200, ErrorMessage = "Team name cannot exceed 200 characters.")]
    public required string TeamName { get; init; }

    /// <summary>
    /// Gets the name of the championship. Required, maximum 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Championship name is required.")]
    [StringLength(200, ErrorMessage = "Championship name cannot exceed 200 characters.")]
    public required string ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date when the player joined the team. Required.
    /// </summary>
    [Required(ErrorMessage = "Joined date is required.")]
    public required DateTime JoinedDate { get; init; }

    /// <summary>
    /// Gets the date when the player left the team. Optional.
    /// If provided, must be after JoinedDate and cannot be in the future.
    /// </summary>
    public DateTime? LeftDate { get; init; }

    /// <summary>
    /// Creates a TeamPlayer entity from this DTO for update operations.
    /// </summary>
    /// <param name="existingTeamPlayer">The existing team player entity to update.</param>
    /// <param name="updatedBy">The identifier of the user making the update.</param>
    /// <returns>A new TeamPlayer entity with updated values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when existingTeamPlayer is null.</exception>
    /// <exception cref="ArgumentException">Thrown when updatedBy is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This method creates a new TeamPlayer entity with the updated values.
    /// The PlayerId, CreatedAt, and CreatedBy fields are preserved from the original entity.
    /// </remarks>
    /// <example>
    /// <code>
    /// var updateDto = new UpdateTeamPlayerDto
    /// {
    ///     TeamPlayerId = 1,
    ///     TeamName = "Team Beta",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     LeftDate = new DateTime(2024, 6, 30)
    /// };
    /// var updatedTeamPlayer = updateDto.ToEntity(existingTeamPlayer, "admin-user");
    /// </code>
    /// </example>
    public TeamPlayer ToEntity(TeamPlayer existingTeamPlayer, string updatedBy)
    {
        ArgumentNullException.ThrowIfNull(existingTeamPlayer);

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy cannot be null, empty, or whitespace.", nameof(updatedBy));
        }

        var teamPlayer = new TeamPlayer
        {
            TeamPlayerId = existingTeamPlayer.TeamPlayerId,
            PlayerId = existingTeamPlayer.PlayerId,
            TeamName = TeamName.Trim(),
            ChampionshipName = ChampionshipName.Trim(),
            JoinedDate = JoinedDate,
            CreatedAt = existingTeamPlayer.CreatedAt,
            CreatedBy = existingTeamPlayer.CreatedBy
        };

        if (LeftDate.HasValue)
        {
            teamPlayer.MarkAsLeft(LeftDate.Value, updatedBy);
        }
        else
        {
            teamPlayer.UpdateLastModified(updatedBy);
        }

        return teamPlayer;
    }
}
