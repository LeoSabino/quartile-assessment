using Stores.Application;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class ListStoresTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StoreService _service;

    public ListStoresTests()
    {
        _service = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task ListAsync_ReturnsOnlyStoresOfTheCompany()
    {
        var companyA = _companies.AddActiveCompany();
        var companyB = _companies.AddActiveCompany();
        await _service.CreateAsync(companyA, "Downtown", "US", "USD");
        await _service.CreateAsync(companyA, "Uptown", "US", "USD");
        await _service.CreateAsync(companyB, "GremioMania", "BR", "BRL");

        var result = await _service.ListAsync(companyA);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, s => Assert.Equal(companyA, s.CompanyId));
    }

    [Fact]
    public async Task ListAsync_DoesNotReturnDeletedStores()
    {
        var companyId = _companies.AddActiveCompany();
        var deleted = (await _service.CreateAsync(companyId, "Downtown", "US", "USD")).Value;
        await _service.CreateAsync(companyId, "Uptown", "US", "USD");
        deleted.Deactivate();

        var result = await _service.ListAsync(companyId);

        var store = Assert.Single(result.Value);
        Assert.Equal("Uptown", store.Name);
    }

    [Fact]
    public async Task ListAsync_WhenCompanyHasNoStores_ReturnsEmptyList()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.ListAsync(companyId);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task ListAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.ListAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }
}
