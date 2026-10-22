using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Products.Functions.Application;
using Products.Functions.Persistence;

namespace Products.IntegrationTests;

public class ProductsDatabase : IAsyncLifetime
{
    private const string DatabaseName = "Products_Tests";
    private const string Server = "Server=(localdb)\\MSSQLLocalDB;Trusted_Connection=True;TrustServerCertificate=True";
    private const string ConnectionString = $"{Server};Database={DatabaseName}";

    private const string StoresTables =
        """
        CREATE TABLE dbo.Companies
        (
            Id       UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
            Name     NVARCHAR(200)    NOT NULL,
            IsActive BIT              NOT NULL
        );

        CREATE TABLE dbo.Stores
        (
            Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
            CompanyId    UNIQUEIDENTIFIER NOT NULL REFERENCES dbo.Companies (Id),
            Name         NVARCHAR(200)    NOT NULL,
            CountryCode  CHAR(2)          NOT NULL,
            CurrencyCode CHAR(3)          NOT NULL,
            IsActive     BIT              NOT NULL
        );

        CREATE UNIQUE INDEX UX_Stores_Id_CurrencyCode ON dbo.Stores (Id, CurrencyCode);
        """;

    private ServiceProvider _services = null!;

    public ProductService NewService() =>
        _services.CreateScope().ServiceProvider.GetRequiredService<ProductService>();

    public async Task InitializeAsync()
    {
        await ExecuteAsync(
            $"{Server};Database=master",
            $"""
            IF DB_ID(N'{DatabaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{DatabaseName}];
            END;
            CREATE DATABASE [{DatabaseName}];
            """);

        await ExecuteAsync(ConnectionString, StoresTables);

        foreach (var script in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "database"), "*.sql").Order())
            foreach (var batch in Regex.Split(await File.ReadAllTextAsync(script), @"^\s*GO\s*$", RegexOptions.Multiline))
                if (!string.IsNullOrWhiteSpace(batch))
                    await ExecuteAsync(ConnectionString, batch);

        _services = new ServiceCollection().AddProducts(ConnectionString).BuildServiceProvider();
    }

    public Task DisposeAsync() => _services.DisposeAsync().AsTask();

    public async Task<Guid> AddStoreAsync(string currencyCode = "BRL", bool storeActive = true, bool companyActive = true)
    {
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(
            """
            INSERT dbo.Companies (Id, Name, IsActive) VALUES (@CompanyId, N'Company', @CompanyActive);
            INSERT dbo.Stores (Id, CompanyId, Name, CountryCode, CurrencyCode, IsActive)
            VALUES (@StoreId, @CompanyId, N'Store', 'BR', @CurrencyCode, @StoreActive);
            """,
            connection);
        command.Parameters.AddWithValue("@CompanyId", companyId);
        command.Parameters.AddWithValue("@StoreId", storeId);
        command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
        command.Parameters.AddWithValue("@StoreActive", storeActive);
        command.Parameters.AddWithValue("@CompanyActive", companyActive);
        await command.ExecuteNonQueryAsync();

        return storeId;
    }

    public async Task SetStoreActiveAsync(Guid storeId, bool isActive)
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand("UPDATE dbo.Stores SET IsActive = @IsActive WHERE Id = @StoreId;", connection);
        command.Parameters.AddWithValue("@StoreId", storeId);
        command.Parameters.AddWithValue("@IsActive", isActive);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteAsync(string connectionString, string sql)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<ProductsDatabase>;
