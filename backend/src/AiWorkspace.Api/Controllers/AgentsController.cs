
using AiWorkspace.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/agents")]
public sealed class AgentsController : ControllerBase
{
    private readonly IOnboardingAgent _onboardingAgent;

    public AgentsController(IOnboardingAgent onboardingAgent)
    {
        _onboardingAgent = onboardingAgent;
    }

    [HttpGet]
    public IActionResult GetAgents()
    {
        return Ok(new[]
        {
            new
            {
                Id = "customer-onboarding",
                Name = "Customer Onboarding Agent",
                Description = "Rule-based banking onboarding assistant",
                ExecutionMode = "Deterministic",
                RequiresLlm = false
            }
        });
    }

    [HttpPost("onboarding/chat")]
    public async Task<IActionResult> Chat(
        [FromBody] OnboardingChatRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                Error = "Message is required."
            });
        }

        var response = await _onboardingAgent.ProcessAsync(
            request.Message,
            cancellationToken);

        return Ok(new
        {
            Agent = "Customer Onboarding Agent",
            Response = response
        });
    }
}

public sealed record OnboardingChatRequest(string Message);
