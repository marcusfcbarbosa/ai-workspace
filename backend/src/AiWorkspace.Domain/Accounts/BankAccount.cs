
namespace AiWorkspace.Domain.Accounts;

public sealed class BankAccount
{
    public Guid Id { get; private set; }

    public Guid OwnerId { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public decimal Balance { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private BankAccount()
    {
    }

    public BankAccount(
        Guid id,
        Guid ownerId,
        string currency)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Account ID is required.",
                nameof(id));
        }

        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Account owner ID is required.",
                nameof(ownerId));
        }

        if (string.IsNullOrWhiteSpace(currency) ||
            currency.Trim().Length != 3)
        {
            throw new ArgumentException(
                "Currency must be a three-letter code.",
                nameof(currency));
        }

        Id = id;
        OwnerId = ownerId;
        Currency = currency.Trim().ToUpperInvariant();
        Balance = 0m;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
