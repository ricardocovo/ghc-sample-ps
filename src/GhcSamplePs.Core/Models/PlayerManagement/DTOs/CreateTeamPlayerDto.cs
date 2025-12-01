using System.ComponentModel.DataAnnotations;

namespace GhcSamplePs.Core.Models.PlayerManagement.DTOs;

/// <summary>
/// Data transfer object for creating a new team player assignment.
/// Contains all required properties for team player creation with validation attributes.
/// </summary>
public sealed record CreateTeamPlayerDto
{
    /// <summary>
    /// Gets the identifier of the player to assign. Required, must be positive.
    /// </summary>
    [Required(ErrorMessage = "Player ID is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Player ID must be a positive integer.")]
    public required int PlayerId { get; init; }

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
    /// Creates a TeamPlayer entity from this DTO.
    /// </summary>
    /// <param name="createdBy">The identifier of the user creating the team player assignment.</param>
    /// <returns>A new TeamPlayer entity.</returns>
    /// <exception cref="ArgumentException">Thrown when createdBy is null, empty, or whitespace.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreateTeamPlayerDto
    /// {
    ///     PlayerId = 123,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15)
    /// };
    /// var teamPlayer = createDto.ToEntity("admin-user");
    /// </code>
    /// </example>
    public TeamPlayer ToEntity(string createdBy)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy cannot be null, empty, or whitespace.", nameof(createdBy));
        }

        return new TeamPlayer
        {
            PlayerId = PlayerId,
            TeamName = TeamName.Trim(),
            ChampionshipName = ChampionshipName.Trim(),
            JoinedDate = JoinedDate,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }
}
