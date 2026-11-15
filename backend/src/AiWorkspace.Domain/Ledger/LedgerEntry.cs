
namespace AiWorkspace.Domain.Ledger;

public sealed class LedgerEntry
{
    public Guid Id { get; private set; }

    public Guid TransactionId { get; private set; }

    public Guid AccountId { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public LedgerEntryType EntryType { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private LedgerEntry()
    {
    }

    public LedgerEntry(
        Guid transactionId,
        Guid accountId,
        decimal amount,
        string currency,
        LedgerEntryType entryType)
    {
        if (transactionId == Guid.Empty)
            throw new ArgumentException(
                "Transaction ID is required.",
                nameof(transactionId));

        if (accountId == Guid.Empty)
            throw new ArgumentException(
                "Account ID is required.",
                nameof(accountId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Ledger entry amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.",
                nameof(currency));

        if (!Enum.IsDefined(entryType))
            throw new ArgumentOutOfRangeException(
                nameof(entryType),
                "Invalid ledger entry type.");

        Id = Guid.NewGuid();
        TransactionId = transactionId;
        AccountId = accountId;
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
        EntryType = entryType;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}

public enum LedgerEntryType
{
    Debit = 1,
    Credit = 2
}
