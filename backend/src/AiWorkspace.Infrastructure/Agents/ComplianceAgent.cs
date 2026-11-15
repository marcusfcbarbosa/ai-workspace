
using AiWorkspace.Application.Interfaces;

namespace AiWorkspace.Infrastructure.Agents;

public sealed class ComplianceAgent : IComplianceAgent
{
    public Task<ComplianceEvaluationResult> EvaluateTransferAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new ComplianceEvaluationResult(
            Decision: ComplianceDecision.Rejected,
            Code: "COMPLIANCE_EVALUATION_UNAVAILABLE",
            Message: "Compliance evaluation is unavailable. Transfer processing is blocked.",
            PolicyReference: null
        ));
    }
}
