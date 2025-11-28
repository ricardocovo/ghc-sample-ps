using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Services.Interfaces;

/// <summary>
/// Service interface for player management operations.
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Retrieves all players from the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing a collection of all players.</returns>
    /// <example>
    /// <code>
    /// var result = await playerService.GetAllPlayersAsync();
    /// if (result.Success)
    /// {
    ///     foreach (var player in result.Data!)
    ///     {
    ///         Console.WriteLine($"{player.Name}, Age: {player.Age}");
    ///     }
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<IReadOnlyList<PlayerDto>>> GetAllPlayersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single player by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the player.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the player if found.</returns>
    /// <example>
    /// <code>
    /// var result = await playerService.GetPlayerByIdAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Found: {result.Data!.Name}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Error: {string.Join(", ", result.ErrorMessages)}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerDto>> GetPlayerByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new player in the system.
    /// </summary>
    /// <param name="createDto">The data transfer object containing player information.</param>
    /// <param name="currentUserId">The identifier of the user creating the player.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the created player if successful.</returns>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerDto
    /// {
    ///     UserId = "owner-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(2010, 5, 15),
    ///     Gender = "Male"
    /// };
    /// var result = await playerService.CreatePlayerAsync(createDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Created player with ID: {result.Data!.Id}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerDto>> CreatePlayerAsync(CreatePlayerDto createDto, string currentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing player in the system.
    /// </summary>
    /// <param name="id">The unique identifier of the player to update.</param>
    /// <param name="updateDto">The data transfer object containing updated player information.</param>
    /// <param name="currentUserId">The identifier of the user making the update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result containing the updated player if successful.</returns>
    /// <example>
    /// <code>
    /// var updateDto = new UpdatePlayerDto
    /// {
    ///     Id = 1,
    ///     Name = "John Doe Updated",
    ///     DateOfBirth = new DateTime(2010, 5, 15),
    ///     Gender = "Male"
    /// };
    /// var result = await playerService.UpdatePlayerAsync(1, updateDto, "admin-user");
    /// if (result.Success)
    /// {
    ///     Console.WriteLine($"Updated: {result.Data!.Name}");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<PlayerDto>> UpdatePlayerAsync(int id, UpdatePlayerDto updateDto, string currentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a player from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the player to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A service result indicating success or failure.</returns>
    /// <example>
    /// <code>
    /// var result = await playerService.DeletePlayerAsync(1);
    /// if (result.Success)
    /// {
    ///     Console.WriteLine("Player deleted successfully");
    /// }
    /// </code>
    /// </example>
    Task<ServiceResult<bool>> DeletePlayerAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates player data without persisting it.
    /// </summary>
    /// <param name="createDto">The data transfer object containing player information to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A validation result indicating if the data is valid.</returns>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerDto
    /// {
    ///     UserId = "owner-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(2010, 5, 15)
    /// };
    /// var result = await playerService.ValidatePlayerAsync(createDto);
    /// if (result.IsValid)
    /// {
    ///     Console.WriteLine("Validation passed");
    /// }
    /// </code>
    /// </example>
    Task<ValidationResult> ValidatePlayerAsync(CreatePlayerDto createDto, CancellationToken cancellationToken = default);
}
