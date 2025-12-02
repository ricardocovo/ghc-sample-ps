using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Service interface for player statistic management operations.
/// </summary>
public interface IPlayerStatisticService
{
    /// <summary>
    /// Retrieves all statistics for a specific player across all teams.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing a collection of player statistics.</returns>
    /// <example>
    /// <code>
    /// var result = await statisticService.GetStatisticsByPlayerIdAsync(1);
    /// if (result.Success)
    /// {
    ///     foreach (var stat in result.Data!)
    ///     {
    ///         Console.WriteLine($"{stat.GameDate}: Goals={stat.Goals}, Assists={stat.Assists}");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<IReadOnlyList<PlayerStatisticDto>>> GetStatisticsByPlayerIdAsync(
        int playerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all statistics for a specific team player assignment.
    /// </summary>
    /// <param name="teamPlayerId">The team player identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing a collection of player statistics for the specific team.</returns>
    /// <example>
    /// <code>
    /// var result = await statisticService.GetStatisticsByTeamPlayerIdAsync(1);
    /// if (result.Success)
    /// {
    ///     foreach (var stat in result.Data!)
    ///     {
    ///         Console.WriteLine($"{stat.GameDate}: Team={stat.TeamName}");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<IReadOnlyList<PlayerStatisticDto>>> GetStatisticsByTeamPlayerIdAsync(
        int teamPlayerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single player statistic by its unique identifier.
    /// </summary>
    /// <param name="statisticId">The unique identifier of the player statistic.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the player statistic if found.</returns>
    /// <example>
    /// <code>
    /// var result = await statisticService.GetStatisticByIdAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Found: {result.Data!.GameDate}, Goals: {result.Data!.Goals}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Error: {string.Join(", ", result.ErrorMessages)}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerStatisticDto>> GetStatisticByIdAsync(
        int statisticId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new player statistic to the system.
    /// </summary>
    /// <param name="createDto">The data transfer object containing player statistic information.</param>
    /// <param name="currentUserId">The identifier of the user creating the statistic.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the created player statistic if successful.</returns>
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
    /// var result = await statisticService.AddStatisticAsync(createDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Created statistic with ID: {result.Data!.PlayerStatisticId}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerStatisticDto>> AddStatisticAsync(
        CreatePlayerStatisticDto createDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player statistic in the system.
    /// </summary>
    /// <param name="statisticId">The unique identifier of the player statistic to update.</param>
    /// <param name="updateDto">The data transfer object containing updated player statistic information.</param>
    /// <param name="currentUserId">The identifier of the user making the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the updated player statistic if successful.</returns>
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
    ///     Assists = 2
    /// };
    /// var result = await statisticService.UpdateStatisticAsync(1, updateDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Updated: Goals={result.Data!.Goals}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerStatisticDto>> UpdateStatisticAsync(
        int statisticId,
        UpdatePlayerStatisticDto updateDto,
        string currentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player statistic from the system.
    /// </summary>
    /// <param name="statisticId">The unique identifier of the player statistic to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result indicating success or failure.</returns>
    /// <example>
    /// <code>
    /// var result = await statisticService.DeleteStatisticAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine("Statistic deleted successfully");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<bool>> DeleteStatisticAsync(
        int statisticId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves aggregate statistics (totals and averages) for a player.
    /// </summary>
    /// <param name="playerId">The player identifier.</param>
    /// <param name="teamPlayerId">Optional team player identifier to filter by specific team.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the aggregate statistics.</returns>
    /// <example>
    /// <code>
    /// var result = await statisticService.GetPlayerAggregatesAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Games: {result.Data!.GameCount}");
    ///     Console.WriteLine($"Total Goals: {result.Data!.TotalGoals}");
    ///     Console.WriteLine($"Avg Goals: {result.Data!.AverageGoals:F2}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerStatisticAggregateResult>> GetPlayerAggregatesAsync(
        int playerId,
        int? teamPlayerId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates player statistic data without persisting it.
    /// </summary>
    /// <param name="createDto">The data transfer object containing player statistic information to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A validation result indicating if the data is valid.</returns>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerStatisticDto
    /// {
    ///     TeamPlayerId = 1,
    ///     GameDate = new DateTime(2024, 3, 15),
    ///     MinutesPlayed = 90,
    ///     IsStarter = true,
    ///     JerseyNumber = 10,
    ///     Goals = 0,
    ///     Assists = 0
    /// };
    /// var result = await statisticService.ValidateStatisticAsync(createDto);
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine("Validation passed");
    /// }
    /// </code>
    /// </example>
    Task<ValidationResult> ValidateStatisticAsync(
        CreatePlayerStatisticDto createDto,
        CancellationToken cancellationToken = default);
}
