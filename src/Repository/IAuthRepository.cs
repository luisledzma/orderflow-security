namespace orderflow.security.Repository;
/// <summary>
/// Defines all the methods that <see cref="AuthRepository"/> must implement.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Authenticate User.
    /// </summary>
    /// <returns>The string Token or null.</returns>
    string? AuthenticateUser(LoginRequest request);
}