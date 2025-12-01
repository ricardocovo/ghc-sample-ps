using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for updating an existing team player assignment.
/// Only allows updating the LeftDate field, as other fields (TeamName, ChampionshipName, 
/// JoinedDate, PlayerId) are immutable after creation per DDD principles.
/// </summary>
public sealed record UpdateTeamPlayerDto
{
    /// <summary>
    /// Gets the unique identifier of the team player to update. Required.
    /// </summary>
    [Required(ErrorMessage = "Team Player ID is required.")]
    public required int TeamPlayerId { get; init; }

    /// <summary>
    /// Gets the date when the player left the team. Optional.
    /// If provided, cannot be in the future. The constraint that LeftDate must be after JoinedDate
    /// is enforced by the domain entity's MarkAsLeft method.
    /// </summary>
    public DateTime? LeftDate { get; init; }

    /// <summary>
    /// Applies the update to an existing TeamPlayer entity.
    /// </summary>
    /// <param name="existingTeamPlayer">The existing team player entity to update.</param>
    /// <param name="updatedBy">The identifier of the user making the update.</param>
    /// <exception cref="ArgumentNullException">Thrown when existingTeamPlayer is null.</exception>
    /// <exception cref="ArgumentException">Thrown when updatedBy is null, empty, or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when LeftDate validation fails.</exception>
    /// <remarks>
    /// This method applies the LeftDate update to the existing entity using domain methods.
    /// Core identity fields (PlayerId, TeamName, ChampionshipName, JoinedDate) are immutable
    /// and cannot be changed after creation, following DDD principles.
    /// </remarks>
    /// <example>
    /// <code>
    /// var updateDto = new UpdateTeamPlayerDto
    /// {
    ///     TeamPlayerId = 1,
    ///     LeftDate = new DateTime(2024, 6, 30)
    /// };
    /// updateDto.ApplyTo(existingTeamPlayer, "admin-user");
    /// // existingTeamPlayer.LeftDate is now set, IsActive returns false
    /// </code>
    /// </example>
    public void ApplyTo(TeamPlayer existingTeamPlayer, string updatedBy)
    {
        ArgumentNullException.ThrowIfNull(existingTeamPlayer);

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy cannot be null, empty, or whitespace.", nameof(updatedBy));
        }

        if (LeftDate.HasValue)
        {
            existingTeamPlayer.MarkAsLeft(LeftDate.Value, updatedBy);
        }
        else
        {
            existingTeamPlayer.UpdateLastModified(updatedBy);
        }
    }
}
