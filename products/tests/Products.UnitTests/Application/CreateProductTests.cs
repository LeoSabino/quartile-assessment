using Products.Functions.Application;
using Products.Functions.Domain;
using Products.UnitTests.Fakes;

namespace Products.UnitTests.Application;

public class CreateProductTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductService _service;

    public CreateProductTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedProduct()
    {
        var storeId = _products.AddActiveStore("BRL");

        var result = await _service.CreateAsync(storeId, "ABC-1", "Coffee", "Arabica 500g", 10.50m, 3);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(storeId, result.Value.StoreId);
        Assert.Equal("ABC-1", result.Value.ProductCode);
        Assert.Equal("BRL", result.Value.CurrencyCode);
        Assert.Single(_products.Products);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ReturnsValidationError()
    {
        var storeId = _products.AddActiveStore();

        var result = await _service.CreateAsync(storeId, "ABC-1", "", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.NameRequired", result.Error.Code);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_DoesNotCallRepository()
    {
        var storeId = _products.AddActiveStore();

        await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, -1m, 0);

        Assert.Equal(0, _products.AddCalls);
    }

    [Fact]
    public async Task CreateAsync_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.CreateAsync(Guid.NewGuid(), "ABC-1", "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }

    [Fact]
    public async Task CreateAsync_WithProductCodeInUseInTheStore_ReturnsConflict()
    {
        var storeId = _products.AddActiveStore();
        await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0);

        var result = await _service.CreateAsync(storeId, "abc-1", "Espresso", null, 12m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Product.ProductCodeAlreadyInUse", result.Error.Code);
    }

    [Fact]
    public async Task CreateAsync_WithSameProductCodeInAnotherStore_ReturnsSuccess()
    {
        var downtown = _products.AddActiveStore();
        var airport = _products.AddActiveStore();
        await _service.CreateAsync(downtown, "ABC-1", "Coffee", null, 10m, 0);

        var result = await _service.CreateAsync(airport, "ABC-1", "Coffee", null, 11m, 0);

        Assert.True(result.IsSuccess);
    }
}
