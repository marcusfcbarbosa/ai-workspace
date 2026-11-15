
using AiWorkspace.Domain.Ledger;

namespace AiWorkspace.Application.Interfaces;

public interface ILedgerRepository
{
    Task<bool> TransactionExistsAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default);

    Task SaveTransferAsync(
        Guid transactionId,
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}
