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
///   <item><description><b>TeamName:</b> Required, 1-200 characters, not whitespace only</description></item>
///   <item><description><b>ChampionshipName:</b> Required, 1-200 characters, not whitespace only</description></item>
///   <item><description><b>JoinedDate:</b> Required, not more than 1 year in the future</description></item>
///   <item><description><b>LeftDate:</b> Optional, if provided must be after JoinedDate and not in the future</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var createDto = new CreateTeamPlayerDto
/// {
///     PlayerId = 1,
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
    /// Maximum allowed length for a team name.
    /// </summary>
    public const int MaxTeamNameLength = 200;

    /// <summary>
    /// Maximum allowed length for a championship name.
    /// </summary>
    public const int MaxChampionshipNameLength = 200;

    /// <summary>
    /// Maximum number of years in the future a JoinedDate can be.
    /// </summary>
    public const int MaxFutureYearsForJoinedDate = 1;

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
    ///     PlayerId = 1,
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
    ///     TeamName = "Team Alpha",
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
    ///     PlayerId = 1,
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

        ValidateTeamName(teamPlayer.TeamName, errors);
        ValidateChampionshipName(teamPlayer.ChampionshipName, errors);
        ValidateJoinedDate(teamPlayer.JoinedDate, errors);
        ValidateLeftDate(teamPlayer.LeftDate, teamPlayer.JoinedDate, errors);

        return BuildResult(errors);
    }

    private static void ValidateTeamName(string? teamName, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(teamName))
        {
            AddError(errors, nameof(TeamPlayer.TeamName), "Team name is required");
            return;
        }

        var trimmedName = teamName.Trim();
        if (trimmedName.Length > MaxTeamNameLength)
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

        var trimmedName = championshipName.Trim();
        if (trimmedName.Length > MaxChampionshipNameLength)
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

        var maxFutureDate = DateTime.UtcNow.AddYears(MaxFutureYearsForJoinedDate);
        if (joinedDate > maxFutureDate)
        {
            AddError(errors, nameof(TeamPlayer.JoinedDate), "Joined date cannot be more than 1 year in the future");
        }
    }

    private static void ValidateLeftDate(DateTime? leftDate, DateTime joinedDate, Dictionary<string, List<string>> errors)
    {
        if (!leftDate.HasValue)
        {
            return;
        }

        if (leftDate.Value <= joinedDate)
        {
            AddError(errors, nameof(TeamPlayer.LeftDate), "Left date must be after the joined date");
        }

        if (leftDate.Value > DateTime.UtcNow)
        {
            AddError(errors, nameof(TeamPlayer.LeftDate), "Left date cannot be in the future");
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
