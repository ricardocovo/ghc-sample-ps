using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Service interface for team player management operations.
/// </summary>
public interface ITeamPlayerService
{
    /// <summary>
    /// Retrieves all team assignments for a specific player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="includeInactive">If true, includes inactive (left) team assignments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing a collection of team player assignments.</returns>
    /// <example>
    /// <code>
    /// var result = await teamPlayerService.GetTeamsByPlayerIdAsync(1, includeInactive: true);
    /// if (result.Success)
    /// {
    ///     foreach (var team in result.Data!)
    ///     {
    ///         Console.WriteLine($"{team.TeamName} ({team.ChampionshipName})");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<IReadOnlyList<TeamPlayerDto>>> GetTeamsByPlayerIdAsync(
        int playerId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves only active team assignments for a specific player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing a collection of active team player assignments.</returns>
    /// <example>
    /// <code>
    /// var result = await teamPlayerService.GetActiveTeamsByPlayerIdAsync(1);
    /// if (result.Success)
    /// {
    ///     foreach (var team in result.Data!)
    ///     {
    ///         Console.WriteLine($"Active: {team.TeamName}");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<IReadOnlyList<TeamPlayerDto>>> GetActiveTeamsByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single team player assignment by its unique identifier.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier of the team player assignment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the team player assignment if found.</returns>
    /// <example>
    /// <code>
    /// var result = await teamPlayerService.GetTeamAssignmentByIdAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Found: {result.Data!.TeamName}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Error: {string.Join(", ", result.ErrorMessages)}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<TeamPlayerDto>> GetTeamAssignmentByIdAsync(
        int teamPlayerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a player to a team.
    /// </summary>
    /// <param name="createDto">The data transfer object containing team player information.</param>
    /// <param name="currentUserId">The identifier of the user creating the assignment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the created team player assignment if successful.</returns>
    /// <example>
    /// <code>
    /// var createDto = new CreateTeamPlayerDto
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15)
    /// };
    /// var result = await teamPlayerService.AddPlayerToTeamAsync(createDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Created team assignment with ID: {result.Data!.TeamPlayerId}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<TeamPlayerDto>> AddPlayerToTeamAsync(
        CreateTeamPlayerDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing team player assignment.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier of the team player assignment to update.</param>
    /// <param name="updateDto">The data transfer object containing updated team player information.</param>
    /// <param name="currentUserId">The identifier of the user making the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the updated team player assignment if successful.</returns>
    /// <example>
    /// <code>
    /// var updateDto = new UpdateTeamPlayerDto
    /// {
    ///     TeamPlayerId = 1,
    ///     LeftDate = new DateTime(2024, 6, 30)
    /// };
    /// var result = await teamPlayerService.UpdateTeamAssignmentAsync(1, updateDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Updated: {result.Data!.TeamName}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<TeamPlayerDto>> UpdateTeamAssignmentAsync(
        int teamPlayerId,
        UpdateTeamPlayerDto updateDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a player from a team by setting the left date.
    /// </summary>
    /// <param name="teamPlayerId">The unique identifier of the team player assignment.</param>
    /// <param name="leftDate">The date when the player left the team.</param>
    /// <param name="currentUserId">The identifier of the user making the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the updated team player assignment if successful.</returns>
    /// <example>
    /// <code>
    /// var result = await teamPlayerService.RemovePlayerFromTeamAsync(1, new DateTime(2024, 6, 30), "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Player removed from team, IsActive: {result.Data!.IsActive}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<TeamPlayerDto>> RemovePlayerFromTeamAsync(
        int teamPlayerId,
        DateTime leftDate,
        string currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates team assignment data without persisting it.
    /// </summary>
    /// <param name="createDto">The data transfer object containing team player information to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A validation result indicating if the data is valid.</returns>
    /// <example>
    /// <code>
    /// var createDto = new CreateTeamPlayerDto
    /// {
    ///     PlayerId = 1,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15)
    /// };
    /// var result = await teamPlayerService.ValidateTeamAssignmentAsync(createDto);
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine("Validation passed");
    /// }
    /// </code>
    /// </example>
    Task<ValidationResult> ValidateTeamAssignmentAsync(
        CreateTeamPlayerDto createDto,
        CancellationToken cancellationToken = default);
}
