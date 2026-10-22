using Products.Functions.Application.Ports;
using Products.Functions.Domain;

namespace Products.UnitTests.Fakes;

public class FakeProductRepository : IProductRepository
{
    public const string StoreProductsJson = """{"products":[]}""";

    public Dictionary<Guid, string> ActiveStores { get; } = [];
    public List<Product> Products { get; } = [];
    public List<Product> UpdatedProducts { get; } = [];
    public int AddCalls { get; private set; }

    public Guid AddActiveStore(string currencyCode = "USD")
    {
        var storeId = Guid.NewGuid();
        ActiveStores[storeId] = currencyCode;
        return storeId;
    }

    public Task<Result<Guid>> AddAsync(Product product, CancellationToken cancellationToken)
    {
        AddCalls++;

        if (!ActiveStores.TryGetValue(product.StoreId, out var currencyCode))
            return Task.FromResult(Result<Guid>.Failure(ProductErrors.StoreNotFound));

        var codeInUse = Products.Any(p =>
            p.IsActive &&
            p.StoreId == product.StoreId &&
            string.Equals(p.ProductCode, product.ProductCode, StringComparison.OrdinalIgnoreCase));

        if (codeInUse)
            return Task.FromResult(Result<Guid>.Failure(ProductErrors.ProductCodeAlreadyInUse));

        var id = Guid.NewGuid();
        typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, id);
        typeof(Product).GetProperty(nameof(Product.CurrencyCode))!.SetValue(product, currencyCode);
        Products.Add(product);

        return Task.FromResult(Result<Guid>.Success(id));
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = Products.SingleOrDefault(p =>
            p.Id == id &&
            p.IsActive &&
            ActiveStores.ContainsKey(p.StoreId));

        return Task.FromResult(product);
    }

    public Task<string?> GetStoreProductsJsonAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var json = ActiveStores.ContainsKey(storeId) ? StoreProductsJson : null;
        return Task.FromResult(json);
    }

    public Task<Result> UpdateAsync(Product product, CancellationToken cancellationToken)
    {
        UpdatedProducts.Add(product);
        return Task.FromResult(Result.Success());
    }
}
