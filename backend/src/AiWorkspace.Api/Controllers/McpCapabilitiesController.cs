
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Client;

namespace AiWorkspace.Api.Controllers;

[ApiController]
//[Authorize]
[Route("api/mcp")]
public sealed class McpCapabilitiesController : ControllerBase
{
    private readonly ILogger<McpCapabilitiesController> _logger;

    public McpCapabilitiesController(
        ILogger<McpCapabilitiesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("capabilities")]
    public async Task<IActionResult> GetCapabilities(
        CancellationToken cancellationToken)
    {
        try
        {
            var transport = new HttpClientTransport(
                new HttpClientTransportOptions
                {
                    Endpoint = new Uri(
                        "http://127.0.0.1:5083/mcp")
                });

            await using var client = await McpClient.CreateAsync(
                transport,
                cancellationToken: cancellationToken);

            var tools = await client.ListToolsAsync(
                cancellationToken: cancellationToken);

            var tool = tools.FirstOrDefault(x => x.Name == "get_banking_capabilities");

            if (tool is null)
            {
                return NotFound(new
                {
                    Error = "GetBankingCapabilities tool was not found.",
                    AvailableTools = tools.Select(x => x.Name)
                });
            }

            _logger.LogInformation(
                "Executing MCP tool: {ToolName}",
                tool.Name);

            var result = await client.CallToolAsync(
                tool.Name,
                new Dictionary<string, object?>(),
                cancellationToken: cancellationToken);

            return Ok(new
            {
                Tool = tool.Name,
                IsError = result.IsError,
                Content = result.Content
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to execute MCP banking capabilities tool.");

            return StatusCode(502, new
            {
                Error = "MCP tool execution failed."
            });
        }
    }
}
