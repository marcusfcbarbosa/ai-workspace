
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AiWorkspace.Api.Authentication;

public sealed class DevelopmentStaticTokenHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevelopmentStaticTokenHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsDevelopment())
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Development authentication is disabled."));
        }

        var configuredToken = Context.RequestServices
            .GetRequiredService<IConfiguration>()
            ["Authentication:DevelopmentStaticToken"];

        var authorization = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(configuredToken) ||
            !authorization.StartsWith(
                "Bearer ",
                StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        var providedToken = authorization[7..].Trim();

        var providedBytes = Encoding.UTF8.GetBytes(providedToken);
        var expectedBytes = Encoding.UTF8.GetBytes(configuredToken);

        if (providedBytes.Length != expectedBytes.Length ||
            !CryptographicOperations.FixedTimeEquals(
                providedBytes,
                expectedBytes))
        {
            return Task.FromResult(
                AuthenticateResult.Fail("Invalid development token."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "development-user"),
            new Claim(ClaimTypes.Name, "Development User"),
            new Claim("authentication_type", "development_static")
        };

        var identity = new ClaimsIdentity(
            claims,
            Scheme.Name);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            Scheme.Name);

        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }
}
