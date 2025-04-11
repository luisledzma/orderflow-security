using orderflow.security.Models;

namespace orderflow.security.Service;
/// <summary>
/// Defines all the methods that <see cref="AuthService"/> must implement.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate User.
    /// </summary>
    /// <returns>The string Token or null.</returns>
    string? AuthenticateUser(LoginRequest request);
}