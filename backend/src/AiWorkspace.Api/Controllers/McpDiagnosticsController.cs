
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/mcp")]
public sealed class McpDiagnosticsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public McpDiagnosticsController(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("tools")]

    public async Task<IActionResult> GetTools(
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("BankingMcp");

        try
        {
            using var initialization = await SendAsync(
                client,
                "initialize",
                1,
                new
                {
                    protocolVersion = "2025-03-26",
                    capabilities = new { },
                    clientInfo = new
                    {
                        name = "AiWorkspace.Api",
                        version = "1.0.0"
                    }
                },
                cancellationToken);

            if (!initialization.IsSuccessStatusCode)
            {
                return StatusCode(502, new
                {
                    Error = "MCP initialization failed.",
                    HttpStatus = (int)initialization.StatusCode
                });
            }

            using var discovery = await SendAsync(
                client,
                "tools/list",
                2,
                new { },
                cancellationToken);

            if (!discovery.IsSuccessStatusCode)
            {
                return StatusCode(502, new
                {
                    Error = "MCP tool discovery failed.",
                    HttpStatus = (int)discovery.StatusCode
                });
            }

            var content = await discovery.Content.ReadAsStringAsync(
                cancellationToken);

            if (content.StartsWith(
                    "event:",
                    StringComparison.OrdinalIgnoreCase) ||
                content.StartsWith(
                    "data:",
                    StringComparison.OrdinalIgnoreCase))
            {
                var dataLine = content
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(line =>
                        line.StartsWith(
                            "data:",
                            StringComparison.OrdinalIgnoreCase));

                if (dataLine is null)
                {
                    return StatusCode(502, new
                    {
                        Error = "MCP event stream did not include a response."
                    });
                }

                content = dataLine[5..].Trim();
            }

            using var document = JsonDocument.Parse(content);

            if (!document.RootElement.TryGetProperty(
                    "result",
                    out var result) ||
                !result.TryGetProperty(
                    "tools",
                    out var tools))
            {
                return StatusCode(502, new
                {
                    Error = "Unexpected MCP response."
                });
            }

            return Ok(new
            {
                Server = "AiWorkspace.McpServer",
                Tools = tools.Clone()
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new
            {
                Error =
                    "MCP server is unavailable. " +
                    "Start AiWorkspace.McpServer on port 5083."
            });
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                Error = "MCP server request timed out."
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                Error = "MCP server returned an invalid response."
            });
        }
    }

    private static async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        string method,
        int id,
        object parameters,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "mcp")
        {
            Content = JsonContent.Create(new
            {
                jsonrpc = "2.0",
                id,
                method,
                @params = parameters
            })
        };

        request.Headers.Accept.ParseAdd("application/json");
        request.Headers.Accept.ParseAdd("text/event-stream");

        return await client.SendAsync(
            request,
            cancellationToken);
    }
}
