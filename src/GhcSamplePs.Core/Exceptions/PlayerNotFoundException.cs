namespace GhcSamplePs.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested player cannot be found.
/// </summary>
public class PlayerNotFoundException : Exception
{
    /// <summary>
    /// Gets the identifier of the player that was not found.
    /// </summary>
    public int? PlayerId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotFoundException"/> class with a default message.
    /// </summary>
    public PlayerNotFoundException()
        : base("The requested player could not be found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotFoundException"/> class with a specified player ID.
    /// </summary>
    /// <param name="playerId">The identifier of the player that was not found.</param>
    public PlayerNotFoundException(int playerId)
        : base($"Player with ID {playerId} could not be found.")
    {
        PlayerId = playerId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PlayerNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerNotFoundException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PlayerNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
