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
    /// Gets the date when the player left the team.
    /// Null indicates the player is still active on the team.
    /// If provided, must be after JoinedDate and not in the future.
    /// </summary>
    public DateTime? LeftDate { get; init; }
}
