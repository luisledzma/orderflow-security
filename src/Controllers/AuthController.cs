using Microsoft.AspNetCore.Mvc;
using orderflow.security.Repository;

namespace orderflow.security.Controllers;

/// <summary>
/// Controller to handle authentication.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/security/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;

    public AuthController(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    /// <summary>
    /// Login endpoint that returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>A JWT token if successful.</returns>
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


