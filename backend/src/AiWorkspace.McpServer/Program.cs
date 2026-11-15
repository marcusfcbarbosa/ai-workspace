
using ModelContextProtocol.Server;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5083");

builder.Configuration["AllowedHosts"] = "localhost;127.0.0.1";

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapGet("/health", () =>
    Results.Ok(new
    {
        Status = "Healthy",
        Service = "AiWorkspace.McpServer",
        LlmEnabled = false
    }));

app.MapMcp("/mcp");

app.Run();
