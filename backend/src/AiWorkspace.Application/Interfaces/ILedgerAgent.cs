namespace AiWorkspace.Application.Interfaces;

public interface ILedgerAgent
{
    Task<LedgerPostingResult> PostTransferAsync(
        Guid transactionId,
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}

public sealed record LedgerPostingResult(
    bool Success,
    Guid TransactionId,
    string Code,
    string Message,
    DateTimeOffset? PostedAt
);
