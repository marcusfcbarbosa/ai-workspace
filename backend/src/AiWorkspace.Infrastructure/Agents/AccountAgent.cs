using AiWorkspace.Application.Interfaces;

namespace AiWorkspace.Infrastructure.Agents;

public sealed class AccountAgent : IAccountAgent
{
    public Task<AccountValidationResult> ValidateTransferAccountsAsync(
        Guid sourceAccountId,
        Guid destinationAccountId,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new AccountValidationResult(
            IsValid: false,
            Code: "ACCOUNT_VALIDATION_UNAVAILABLE",
            Message: "Account validation is unavailable. Transfer processing is blocked."
        ));
    }
}
