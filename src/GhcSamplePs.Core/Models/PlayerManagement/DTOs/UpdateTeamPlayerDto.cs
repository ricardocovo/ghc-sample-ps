using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing team player assignment.
/// Contains the updatable properties for team player modification with validation attributes.
/// </summary>
public sealed record UpdateTeamPlayerDto
{
    /// <summary>
    /// Gets the unique identifier of the team player assignment to update. Required.
    /// </summary>
    [Required(ErrorMessage = "Team Player ID is required.")]
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the team name. Required, 1-200 characters, not whitespace only.
    /// </summary>
    [Required(ErrorMessage = "Team name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Team name must be between 1 and 200 characters.")]
    public required string TeamName { get; init; }

    /// <summary>
    /// Gets the championship name. Required, 1-200 characters, not whitespace only.
    /// </summary>
    [Required(ErrorMessage = "Championship name is required.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Championship name must be between 1 and 200 characters.")]
    public required string ChampionshipName { get; init; }

    /// <summary>
    /// Gets the date when the player joined the team. Required.
    /// Must not be more than 1 year in the future.
    /// </summary>
    [Required(ErrorMessage = "Joined date is required.")]
    public required DateTime JoinedDate { get; init; }

    /// <summary>
    /// Gets the date when the player left the team.
    /// Null indicates the player is still active on the team.
    /// If provided, must be after JoinedDate and not in the future.
    /// </summary>
    public DateTime? LeftDate { get; init; }
}
