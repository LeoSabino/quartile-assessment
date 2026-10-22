using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Products.Functions.Application.Ports;
using Products.Functions.Domain;

namespace Products.Functions.Persistence;

internal class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _db;

    public ProductRepository(ProductsDbContext db)
    {
        _db = db;
    }

    public async Task<Result<Guid>> AddAsync(Product product, CancellationToken cancellationToken)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        try
        {
            var id = await strategy.ExecuteAsync(ct => InsertAsync(product, ct), cancellationToken);
            return id is Guid productId
                ? Result<Guid>.Success(productId)
                : Result<Guid>.Failure(ProductErrors.StoreNotFound);
        }
        catch (SqlException ex) when (SqlErrors.ToError(ex) is { } error)
        {
            return Result<Guid>.Failure(error);
        }
    }

    private async Task<object?> InsertAsync(Product product, CancellationToken cancellationToken)
    {
        await using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "dbo.InsertProduct";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddRange(new[]
        {
            new SqlParameter("@StoreId", SqlDbType.UniqueIdentifier) { Value = product.StoreId },
            new SqlParameter("@ProductCode", SqlDbType.NVarChar, Product.ProductCodeMaxLength) { Value = product.ProductCode },
            new SqlParameter("@Name", SqlDbType.NVarChar, Product.NameMaxLength) { Value = product.Name },
            new SqlParameter("@Description", SqlDbType.NVarChar, Product.DescriptionMaxLength) { Value = (object?)product.Description ?? DBNull.Value },
            new SqlParameter("@Price", SqlDbType.Decimal) { Precision = 18, Scale = Product.PriceDecimalPlaces, Value = product.Price },
            new SqlParameter("@Quantity", SqlDbType.Int) { Value = product.Quantity }
        });

        await _db.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            return await command.ExecuteScalarAsync(cancellationToken);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _db.Products.SingleOrDefaultAsync(
            p => p.Id == id && p.IsActive &&
                 _db.Stores.Any(s => s.Id == p.StoreId && s.IsActive &&
                                     _db.Companies.Any(c => c.Id == s.CompanyId && c.IsActive)),
            cancellationToken);

    public Task<string?> GetStoreProductsJsonAsync(Guid storeId, CancellationToken cancellationToken) =>
        _db.Database
            .SqlQuery<string?>($"SELECT dbo.GetProductsJson({storeId}) AS Value")
            .SingleAsync(cancellationToken);

    public async Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException &&
                                           SqlErrors.ToError(sqlException) is { } error)
        {
            return Result.Failure(error);
        }
    }
}
