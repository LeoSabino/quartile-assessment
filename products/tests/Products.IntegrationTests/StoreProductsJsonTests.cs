using System.Text.Json;

namespace Products.IntegrationTests;

[Collection("Database")]
public class StoreProductsJsonTests
{
    private readonly ProductsDatabase _database;

    public StoreProductsJsonTests(ProductsDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task List_ReturnsActiveProductsOrderedByCode()
    {
        var storeId = await _database.AddStoreAsync(currencyCode: "BRL");
        var service = _database.NewService();
        var coffee = (await service.CreateAsync(storeId, "B-2", "Coffee", null, 10.5m, 3)).Value;
        await service.CreateAsync(storeId, "A-1", "Tea", "Green", 5m, 1);
        var deleted = (await service.CreateAsync(storeId, "C-3", "Old", null, 1m, 0)).Value;
        await _database.NewService().DeleteAsync(deleted.Id);

        var result = await _database.NewService().ListByStoreAsync(storeId);

        Assert.True(result.IsSuccess);
        using var json = JsonDocument.Parse(result.Value);
        var root = json.RootElement;
        Assert.Equal(storeId.ToString(), root.GetProperty("storeId").GetString());
        Assert.Equal("BRL", root.GetProperty("currencyCode").GetString());

        var products = root.GetProperty("products").EnumerateArray().ToList();
        Assert.Equal(["A-1", "B-2"], products.Select(p => p.GetProperty("productCode").GetString()));
        Assert.Equal(coffee.Id.ToString(), products[1].GetProperty("id").GetString());
        Assert.Equal(10.50m, products[1].GetProperty("price").GetDecimal());
        Assert.Equal(JsonValueKind.Null, products[1].GetProperty("description").ValueKind);
    }

    [Fact]
    public async Task List_WhenStoreHasNoProducts_ReturnsEmptyArray()
    {
        var storeId = await _database.AddStoreAsync();

        var result = await _database.NewService().ListByStoreAsync(storeId);

        Assert.True(result.IsSuccess);
        using var json = JsonDocument.Parse(result.Value);
        Assert.Equal(0, json.RootElement.GetProperty("products").GetArrayLength());
    }

    [Fact]
    public async Task List_WhenStoreDoesNotExist_ReturnsStoreNotFound()
    {
        var result = await _database.NewService().ListByStoreAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }

    [Fact]
    public async Task List_WhenCompanyIsInactive_ReturnsStoreNotFound()
    {
        var storeId = await _database.AddStoreAsync(companyActive: false);

        var result = await _database.NewService().ListByStoreAsync(storeId);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }
}
