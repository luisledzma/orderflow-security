using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using orderflow.security.Middleware;
using orderflow.security.Repository;
using orderflow.security.Service;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- Configuration --------------------
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

if (jwtSettings == null)
{
    throw new ArgumentNullException(nameof(jwtSettings), "JWT settings are missing from configuration.");
}

var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("Key is missing in JwtSettings."));

// -------------------- Logging --------------------
// Log.Logger = new LoggerConfiguration()
//     .WriteTo.Console()
//     .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//     .Enrich.FromLogContext()
//     .CreateLogger();

// builder.Host.UseSerilog();

// -------------------- Services --------------------

// Add controllers and HTTP context accessor
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.UseSecurityTokenValidators = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// -------------------- Web Host --------------------
builder.WebHost.UseUrls("http://0.0.0.0:5052"); // Security Service runs on port 5052

var app = builder.Build();

// -------------------- Middleware --------------------
app.UseMiddleware<LoggingMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local") || app.Environment.IsEnvironment("PreProduction"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Lifetime.ApplicationStopping.Register(Log.CloseAndFlush);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
