using Microsoft.AspNetCore.Mvc;
using orderflow.security.Repository;
using orderflow.security.Models;

namespace orderflow.security.Controllers;

/// <summary>
/// Controller responsible for handling authentication operations,
/// such as user login and JWT token issuance.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/security/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authRepository">The authentication repository used for user validation and token generation.</param>
    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    /// Authenticates the user and returns a JWT token if the credentials are valid.
    /// </summary>
    /// <param name="request">The login request containing username and password.</param>
    /// <returns>
    /// An HTTP 200 response with the token if successful; otherwise, HTTP 401 Unauthorized.
    /// </returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _authRepository.AuthenticateUser(request);
        if (token == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(new { token });
    }
}
