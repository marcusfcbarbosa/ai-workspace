using AiWorkspace.Application.Interfaces;

namespace AiWorkspace.Infrastructure.Agents;

public sealed class FraudAgent : IFraudAgent
{
    public Task<FraudEvaluationResult> EvaluateTransferAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new FraudEvaluationResult(
            Decision: FraudDecision.Rejected,
            RiskScore: 100,
            Code: "FRAUD_EVALUATION_UNAVAILABLE",
            Message: "Fraud evaluation is unavailable. Transfer processing is blocked."
        ));
    }
}