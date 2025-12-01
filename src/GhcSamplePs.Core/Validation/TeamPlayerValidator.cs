using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Validation;

/// <summary>
/// Provides validation logic for team player data, implementing all business rules defined in the specification.
/// </summary>
/// <remarks>
/// <para>Validation Rules:</para>
/// <list type="bullet">
///   <item><description><b>TeamName:</b> Required, 1-200 characters (untrimmed length), not whitespace only</description></item>
///   <item><description><b>ChampionshipName:</b> Required, 1-200 characters (untrimmed length), not whitespace only</description></item>
///   <item><description><b>JoinedDate:</b> Required, valid date, not more than 1 year in future, not more than 100 years in past</description></item>
///   <item><description><b>LeftDate:</b> Optional, if provided must be after JoinedDate and cannot be in the future</description></item>
///   <item><description><b>PlayerId:</b> Must be a positive integer</description></item>
/// </list>
/// <para>
/// Note: Length validation checks the untrimmed string length to match entity validation behavior.
/// DTOs trim values when creating entities, so input with leading/trailing whitespace should be
/// validated before trimming if the total length including whitespace exceeds 200 characters.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Validate a CreateTeamPlayerDto
/// var createDto = new CreateTeamPlayerDto
/// {
///     PlayerId = 123,
///     TeamName = "Team Alpha",
///     ChampionshipName = "Championship 2024",
///     JoinedDate = new DateTime(2024, 1, 15)
/// };
/// var result = TeamPlayerValidator.ValidateCreateTeamPlayer(createDto);
/// if (!result.IsValid)
/// {
///     foreach (var (field, messages) in result.Errors)
///     {
///         Console.WriteLine($"{field}: {string.Join(", ", messages)}");
///     }
/// }
/// </code>
/// </example>
public static class TeamPlayerValidator
{
    /// <summary>
    /// Maximum allowed length for team name and championship name.
    /// </summary>
    public const int MaxNameLength = 200;

    /// <summary>
    /// Maximum years in the past for joined date validation.
    /// </summary>
    public const int MaxPastYears = 100;

    /// <summary>
    /// Maximum years in the future for joined date validation.
    /// </summary>
    public const int MaxFutureYears = 1;

