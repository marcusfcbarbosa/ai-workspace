using Microsoft.AspNetCore.Mvc;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            Application = "AI Banking Workspace",
            Version = "1.0.0",
            Framework = ".NET 9",
            Modules = new[]
            {
                "Customer Management",
                "AI Agents",
                "MCP Server",
                "Tools",
                "RAG",
                "Banking Transactions"
            }
        });
    }
}
