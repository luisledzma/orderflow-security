using orderflow.security.Models;
using orderflow.security.Repository;

namespace orderflow.security.Service;
/// <summary>
/// Implement all the methods that <see cref="IAuthService"/> define.
/// </summary>
public class AuthService : IAuthService
{
    #region Properties
    /// <summary>
    /// The <c>_repository</c> property represents the service from where the token is retrieved.
    /// </summary>
    private IAuthRepository _repository;
    #endregion
    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="repository">The repository service used to retrieve the generated Token.</param>
    public AuthService
    (IAuthRepository repository)
    {
        _repository = repository;
    }
    #endregion
    #region IAuthService
    /// <inheritdoc cref="IAuthService.AuthenticateUser"/>
    public string? AuthenticateUser(LoginRequest request)
    {
        return _repository.AuthenticateUser(request);
    }
    #endregion
}