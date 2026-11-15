namespace AiWorkspace.Application.Transfers;

public sealed record TransferResponse(
    Guid TransactionId,
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Currency,
    TransferStatus Status,
    string Message,
    DateTimeOffset CreatedAt
);

public enum TransferStatus
{
    Pending,
    Approved,
    Rejected,
    Completed,
    Failed
}
