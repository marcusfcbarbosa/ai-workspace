namespace AiWorkspace.Application.Interfaces;

public interface IComplianceAgent
{
    Task<ComplianceEvaluationResult> EvaluateTransferAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);
}

public sealed record ComplianceEvaluationResult(
    ComplianceDecision Decision,
    string Code,
    string Message,
    string? PolicyReference
);

public enum ComplianceDecision
{
    Approved,
    Rejected,
    ManualReview
}
