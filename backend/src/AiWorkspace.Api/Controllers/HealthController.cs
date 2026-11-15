using Microsoft.AspNetCore.Mvc;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Application = "AI Banking Workspace",
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}
