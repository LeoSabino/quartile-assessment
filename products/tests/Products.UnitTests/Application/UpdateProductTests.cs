using Products.Functions.Application;
using Products.Functions.Domain;
using Products.UnitTests.Fakes;

namespace Products.UnitTests.Application;

public class UpdateProductTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductService _service;

    public UpdateProductTests()
    {
        _service = new ProductService(_products);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesProduct()
    {
        var product = await CreateProductAsync();

        var result = await _service.UpdateAsync(product.Id, "ABC-1", "Espresso", "Dark roast", 12.90m, 7);

        Assert.True(result.IsSuccess);
        Assert.Equal("Espresso", result.Value.Name);
        Assert.Equal(12.90m, result.Value.Price);
        Assert.Single(_products.UpdatedProducts);
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentProductCode_ReturnsValidationError()
    {
        var product = await CreateProductAsync();

        var result = await _service.UpdateAsync(product.Id, "XYZ-9", "Espresso", null, 12.90m, 7);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Equal("Product.ProductCodeCannotChange", result.Error.Code);
        Assert.Empty(_products.UpdatedProducts);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ReturnsValidationError()
    {
        var product = await CreateProductAsync();

        var result = await _service.UpdateAsync(product.Id, "ABC-1", "Espresso", null, 12.90m, -1);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.InvalidQuantity", result.Error.Code);
        Assert.Empty(_products.UpdatedProducts);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.UpdateAsync(Guid.NewGuid(), "ABC-1", "Espresso", null, 12.90m, 7);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    private async Task<Product> CreateProductAsync()
    {
        var storeId = _products.AddActiveStore();
        return (await _service.CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 3)).Value;
    }
}
