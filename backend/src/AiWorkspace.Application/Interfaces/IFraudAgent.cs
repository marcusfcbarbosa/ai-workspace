namespace AiWorkspace.Application.Interfaces;

public interface IFraudAgent
{
    Task<FraudEvaluationResult> EvaluateTransferAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);
}

public sealed record FraudEvaluationResult(
    FraudDecision Decision,
    int RiskScore,
    string Code,
    string Message
);

public enum FraudDecision
{
    Approved,
    Rejected,
    ManualReview
}
