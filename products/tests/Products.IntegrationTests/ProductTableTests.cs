using System.Globalization;

namespace Products.IntegrationTests;

[Collection("Database")]
public class ProductTableTests
{
    private readonly ProductsDatabase _database;

    public ProductTableTests(ProductsDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task Update_ChangesProductAndKeepsTwoDecimalPlaces()
    {
        var storeId = await _database.AddStoreAsync();
        var product = (await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 3)).Value;

        var updated = await _database.NewService().UpdateAsync(product.Id, "abc-1", "Espresso", "Dark roast", 12.9m, 7);
        var reread = await _database.NewService().GetAsync(product.Id);

        Assert.True(updated.IsSuccess);
        Assert.Equal("Espresso", reread.Value.Name);
        Assert.Equal("Dark roast", reread.Value.Description);
        Assert.Equal("12.90", reread.Value.Price.ToString(CultureInfo.InvariantCulture));
        Assert.Equal(7, reread.Value.Quantity);
        Assert.Equal("ABC-1", reread.Value.ProductCode);
    }

    [Fact]
    public async Task Get_WhenStoreIsDeactivated_ReturnsNotFound()
    {
        var storeId = await _database.AddStoreAsync();
        var product = (await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;
        await _database.SetStoreActiveAsync(storeId, false);

        var result = await _database.NewService().GetAsync(product.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Delete_HidesProduct()
    {
        var storeId = await _database.AddStoreAsync();
        var product = (await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0)).Value;

        var deleted = await _database.NewService().DeleteAsync(product.Id);
        var result = await _database.NewService().GetAsync(product.Id);

        Assert.True(deleted.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Product.NotFound", result.Error.Code);
    }
}
