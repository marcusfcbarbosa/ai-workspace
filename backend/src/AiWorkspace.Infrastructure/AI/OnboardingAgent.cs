
using AiWorkspace.Application.Interfaces;

namespace AiWorkspace.Infrastructure.AI;

public sealed class OnboardingAgent : IOnboardingAgent
{
    public Task<string> ProcessAsync(
        string message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Message cannot be empty.",
                nameof(message));
        }

        var input = message.Trim().ToLowerInvariant();

        string response;

        if (ContainsAny(input,
            "business account",
            "business bank account",
            "company account",
            "corporate account"))
        {
            response = """
                To open a business bank account, you typically need:

                1. Registered company name and business registration number.
                2. Registered business address.
                3. Company ownership information.
                4. Identification of authorized representatives.
                5. Supporting business registration documents.

                Requirements vary by institution and jurisdiction.

                This is a demonstration application.
                No bank account has been created.
                """;
        }
        else if (ContainsAny(input,
            "personal account",
            "individual account",
            "checking account",
            "current account"))
        {
            response = """
                To open a personal bank account, you typically need:

                1. Your full legal name.
                2. A valid identification document.
                3. Your residential address.
                4. Contact information.
                5. Additional information required by the institution.

                This is a demonstration application.
                No identity verification has been performed.
                """;
        }
        else if (ContainsAny(input,
            "documents",
            "documentation",
            "requirements",
            "required information"))
        {
            response = """
                Account opening requirements depend on the account type.

                For personal accounts, identification and address
                information are commonly required.

                For business accounts, company registration,
                ownership and representative information may
                also be required.

                Would you like information about a personal
                or business account?
                """;
        }
        else if (ContainsAny(input,
            "status",
            "application progress",
            "track application"))
        {
            response = """
                This demonstration does not currently track
                account opening applications.

                No application status can be verified.

                A future workflow could retrieve application
                status from the banking database.
                """;
        }
        else if (ContainsAny(input,
            "hello",
            "hi",
            "good morning",
            "good afternoon"))
        {
            response = """
                Hello! Welcome to the banking onboarding demo.

                I can provide general information about
                personal accounts, business accounts,
                required documents and onboarding workflows.

                How can I help you today?
                """;
        }
        else
        {
            response = """
                I can help with general banking onboarding information.

                Available topics include:

                - Personal account opening
                - Business account opening
                - Required documentation
                - Onboarding process information

                Please specify which topic you would like
                to explore.

                This service uses predefined business rules
                and does not process requests using an LLM.
                """;
        }

        return Task.FromResult(response);
    }

    private static bool ContainsAny(
        string input,
        params string[] keywords)
    {
        return keywords.Any(keyword =>
            input.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase));
    }
}
