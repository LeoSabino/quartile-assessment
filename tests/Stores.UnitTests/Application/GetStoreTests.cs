using Stores.Application;
using Stores.Domain;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class GetStoreTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StoreService _service;

    public GetStoreTests()
    {
        _service = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task GetAsync_WhenStoreExists_ReturnsStore()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;

        var result = await _service.GetAsync(companyId, store.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(store.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetAsync_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.GetAsync(companyId, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_WhenStoreBelongsToAnotherCompany_ReturnsNotFound()
    {
        var companyA = _companies.AddActiveCompany();
        var companyB = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyA, "Downtown", "US", "USD")).Value;

        var result = await _service.GetAsync(companyB, store.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_WhenStoreIsDeleted_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;
        store.Deactivate();

        var result = await _service.GetAsync(companyId, store.Id);

        Assert.True(result.IsFailure);
        Assert.Equal("Store.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.GetAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }
}
