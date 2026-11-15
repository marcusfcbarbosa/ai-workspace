
using AiWorkspace.Application.Interfaces;
using AiWorkspace.Application.Transfers;

namespace AiWorkspace.Application.Orchestration;

public sealed class BankingOrchestrator : IBankingOrchestrator
{
    private readonly IAccountAgent _accountAgent;
    private readonly IFraudAgent _fraudAgent;
    private readonly IComplianceAgent _complianceAgent;
    private readonly ILedgerAgent _ledgerAgent;

    public BankingOrchestrator(
        IAccountAgent accountAgent,
        IFraudAgent fraudAgent,
        IComplianceAgent complianceAgent,
        ILedgerAgent ledgerAgent)
    {
        _accountAgent = accountAgent;
        _fraudAgent = fraudAgent;
        _complianceAgent = complianceAgent;
        _ledgerAgent = ledgerAgent;
    }

    public async Task<TransferResponse> ExecuteTransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.SourceAccountId == Guid.Empty ||
            request.DestinationAccountId == Guid.Empty)
        {
            return Reject(request, "Invalid account identifier.");
        }

        if (request.SourceAccountId == request.DestinationAccountId)
        {
            return Reject(
                request,
                "Source and destination accounts must be different.");
        }

        if (request.Amount <= 0)
        {
            return Reject(
                request,
                "Transfer amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency) ||
            string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            return Reject(
                request,
                "Currency and idempotency key are required.");
        }

        AccountValidationResult accountValidation =
            await _accountAgent.ValidateTransferAccountsAsync(
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Currency,
                cancellationToken);

        if (!accountValidation.IsValid)
        {
            return Reject(request, accountValidation.Message);
        }

        FraudEvaluationResult fraudEvaluation =
            await _fraudAgent.EvaluateTransferAsync(
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Currency,
                cancellationToken);

        if (fraudEvaluation.Decision != FraudDecision.Approved)
        {
            return Reject(request, fraudEvaluation.Message);
        }

        ComplianceEvaluationResult complianceEvaluation =
            await _complianceAgent.EvaluateTransferAsync(
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Currency,
                cancellationToken);

        if (complianceEvaluation.Decision != ComplianceDecision.Approved)
        {
            return Reject(request, complianceEvaluation.Message);
        }

        Guid transactionId = Guid.NewGuid();

        LedgerPostingResult ledgerResult =
            await _ledgerAgent.PostTransferAsync(
                transactionId,
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Currency,
                request.IdempotencyKey,
                cancellationToken);

        if (!ledgerResult.Success)
        {
            return new TransferResponse(
                transactionId,
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Currency,
                TransferStatus.Failed,
                ledgerResult.Message,
                DateTimeOffset.UtcNow);
        }

        return new TransferResponse(
            transactionId,
            request.SourceAccountId,
            request.DestinationAccountId,
            request.Amount,
            request.Currency,
            TransferStatus.Completed,
            "Transfer completed successfully.",
            ledgerResult.PostedAt ?? DateTimeOffset.UtcNow);
    }

    private static TransferResponse Reject(
        TransferRequest request,
        string message)
    {
        return new TransferResponse(
            Guid.NewGuid(),
            request.SourceAccountId,
            request.DestinationAccountId,
            request.Amount,
            request.Currency,
            TransferStatus.Rejected,
            message,
            DateTimeOffset.UtcNow);
    }
}
