using Microsoft.AspNetCore.Authorization;
using Stores.Api.Authentication;
using Stores.Application;
using Stores.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Stores")
    ?? throw new InvalidOperationException("Connection string 'Stores' is not configured.");

var apiKeys = builder.Configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>();
if (apiKeys is null || apiKeys.Count == 0)
    throw new InvalidOperationException("No API keys configured in 'ApiKeys'.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

builder.Services
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<ApiKeyOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName,
        options => options.Clients = apiKeys);

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
