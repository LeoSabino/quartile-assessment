using Microsoft.EntityFrameworkCore;
using Stores.Infrastructure.Persistence;

namespace Stores.IntegrationTests.Contracts;

// This keeps only what the Stores API depends on: the composite FK to Stores(Id, CurrencyCode).
public static class ProductsTableContract
{
    public static Task EnsureCreatedAsync(StoresDbContext db) =>
        db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
                CREATE TABLE dbo.Products
                (
                    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Products PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
                    StoreId      UNIQUEIDENTIFIER NOT NULL,
                    CurrencyCode CHAR(3)          NOT NULL,

                    CONSTRAINT FK_Products_Stores_StoreId_CurrencyCode
                        FOREIGN KEY (StoreId, CurrencyCode) REFERENCES dbo.Stores (Id, CurrencyCode)
                );
            """);

    public static Task AddProductAsync(StoresDbContext db, Guid storeId, string currencyCode) =>
        db.Database.ExecuteSqlAsync(
            $"INSERT INTO dbo.Products (StoreId, CurrencyCode) VALUES ({storeId}, {currencyCode})");
}
