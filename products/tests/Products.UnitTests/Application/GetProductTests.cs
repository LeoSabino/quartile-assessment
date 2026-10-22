using Products.Functions.Application;
using Products.Functions.Domain;
using Products.UnitTests.Fakes;

namespace Products.UnitTests.Application;

public class GetProductTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductService _service;

    public GetProductTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task GetAsync_WhenProductExists_ReturnsProduct()
    {
        var storeId = _products.AddActiveStore();
        var created = (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;

        var result = await _service.GetAsync(created.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(created.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.GetAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_WhenProductIsDeleted_ReturnsNotFound()
    {
        var storeId = _products.AddActiveStore();
        var created = (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;
        await _service.DeleteAsync(created.Id);

        var result = await _service.GetAsync(created.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }
}
