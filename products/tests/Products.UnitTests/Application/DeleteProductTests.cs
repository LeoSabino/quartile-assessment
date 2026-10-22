using Products.Functions.Application;
using Products.UnitTests.Fakes;

namespace Products.UnitTests.Application;

public class DeleteProductTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductService _service;

    public DeleteProductTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_DeactivatesProduct()
    {
        var storeId = _products.AddActiveStore();
        var product = (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;

        var result = await _service.DeleteAsync(product.Id);

        Assert.True(result.IsSuccess);
        Assert.False(product.IsActive);
        Assert.Single(_products.UpdatedProducts);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductIsAlreadyDeleted_ReturnsNotFound()
    {
        var storeId = _products.AddActiveStore();
        var product = (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;
        await _service.DeleteAsync(product.Id);

        var result = await _service.DeleteAsync(product.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_AllowsReusingTheProductCode()
    {
        var storeId = _products.AddActiveStore();
        var product = (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;
        await _service.DeleteAsync(product.Id);

        var result = await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0);

        Assert.True(result.IsSuccess);
    }
}
