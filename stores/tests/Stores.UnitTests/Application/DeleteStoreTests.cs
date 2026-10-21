using Stores.Application;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class DeleteStoreTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StoreService _service;

    public DeleteStoreTests()
    {
        _service = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreExists_DeactivatesStore()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;

        var result = await _service.DeleteAsync(companyId, store.Id);

        Assert.True(result.IsSuccess);
        Assert.False(store.IsActive);
        Assert.Single(_stores.UpdatedStores);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreIsAlreadyDeleted_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;
        await _service.DeleteAsync(companyId, store.Id);

        var result = await _service.DeleteAsync(companyId, store.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreBelongsToAnotherCompany_ReturnsNotFound()
    {
        var companyA = _companies.AddActiveCompany();
        var companyB = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyA, "Downtown", "US", "USD")).Value;

        var result = await _service.DeleteAsync(companyB, store.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
        Assert.True(store.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.DeleteAsync(companyId, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }
}
