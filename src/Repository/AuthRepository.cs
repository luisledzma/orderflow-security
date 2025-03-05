using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace orderflow.security.Repository;

/// <summary>
/// Implement all the methods that <see cref="IAuthRepository"/> define.
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly IConfiguration _config;

    public AuthRepository(IConfiguration config)
    {
        _config = config;
    }

    public string? AuthenticateUser(LoginRequest request)
    {
        // Simulate user validation (In a real case, check DB)
        if (request.Username == "admin" && request.Password == "password123")
        {
            return GenerateJwtToken(request.Username);
        }

        return null; // Authentication failed
    }

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