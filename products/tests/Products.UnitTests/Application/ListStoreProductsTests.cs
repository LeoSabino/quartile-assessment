using Products.Functions.Application;
using Products.Functions.Domain;
using Products.UnitTests.Fakes;

namespace Products.UnitTests.Application;

public class ListStoreProductsTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductService _service;

    public ListStoreProductsTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task ListByStoreAsync_WhenStoreExists_ReturnsJsonFromDatabase()
    {
        var storeId = _products.AddActiveStore();

        var result = await _service.ListByStoreAsync(storeId);

        Assert.True(result.IsSuccess);
        Assert.Equal(FakeProductRepository.StoreProductsJson, result.Value);
    }

    [Fact]
    public async Task ListByStoreAsync_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.ListByStoreAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }
}
