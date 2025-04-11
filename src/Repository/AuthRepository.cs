using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using orderflow.security.Models;

namespace orderflow.security.Repository;

/// <summary>
/// Implements authentication-related logic such as validating users and generating JWT tokens.
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthRepository"/> class.
    /// </summary>
    /// <param name="config">The configuration instance used to access JWT settings.</param>
    public AuthRepository(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Authenticates the user with the given login request.
    /// </summary>
    /// <param name="request">The login request containing username and password.</param>
    /// <returns>A JWT token if authentication is successful; otherwise, null.</returns>
    public string? AuthenticateUser(LoginRequest request)
    {
        // Simulate user validation (In a real case, check DB)
        if (request.Username == "admin" && request.Password == "password123")
        {
            return GenerateJwtToken(request.Username);
        }

        return null; // Authentication failed
    }

    /// <summary>
    /// Generates a JWT token for the specified username.
    /// </summary>
    /// <param name="username">The username for which to generate the token.</param>
    /// <returns>A JWT token string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any required JWT settings are missing.</exception>
    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        if (jwtSettings == null)
        {
            throw new ArgumentNullException(nameof(jwtSettings), "JWT settings are missing from configuration.");
        }

        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("Key is missing in JwtSettings."));

        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin") // Assign roles
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        return tokenHandler.CreateEncodedJwt(tokenDescriptor);
    }
}