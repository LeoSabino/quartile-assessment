using Products.Functions.Domain;

namespace Products.Functions.Application.Ports;

public interface IProductRepository
{
    Task<Result<Guid>> AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<string?> GetStoreProductsJsonAsync(Guid storeId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken);
}
