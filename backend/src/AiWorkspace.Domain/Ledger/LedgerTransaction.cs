
namespace AiWorkspace.Domain.Ledger;

public sealed class LedgerTransaction
{
    public Guid Id { get; private set; }

    public string IdempotencyKey { get; private set; } = string.Empty;

    public Guid SourceAccountId { get; private set; }

    public Guid DestinationAccountId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    private LedgerTransaction()
    {
    }

    public LedgerTransaction(
        Guid id,
        string idempotencyKey,
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "Transaction ID is required.",
                nameof(id));

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException(
                "Idempotency key is required.",
                nameof(idempotencyKey));

        if (idempotencyKey.Length > 128)
            throw new ArgumentException(
                "Idempotency key must not exceed 128 characters.",
                nameof(idempotencyKey));

        if (sourceAccountId == Guid.Empty ||
            destinationAccountId == Guid.Empty)
            throw new ArgumentException(
                "Valid account identifiers are required.");

        if (sourceAccountId == destinationAccountId)
            throw new ArgumentException(
                "Source and destination accounts must be different.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Transaction amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(currency) ||
            currency.Trim().Length != 3)
            throw new ArgumentException(
                "Currency must be a three-letter code.",
                nameof(currency));

        Id = id;
        IdempotencyKey = idempotencyKey;
        SourceAccountId = sourceAccountId;
        DestinationAccountId = destinationAccountId;
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
