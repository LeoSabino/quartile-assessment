using Stores.Application;
using Stores.Domain;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class DeleteCompanyTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly CompanyService _service;
    private readonly StoreService _storeService;

    public DeleteCompanyTests()
    {
        _service = new CompanyService(_companies, _stores);
        _storeService = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyHasNoStores_DeactivatesCompany()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.DeleteAsync(companyId);

        Assert.True(result.IsSuccess);
        Assert.False(_companies.Companies.Single().IsActive);
        Assert.Single(_companies.UpdatedCompanies);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyHasOnlyDeletedStores_DeactivatesCompany()
    {
        var companyId = _companies.AddActiveCompany();
        var store = (await _storeService.CreateAsync(companyId, "Downtown", "US", "USD")).Value;
        await _storeService.DeleteAsync(companyId, store.Id);

        var result = await _service.DeleteAsync(companyId);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyHasActiveStores_ReturnsConflict()
    {
        var companyId = _companies.AddActiveCompany();
        await _storeService.CreateAsync(companyId, "Downtown", "US", "USD");

        var result = await _service.DeleteAsync(companyId);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Company.HasActiveStores", result.Error.Code);
        Assert.True(_companies.Companies.Single().IsActive);
        Assert.Empty(_companies.UpdatedCompanies);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task DeleteAsync_WhenCompanyIsAlreadyDeleted_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();
        await _service.DeleteAsync(companyId);

        var result = await _service.DeleteAsync(companyId);

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }
}
