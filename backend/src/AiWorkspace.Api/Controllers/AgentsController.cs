
using AiWorkspace.Application.Interfaces;
using AiWorkspace.Application.Transfers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/agents")]
public sealed class AgentsController : ControllerBase
{
    private readonly IOnboardingAgent _onboardingAgent;
    private readonly IBankingOrchestrator _bankingOrchestrator;

    public AgentsController(
        IOnboardingAgent onboardingAgent,
        IBankingOrchestrator bankingOrchestrator)
    {
        _onboardingAgent = onboardingAgent;
        _bankingOrchestrator = bankingOrchestrator;
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
                ExecutionMode = "Deterministic"
            },
            new
            {
                Id = "banking-orchestrator",
                Name = "Banking Orchestrator",
                ExecutionMode = "Deterministic"
            }
        });
    }

    [HttpPost("onboarding/chat")]
    public async Task<IActionResult> OnboardingChat(
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

    [HttpPost("chat")]
    public async Task<IActionResult> BankingChat(
        [FromBody] BankingChatRequest request,
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

        var message = request.Message.Trim();

        if (BankingIntentRouter.IsTransferIntent(message))
        {
            if (request.Transfer is null)
            {
                return Ok(new
                {
                    Agent = "Banking Orchestrator",
                    Intent = "BANK_TRANSFER",
                    Status = "NeedsInformation",
                    Response =
                        "Transfer requests require sourceAccountId, " +
                        "destinationAccountId, amount, currency, " +
                        "and idempotencyKey in the transfer object. " +
                        "No transfer was executed."
                });
            }

            // This endpoint is intentionally non-executing.
            // A transfer must use a dedicated, authorized workflow
            // with account ownership and transaction checks.
            return Ok(new
            {
                Agent = "Banking Orchestrator",
                Intent = "BANK_TRANSFER",
                Status = "RequiresAuthorizedTransferEndpoint",
                Response =
                    "Transfer details were provided, but no transfer " +
                    "was executed. Use the authorized transfer workflow " +
                    "after account ownership, fraud, compliance, and " +
                    "ledger controls have been implemented."
            });
        }

        if (BankingIntentRouter.IsOnboardingIntent(message))
        {
            var response = await _onboardingAgent.ProcessAsync(
                message,
                cancellationToken);

            return Ok(new
            {
                Agent = "Customer Onboarding Agent",
                Intent = "ACCOUNT_ONBOARDING",
                Response = response
            });
        }

        return Ok(new
        {
            Agent = "Banking Chat Router",
            Intent = "UNKNOWN",
            Response =
                "I can help with account opening and transfer requests. " +
                "Please describe what you would like to do."
        });
    }
}

public sealed record OnboardingChatRequest(string Message);

public sealed record BankingChatRequest(
    string Message,
    TransferRequest? Transfer);
