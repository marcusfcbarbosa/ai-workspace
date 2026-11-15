
using System.Text.RegularExpressions;

namespace AiWorkspace.Api.Controllers;

public static class BankingIntentRouter
{
    private static readonly Regex TransferWords = new(
        @"\b(transfer|send money|wire|remit|move funds|payment|pay someone)\b",
        RegexOptions.IgnoreCase |
        RegexOptions.CultureInvariant |
        RegexOptions.Compiled);

    private static readonly Regex OnboardingWords = new(
        @"\b(open|create|register|apply for|set up|start)\b.{0,90}\b(account)\b|\b(account opening|onboarding|business account requirements|personal account requirements)\b",
        RegexOptions.IgnoreCase |
        RegexOptions.CultureInvariant |
        RegexOptions.Compiled);

    public static bool IsTransferIntent(string message)
    {
        return !string.IsNullOrWhiteSpace(message) &&
               TransferWords.IsMatch(message);
    }

    public static bool IsOnboardingIntent(string message)
    {
        return !string.IsNullOrWhiteSpace(message) &&
               OnboardingWords.IsMatch(message);
    }
}
