namespace AiWorkspace.Application.Interfaces;

public interface IAccountAgent
{
    Task<AccountValidationResult> ValidateTransferAccountsAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default);
}

public sealed record AccountValidationResult(
    bool IsValid,
    string Code,
    string Message
);
