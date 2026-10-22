using Products.Functions.Application.Ports;
using Products.Functions.Domain;

namespace Products.Functions.Application;

public class ProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<Result<Product>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return Result<Product>.Failure(ProductErrors.NotFound);

        return Result<Product>.Success(product);
    }

    public async Task<Result<string>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken = default)
    {
        var json = await _products.GetStoreProductsJsonAsync(storeId, cancellationToken);
        if (json is null)
            return Result<string>.Failure(ProductErrors.StoreNotFound);

        return Result<string>.Success(json);
    }

    public async Task<Result<Product>> CreateAsync(
        Guid storeId,
        string productCode,
        string name,
        string? description,
        decimal price,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var result = Product.Create(storeId, productCode, name, description, price, quantity);
        if (result.IsFailure)
            return result;

        var added = await _products.AddAsync(result.Value, cancellationToken);
        if (added.IsFailure)
            return Result<Product>.Failure(added.Error);

        return await GetAsync(added.Value, cancellationToken);
    }

    public async Task<Result<Product>> UpdateAsync(
        Guid id,
        string productCode,
        string name,
        string? description,
        decimal price,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(id, cancellationToken);
        if (found.IsFailure)
            return found;

        var product = found.Value;

        var updated = product.Update(productCode, name, description, price, quantity);
        if (updated.IsFailure)
            return Result<Product>.Failure(updated.Error);

        var saved = await _products.UpdateAsync(product, cancellationToken);
        if (saved.IsFailure)
            return Result<Product>.Failure(saved.Error);

        return found;
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(id, cancellationToken);
        if (found.IsFailure)
            return Result.Failure(found.Error);

        found.Value.Deactivate();
        return await _products.UpdateAsync(found.Value, cancellationToken);
    }
}
