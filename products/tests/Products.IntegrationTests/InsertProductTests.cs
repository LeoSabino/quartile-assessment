namespace Products.IntegrationTests;

// POST goes through dbo.InsertProduct.
[Collection("Database")]
public class InsertProductTests
{
    private readonly ProductsDatabase _database;

    public InsertProductTests(ProductsDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task Create_CopiesCurrencyFromStore()
    {
        var storeId = await _database.AddStoreAsync(currencyCode: "USD");

        var result = await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", "Arabica", 10.5m, 3);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("USD", result.Value.CurrencyCode);
        Assert.Equal(10.50m, result.Value.Price);
        Assert.Equal(3, result.Value.Quantity);
    }

    [Fact]
    public async Task Create_WhenStoreDoesNotExist_ReturnsStoreNotFound()
    {
        var result = await _database.NewService().CreateAsync(Guid.NewGuid(), "ABC-1", "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Create_WhenStoreOrCompanyIsInactive_ReturnsStoreNotFound(bool storeActive, bool companyActive)
    {
        var storeId = await _database.AddStoreAsync(storeActive: storeActive, companyActive: companyActive);

        var result = await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.StoreNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Create_WithCodeInUseInOtherCase_ReturnsConflict()
    {
        var storeId = await _database.AddStoreAsync();
        await _database.NewService().CreateAsync(storeId, "abc-1", "Coffee", null, 10m, 0);

        var result = await _database.NewService().CreateAsync(storeId, "ABC-1", "Espresso", null, 12m, 0);

        Assert.True(result.IsFailure);
        Assert.Equal("Product.ProductCodeAlreadyInUse", result.Error.Code);
    }

    [Fact]
    public async Task Create_WithCodeOfDeletedProduct_ReturnsSuccess()
    {
        var storeId = await _database.AddStoreAsync();
        var deleted = await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0);
        await _database.NewService().DeleteAsync(deleted.Value.Id);

        var result = await _database.NewService().CreateAsync(storeId, "ABC-1", "Coffee", null, 10m, 0);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(deleted.Value.Id, result.Value.Id);
    }

    [Fact]
    public async Task Create_WithCodeUsedByAnotherStore_ReturnsSuccess()
    {
        var firstStore = await _database.AddStoreAsync();
        var secondStore = await _database.AddStoreAsync();
        await _database.NewService().CreateAsync(firstStore, "ABC-1", "Coffee", null, 10m, 0);

        var result = await _database.NewService().CreateAsync(secondStore, "ABC-1", "Coffee", null, 11m, 0);

        Assert.True(result.IsSuccess);
    }
}