    /// <summary>
    /// Validates a CreateTeamPlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The CreateTeamPlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreateTeamPlayerDto
    /// {
    ///     PlayerId = 123,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15)
    /// };
    /// var result = TeamPlayerValidator.ValidateCreateTeamPlayer(createDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateCreateTeamPlayer(CreateTeamPlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidatePlayerId(dto.PlayerId, errors);
        ValidateTeamName(dto.TeamName, errors);
        ValidateChampionshipName(dto.ChampionshipName, errors);
        ValidateJoinedDate(dto.JoinedDate, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates an UpdateTeamPlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The UpdateTeamPlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
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
    /// var result = TeamPlayerValidator.ValidateUpdateTeamPlayer(updateDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateUpdateTeamPlayer(UpdateTeamPlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateTeamName(dto.TeamName, errors);
        ValidateChampionshipName(dto.ChampionshipName, errors);
        ValidateJoinedDate(dto.JoinedDate, errors);
        ValidateLeftDate(dto.LeftDate, dto.JoinedDate, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates a TeamPlayer entity against all business rules.
    /// </summary>
    /// <param name="teamPlayer">The TeamPlayer entity to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when teamPlayer is null.</exception>
    /// <example>
    /// <code>
    /// var teamPlayer = new TeamPlayer
    /// {
    ///     PlayerId = 123,
    ///     TeamName = "Team Alpha",
    ///     ChampionshipName = "Championship 2024",
    ///     JoinedDate = new DateTime(2024, 1, 15),
    ///     CreatedBy = "system"
    /// };
    /// var result = TeamPlayerValidator.ValidateTeamPlayer(teamPlayer);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateTeamPlayer(TeamPlayer teamPlayer)
    {
        ArgumentNullException.ThrowIfNull(teamPlayer);

        var errors = new Dictionary<string, List<string>>();

        ValidatePlayerId(teamPlayer.PlayerId, errors);
        ValidateTeamName(teamPlayer.TeamName, errors);
        ValidateChampionshipName(teamPlayer.ChampionshipName, errors);
        ValidateJoinedDate(teamPlayer.JoinedDate, errors);
        ValidateLeftDate(teamPlayer.LeftDate, teamPlayer.JoinedDate, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates a CreateTeamPlayerDto against all business rules. Alias for ValidateCreateTeamPlayer.
    /// </summary>
    /// <param name="dto">The CreateTeamPlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    public static ValidationResult Validate(CreateTeamPlayerDto dto) => ValidateCreateTeamPlayer(dto);

    /// <summary>
    /// Validates an UpdateTeamPlayerDto against all business rules. Alias for ValidateUpdateTeamPlayer.
    /// </summary>
    /// <param name="dto">The UpdateTeamPlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    public static ValidationResult Validate(UpdateTeamPlayerDto dto) => ValidateUpdateTeamPlayer(dto);

    private static void ValidatePlayerId(int playerId, Dictionary<string, List<string>> errors)
    {
        if (playerId <= 0)
        {
            AddError(errors, nameof(TeamPlayer.PlayerId), "Player ID must be a positive integer");
        }
    }

    private static void ValidateTeamName(string? teamName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(teamName))
        {
            AddError(errors, nameof(TeamPlayer.TeamName), "Team name is required");
            return;
        }

        // Check untrimmed length to match entity validation behavior
        if (teamName.Length > MaxNameLength)
        {
            AddError(errors, nameof(TeamPlayer.TeamName), "Team name must not exceed 200 characters");
        }
    }

    private static void ValidateChampionshipName(string? championshipName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(championshipName))
        {
            AddError(errors, nameof(TeamPlayer.ChampionshipName), "Championship name is required");
            return;
        }

        // Check untrimmed length to match entity validation behavior
        if (championshipName.Length > MaxNameLength)
        {
            AddError(errors, nameof(TeamPlayer.ChampionshipName), "Championship name must not exceed 200 characters");
        }
    }

    private static void ValidateJoinedDate(DateTime joinedDate, Dictionary<string, List<string>> errors)
    {
        if (joinedDate == default)
        {
            AddError(errors, nameof(TeamPlayer.JoinedDate), "Joined date is required");
            return;
        }

        var today = DateTime.UtcNow.Date;

        var maxFutureDate = today.AddYears(MaxFutureYears);
        if (joinedDate.Date > maxFutureDate)
        {
            AddError(errors, nameof(TeamPlayer.JoinedDate), "Joined date cannot be more than 1 year in the future");
        }

        var maxPastDate = today.AddYears(-MaxPastYears);
        if (joinedDate.Date < maxPastDate)
        {
            AddError(errors, nameof(TeamPlayer.JoinedDate), "Joined date cannot be more than 100 years in the past");
        }
    }

    private static void ValidateLeftDate(DateTime? leftDate, DateTime joinedDate, Dictionary<string, List<string>> errors)
    {
        if (!leftDate.HasValue)
        {
            return;
        }

        var today = DateTime.UtcNow.Date;

        if (leftDate.Value.Date > today)
        {
            AddError(errors, nameof(TeamPlayer.LeftDate), "Left date cannot be in the future");
        }

        if (leftDate.Value.Date <= joinedDate.Date)
        {
            AddError(errors, nameof(TeamPlayer.LeftDate), "Left date must be after joined date");
        }
    }

    private static void AddError(Dictionary<string, List<string>> errors, string fieldName, string errorMessage)
    {
        if (!errors.TryGetValue(fieldName, out var fieldErrors))
        {
            fieldErrors = [];
            errors[fieldName] = fieldErrors;
        }

        fieldErrors.Add(errorMessage);
    }

    private static ValidationResult BuildResult(Dictionary<string, List<string>> errors)
    {
        if (errors.Count == 0)
        {
            return ValidationResult.Valid();
        }

        var errorDictionary = errors.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToArray());

        return ValidationResult.Invalid(errorDictionary);
    }
}
