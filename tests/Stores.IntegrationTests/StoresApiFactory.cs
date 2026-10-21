using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stores.Infrastructure.Persistence;

namespace Stores.IntegrationTests;

public class StoresApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string ApiKeyHeader = "X-Api-Key";
    public const string TestsApiKey = "tests-key";
    public const string QuartileApiKey = "quartile-key";

    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=Quartile_Tests;Trusted_Connection=True;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Stores", ConnectionString);
        builder.UseSetting("ApiKeys:tests", TestsApiKey);
        builder.UseSetting("ApiKeys:quartile", QuartileApiKey);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        base.ConfigureClient(client);
        client.DefaultRequestHeaders.Add(ApiKeyHeader, TestsApiKey);
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StoresDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<StoresApiFactory>;
