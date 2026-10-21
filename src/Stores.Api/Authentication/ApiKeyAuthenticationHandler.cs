using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Stores.Api.Authentication;

public class ApiKeyOptions : AuthenticationSchemeOptions
{
    // client name -> api key
    public Dictionary<string, string> Clients { get; set; } = [];
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyOptions>
{
    public const string SchemeName = "ApiKey";
    public const string HeaderName = "X-Api-Key";

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var providedKey = Request.Headers[HeaderName].ToString();
        if (string.IsNullOrEmpty(providedKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        var client = FindClient(providedKey);
        if (client is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, client)], Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties) =>
        Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: $"A valid API key is required in the {HeaderName} header.")
            .ExecuteAsync(Context);

    private string? FindClient(string providedKey)
    {
        var provided = SHA256.HashData(Encoding.UTF8.GetBytes(providedKey));

        foreach (var (client, key) in Options.Clients)
        {
            var expected = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            if (CryptographicOperations.FixedTimeEquals(provided, expected))
                return client;
        }

        return null;
    }
}
