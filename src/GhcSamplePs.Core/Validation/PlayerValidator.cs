using GhcSamplePs.Core.Common;
using GhcSamplePs.Core.Models.PlayerManagement;
using GhcSamplePs.Core.Models.PlayerManagement.DTOs;

namespace GhcSamplePs.Core.Validation;

/// <summary>
/// Provides validation logic for player data, implementing all business rules defined in the specification.
/// </summary>
/// <remarks>
/// <para>Validation Rules:</para>
/// <list type="bullet">
///   <item><description><b>Name:</b> Required, 1-200 characters, not whitespace only</description></item>
///   <item><description><b>DateOfBirth:</b> Required, must be a past date, not more than 100 years ago</description></item>
///   <item><description><b>Gender:</b> Optional, if provided must be: Male, Female, Non-binary, or Prefer not to say (case-insensitive)</description></item>
///   <item><description><b>PhotoUrl:</b> Optional, if provided must be max 500 characters and a valid HTTP or HTTPS URL</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// // Validate a CreatePlayerDto
/// var createDto = new CreatePlayerDto
/// {
///     UserId = "user-123",
///     Name = "John Doe",
///     DateOfBirth = new DateTime(1990, 6, 15)
/// };
/// var result = PlayerValidator.ValidateCreatePlayer(createDto);
/// if (!result.IsValid)
/// {
///     foreach (var (field, messages) in result.Errors)
///     {
///         Console.WriteLine($"{field}: {string.Join(", ", messages)}");
///     }
/// }
/// </code>
/// </example>
public static class PlayerValidator
{
    /// <summary>
    /// Maximum allowed length for a player's name.
    /// </summary>
    public const int MaxNameLength = 200;

    /// <summary>
    /// Maximum allowed length for a player's photo URL.
    /// </summary>
    public const int MaxPhotoUrlLength = 500;

    /// <summary>
    /// Maximum age in years for date of birth validation.
    /// </summary>
    public const int MaxAgeYears = 100;

    /// <summary>
    /// Valid gender options (case-insensitive).
    /// Valid values: Male, Female, Non-binary, Prefer not to say.
    /// </summary>
    public static readonly string[] ValidGenderOptions = ["Male", "Female", "Non-binary", "Prefer not to say"];

    /// <summary>
    /// Validates a CreatePlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The CreatePlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    /// <example>
    /// <code>
    /// var createDto = new CreatePlayerDto
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15)
    /// };
    /// var result = PlayerValidator.ValidateCreatePlayer(createDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateCreatePlayer(CreatePlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateName(dto.Name, errors);
        ValidateDateOfBirth(dto.DateOfBirth, errors);
        ValidateGender(dto.Gender, errors);
        ValidatePhotoUrl(dto.PhotoUrl, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates an UpdatePlayerDto against all business rules.
    /// </summary>
    /// <param name="dto">The UpdatePlayerDto to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null.</exception>
    /// <example>
    /// <code>
    /// var updateDto = new UpdatePlayerDto
    /// {
    ///     Id = 1,
    ///     Name = "John Doe Updated",
    ///     DateOfBirth = new DateTime(1990, 6, 15)
    /// };
    /// var result = PlayerValidator.ValidateUpdatePlayer(updateDto);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidateUpdatePlayer(UpdatePlayerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var errors = new Dictionary<string, List<string>>();

        ValidateName(dto.Name, errors);
        ValidateDateOfBirth(dto.DateOfBirth, errors);
        ValidateGender(dto.Gender, errors);
        ValidatePhotoUrl(dto.PhotoUrl, errors);

        return BuildResult(errors);
    }

    /// <summary>
    /// Validates a Player entity against all business rules.
    /// </summary>
    /// <param name="player">The Player entity to validate.</param>
    /// <returns>A ValidationResult containing all validation errors found, or a valid result if no errors.</returns>
    /// <exception cref="ArgumentNullException">Thrown when player is null.</exception>
    /// <example>
    /// <code>
    /// var player = new Player
    /// {
    ///     UserId = "user-123",
    ///     Name = "John Doe",
    ///     DateOfBirth = new DateTime(1990, 6, 15),
    ///     CreatedBy = "system"
    /// };
    /// var result = PlayerValidator.ValidatePlayer(player);
    /// Console.WriteLine(result.IsValid); // True
    /// </code>
    /// </example>
    public static ValidationResult ValidatePlayer(Player player)
    {
        ArgumentNullException.ThrowIfNull(player);

        var errors = new Dictionary<string, List<string>>();

        ValidateName(player.Name, errors);
        ValidateDateOfBirth(player.DateOfBirth, errors);
        ValidateGender(player.Gender, errors);
        ValidatePhotoUrl(player.PhotoUrl, errors);

        return BuildResult(errors);
    }

    private static void ValidateName(string? name, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddError(errors, nameof(Player.Name), "Name is required");
            return;
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
        {
            AddError(errors, nameof(Player.Name), "Name must not exceed 200 characters");
        }
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth, Dictionary<string, List<string>> errors)
    {
        if (dateOfBirth == default)
        {
            AddError(errors, nameof(Player.DateOfBirth), "Date of birth is required");
            return;
        }

        var today = DateTime.UtcNow.Date;

        if (dateOfBirth.Date >= today)
        {
            AddError(errors, nameof(Player.DateOfBirth), "Date of birth must be in the past");
        }

        var maxPastDate = today.AddYears(-MaxAgeYears);
        if (dateOfBirth.Date < maxPastDate)
        {
            AddError(errors, nameof(Player.DateOfBirth), "Date of birth cannot be more than 100 years ago");
        }
    }

    private static void ValidateGender(string? gender, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(gender))
        {
            return;
        }

        var trimmedGender = gender.Trim();
        var isValidGender = ValidGenderOptions.Any(validOption =>
            string.Equals(validOption, trimmedGender, StringComparison.OrdinalIgnoreCase));

        if (!isValidGender)
        {
            AddError(errors, nameof(Player.Gender), "Gender must be Male, Female, Non-binary, or Prefer not to say");
        }
    }

    private static void ValidatePhotoUrl(string? photoUrl, Dictionary<string, List<string>> errors)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return;
        }

        var trimmedUrl = photoUrl.Trim();

        if (trimmedUrl.Length > MaxPhotoUrlLength)
        {
            AddError(errors, nameof(Player.PhotoUrl), "Photo URL must not exceed 500 characters");
        }

        if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            AddError(errors, nameof(Player.PhotoUrl), "Photo URL must be a valid HTTP or HTTPS URL");
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
