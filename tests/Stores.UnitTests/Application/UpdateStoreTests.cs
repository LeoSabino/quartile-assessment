using Stores.Application;
using Stores.Domain;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class UpdateStoreTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StoreService _service;

    public UpdateStoreTests()
    {
        _service = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesStore()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;

        var result = await _service.UpdateAsync(companyId, store.Id, "Uptown", "BR", "BRL");

        Assert.True(result.IsSuccess);
        Assert.Equal("Uptown", result.Value.Name);
        Assert.Equal("BRL", result.Value.CurrencyCode);
        Assert.Single(_stores.UpdatedStores);
    }

    [Fact]
    public async Task UpdateAsync_KeepingTheSameName_ReturnsSuccess()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;

        var result = await _service.UpdateAsync(companyId, store.Id, "Downtown", "BR", "BRL");

        Assert.True(result.IsSuccess);
        Assert.Equal("BRL", result.Value.CurrencyCode);
    }

    [Fact]
    public async Task UpdateAsync_WhenNameIsUsedByAnotherStore_ReturnsConflict()
    {
        var companyId = _companies.AddActiveCompany();
        await _service.CreateAsync(companyId, "Downtown", "US", "USD");
        var store = (await _service.CreateAsync(companyId, "Uptown", "US", "USD")).Value;

        var result = await _service.UpdateAsync(companyId, store.Id, "Downtown", "US", "USD");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Store.NameAlreadyInUse", result.Error.Code);
        Assert.Equal("Uptown", store.Name);
        Assert.Empty(_stores.UpdatedStores);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ReturnsValidationError()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;

        var result = await _service.UpdateAsync(companyId, store.Id, "", "US", "USD");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Empty(_stores.UpdatedStores);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_DoesNotCheckTheName()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;
        var checksBefore = _stores.NameChecks;

        await _service.UpdateAsync(companyId, store.Id, "", "US", "USD");

        Assert.Equal(checksBefore, _stores.NameChecks);
    }

    [Fact]
    public async Task UpdateAsync_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.UpdateAsync(companyId, Guid.NewGuid(), "Downtown", "US", "USD");

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }
}
