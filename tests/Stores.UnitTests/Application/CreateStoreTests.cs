using Stores.Application;
using Stores.Domain;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class CreateStoreTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly StoreService _service;

    public CreateStoreTests()
    {
        _service = new StoreService(_companies, _stores);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_AddsStore()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.CreateAsync(companyId, "Downtown", "US", "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal("Downtown", result.Value.Name);
        Assert.Single(_stores.Stores);
    }

    [Fact]
    public async Task CreateAsync_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var result = await _service.CreateAsync(Guid.NewGuid(), "Downtown", "US", "USD");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Equal("Company.NotFound", result.Error.Code);
        Assert.Empty(_stores.Stores);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ReturnsValidationErrorAndDoesNotAdd()
    {
        var companyId = _companies.AddActiveCompany();

        var result = await _service.CreateAsync(companyId, "Downtown", "US", "ABC");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Empty(_stores.Stores);
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExistsInCompany_ReturnsConflict()
    {
        var companyId = _companies.AddActiveCompany();
        await _service.CreateAsync(companyId, "Downtown", "US", "USD");

        var result = await _service.CreateAsync(companyId, "Downtown", "BR", "BRL");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Store.NameAlreadyInUse", result.Error.Code);
        Assert.Single(_stores.Stores);
    }

    [Fact]
    public async Task CreateAsync_WhenNameExistsInAnotherCompany_ReturnsSuccess()
    {
        var companyA = _companies.AddActiveCompany();
        var companyB = _companies.AddActiveCompany();
        await _service.CreateAsync(companyA, "Downtown", "US", "USD");

        var result = await _service.CreateAsync(companyB, "Downtown", "US", "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, _stores.Stores.Count);
    }

    [Fact]
    public async Task CreateAsync_WhenNameBelongsToDeletedStore_ReturnsSuccess()
    {
        var companyId = _companies.AddActiveCompany();
        var first = await _service.CreateAsync(companyId, "Downtown", "US", "USD");
        first.Value.Deactivate();

        var result = await _service.CreateAsync(companyId, "Downtown", "US", "USD");

        Assert.True(result.IsSuccess);
    }
}
