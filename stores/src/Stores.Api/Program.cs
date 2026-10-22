using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Stores.Api.Authentication;
using Stores.Application;
using Stores.Infrastructure;
using Stores.Infrastructure.Persistence;

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

builder.Services.AddHealthChecks().AddDbContextCheck<StoresDbContext>(tags: ["db"]);

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
// Anonymous liveness doesn't touch the database, so it can't keep the serverless database awake.
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("db") });

app.Run();
