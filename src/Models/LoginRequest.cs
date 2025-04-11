namespace orderflow.security.Models;

/// <summary>
/// Represents the data required for a user to log in.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the username of the user attempting to log in.
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the user attempting to log in.
    /// </summary>
    public required string Password { get; set; }
}
