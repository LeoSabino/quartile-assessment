using Stores.Application;
using Stores.Domain;
using Stores.UnitTests.Fakes;

namespace Stores.UnitTests.Application;

public class CreateCompanyTests
{
    private readonly FakeCompanyRepository _companies = new();
    private readonly FakeStoreRepository _stores = new();
    private readonly CompanyService _service;

    public CreateCompanyTests()
    {
        _service = new CompanyService(_companies, _stores);
    }

    [Fact]
    public async Task CreateAsync_WithValidName_AddsCompany()
    {
        var result = await _service.CreateAsync("Contoso Retail");

        Assert.True(result.IsSuccess);
        Assert.Equal("Contoso Retail", result.Value.Name);
        Assert.Single(_companies.Companies);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidName_ReturnsValidationErrorAndDoesNotAdd()
    {
        var result = await _service.CreateAsync("");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Empty(_companies.Companies);
    }

    [Fact]
    public async Task CreateAsync_WhenNameAlreadyExists_ReturnsConflict()
    {
        await _service.CreateAsync("Contoso Retail");

        var result = await _service.CreateAsync("contoso retail");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
        Assert.Equal("Company.NameAlreadyInUse", result.Error.Code);
        Assert.Single(_companies.Companies);
    }

    [Fact]
    public async Task CreateAsync_WhenNameBelongsToDeletedCompany_ReturnsSuccess()
    {
        var first = await _service.CreateAsync("Contoso Retail");
        await _service.DeleteAsync(first.Value.Id);

        var result = await _service.CreateAsync("Contoso Retail");

        Assert.True(result.IsSuccess);
    }
}
