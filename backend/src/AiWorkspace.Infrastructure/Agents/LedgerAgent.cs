
using AiWorkspace.Application.Interfaces;

namespace AiWorkspace.Infrastructure.Agents;

public sealed class LedgerAgent : ILedgerAgent
{
    public Task<LedgerPostingResult> PostTransferAsync(
        Guid transactionId,
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new LedgerPostingResult(
            Success: false,
            TransactionId: transactionId,
            Code: "LEDGER_POSTING_UNAVAILABLE",
            Message: "Ledger posting is unavailable. No funds have been transferred.",
            PostedAt: null
        ));
    }
}
