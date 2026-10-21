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
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<ApiKeySecurityTransformer>());

var app = builder.Build();

app.MapOpenApi().AllowAnonymous();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Stores API v1"));
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous().ExcludeFromDescription();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
