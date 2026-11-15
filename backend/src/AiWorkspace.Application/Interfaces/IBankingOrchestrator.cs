using AiWorkspace.Application.Transfers;

namespace AiWorkspace.Application.Interfaces;

public interface IBankingOrchestrator
{
    Task<TransferResponse> ExecuteTransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default);
}
