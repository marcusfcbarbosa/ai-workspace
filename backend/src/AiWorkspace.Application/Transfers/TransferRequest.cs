namespace AiWorkspace.Application.Transfers;

public sealed record TransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Currency,
    string IdempotencyKey
);
