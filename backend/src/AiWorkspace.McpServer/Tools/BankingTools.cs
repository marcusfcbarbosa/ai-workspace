using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AiWorkspace.McpServer.Tools;

[McpServerToolType]
public static class BankingTools
{
    [McpServerTool]
    [Description("Returns the banking operations supported by the workspace.")]
    public static object GetBankingCapabilities()
    {
        return new
        {
            Service = "AI Banking Workspace",
            ExecutionMode = "Deterministic",
            LlmEnabled = false,
            Operations = new[]
            {
                "AccountOpening",
                "AccountBalance",
                "InternalTransfer",
                "FraudEvaluation",
                "TransactionHistory",
                "PaymentProcessing",
                "TransactionReversal",
                "AccountBlocking"
            }
        };
    }
}
