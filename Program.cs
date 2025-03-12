using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using orderflow.security.Repository;
using orderflow.security.Service;
using Serilog;
using System.Text;
using orderflow.security.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

if (jwtSettings == null)
{
    throw new ArgumentNullException(nameof(jwtSettings), "JWT settings are missing from configuration.");
}

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("Key is missing in JwtSettings."));

//Add Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Add services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddScoped<IAuthService, AuthService>();

// Add JWT authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,  // Ensures the token is not expired
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        // Optional: Log token validation failures
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
           {
               Console.WriteLine("JWT Challenge: Token validation failed.");
               return Task.CompletedTask;
           },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };

    });


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Logs to file
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddAuthorization(); // Enables authorization

builder.WebHost.UseUrls("http://0.0.0.0:5052"); // Security Service runs on port 5052

var app = builder.Build();

app.UseMiddleware<LoggingMiddleware>();

// Ensure logging is captured during shutdown
app.Lifetime.ApplicationStopping.Register(Log.CloseAndFlush);

// Enable middleware
app.UseRouting();  // Move routing to the correct order

app.UseAuthentication(); // Use authentication middleware
app.UseAuthorization();  // Use authorization middleware

// Map controllers to routes
app.MapControllers();

app.Run();
