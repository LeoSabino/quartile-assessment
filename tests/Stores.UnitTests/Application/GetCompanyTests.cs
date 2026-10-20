using Stores.Application;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class GetCompanyTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly CompanyService _service;

    public GetCompanyTests()
    {
        _service = new CompanyService(_companies, _stores);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyExists_ReturnsCompany()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.GetAsync(companyId);

        Assert.True(result.IsSuccess);
        Assert.Equal(companyId, result.Value.Id);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.GetAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task GetAsync_WhenCompanyIsDeleted_ReturnsNotFound()
    {
        var companyId = _companies.AddActiveCompany();
        await _service.DeleteAsync(companyId);

        var result = await _service.GetAsync(companyId);

        Assert.True(result.IsFailure);
        Assert.Equal("Company.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task ListAsync_ReturnsOnlyActiveCompanies()
    {
        var active = _companies.AddActiveCompany();
        var deleted = _companies.AddActiveCompany();
        await _service.DeleteAsync(deleted);

        var companies = await _service.ListAsync();

        var company = Assert.Single(companies);
        Assert.Equal(active, company.Id);
    }
}
