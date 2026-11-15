
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AiWorkspace.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiWorkspace.Infrastructure.AI;

public sealed class OnboardingAgent : IOnboardingAgent
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OnboardingAgent(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> ProcessAsync(
        string message,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException(
                "Message cannot be empty.",
                nameof(message));

        var apiKey = _configuration["OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException(
                "OpenAI API key is not configured.");

        var model = _configuration["OpenAI:Model"]
            ?? "gpt-4.1-mini";

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/responses");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Content = JsonContent.Create(new
        {
            model,
            instructions = """
                You are a banking customer onboarding assistant.

                Help customers understand account registration.

                This is a simulated banking application.

                Never claim that an account has been created.
                Never claim that identity verification is complete.
                Never execute financial transactions.
                Never request passwords or banking credentials.

                Explain what information is needed and
                ask for missing details one step at a time.
                """,
            input = message,
            max_output_tokens = 600,
            store = false
        });

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(
                cancellationToken),
            cancellationToken: cancellationToken);

        var output = document.RootElement
            .GetProperty("output");

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content))
                continue;

            foreach (var part in content.EnumerateArray())
            {
                if (part.TryGetProperty("type", out var type)
                    && type.GetString() == "output_text"
                    && part.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
            }
        }

        throw new InvalidOperationException(
            "OpenAI did not return a text response.");
    }
}
