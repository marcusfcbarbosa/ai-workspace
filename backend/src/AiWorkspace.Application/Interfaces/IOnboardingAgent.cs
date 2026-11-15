
namespace AiWorkspace.Application.Interfaces;

public interface IOnboardingAgent
{
    Task<string> ProcessAsync(
        string message,
        CancellationToken cancellationToken = default);
}
