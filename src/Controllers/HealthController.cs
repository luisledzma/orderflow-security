using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace orderflow.security.Controllers;
/// <summary>
/// This controller provides health checks for this microservice.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/security/health")]
[ApiVersion("1.0")]
public class HealthController : ControllerBase
{
    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    public HealthController()
    {
    }
    #endregion
    #region Actions
    /// <summary>
    /// Gets the current health status of the microservice.
    /// </summary>
    /// <returns>A 200 OK, indicating the microservice is healthy.</returns>
    [HttpGet("getstatus")]
    [Authorize(Roles = "Admin")] // 🔐 This requires JWT authentication
    public IActionResult GetStatus()
    {
        return Ok(new { status = "Security Healthy", timestamp = DateTime.UtcNow });
    }
    #endregion
}
