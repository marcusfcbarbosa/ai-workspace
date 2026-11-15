
using AiWorkspace.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AiWorkspace.Infrastructure.Persistence;

public sealed class LedgerRepository : ILedgerRepository
{
    private readonly LedgerDbContext _context;

    public LedgerRepository(LedgerDbContext context)
    {
        _context = context;
    }

    public async Task<bool> TransactionExistsAsync(
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ArgumentException(
                "Idempotency key is required.",
                nameof(idempotencyKey));
        }

        cancellationToken.ThrowIfCancellationRequested();

        return await _context.LedgerTransactions
            .AsNoTracking()
            .AnyAsync(
                transaction =>
                    transaction.IdempotencyKey == idempotencyKey,
                cancellationToken);
    }

    public Task SaveTransferAsync(
        Guid transactionId,
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        string idempotencyKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (transactionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Transaction ID is required.",
                nameof(transactionId));
        }

        if (sourceAccountId == Guid.Empty ||
            destinationAccountId == Guid.Empty)
        {
            throw new ArgumentException(
                "Valid account identifiers are required.");
        }

        if (sourceAccountId == destinationAccountId)
        {
            throw new ArgumentException(
                "Source and destination accounts must be different.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Transfer amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(currency) ||
            currency.Trim().Length != 3)
        {
            throw new ArgumentException(
                "Currency must be a three-letter code.",
                nameof(currency));
        }

        if (string.IsNullOrWhiteSpace(idempotencyKey) ||
            idempotencyKey.Length > 128)
        {
            throw new ArgumentException(
                "Idempotency key must contain between 1 and 128 characters.",
                nameof(idempotencyKey));
        }

        throw new NotSupportedException(
            "Ledger posting requires account authorization, balance validation, and atomic persistence.");
    }
}
