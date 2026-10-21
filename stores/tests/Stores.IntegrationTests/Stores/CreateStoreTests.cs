using System.Net;
using System.Net.Http.Json;
using Stores.IntegrationTests.Companies;

namespace Stores.IntegrationTests.Stores;

[Collection("Api")]
public class CreateStoreTests
{
    private readonly HttpClient _client;

    public CreateStoreTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_WithValidData_ReturnsCreatedWithLocation()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(company.Id),
            new { name = "Downtown", countryCode = "us", currencyCode = "usd" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var store = await response.Content.ReadFromJsonAsync<StoreResponse>();
        Assert.NotNull(store);
        Assert.NotEqual(Guid.Empty, store.Id);
        Assert.Equal(company.Id, store.CompanyId);
        Assert.Equal("Downtown", store.Name);
        Assert.Equal("US", store.CountryCode);
        Assert.Equal("USD", store.CurrencyCode);
        Assert.Equal(StoreApi.StoreUrl(company.Id, store.Id), response.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task Post_WithInvalidCountryCode_ReturnsBadRequest()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(company.Id),
            new { name = "Downtown", countryCode = "XX", currencyCode = "USD" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.InvalidCountryCode", error?.Code);
    }

    [Fact]
    public async Task Post_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(Guid.NewGuid()),
            new { name = "Downtown", countryCode = "US", currencyCode = "USD" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Company.NotFound", error?.Code);
    }

    [Fact]
    public async Task Post_WithDuplicateNameInSameCompany_ReturnsConflict()
    {
        var company = await _client.CreateCompanyAsync();
        await _client.CreateStoreAsync(company.Id, "Downtown");

        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(company.Id),
            new { name = "downtown", countryCode = "US", currencyCode = "USD" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.NameAlreadyInUse", error?.Code);
    }

    [Fact]
    public async Task Post_WithSameNameInAnotherCompany_ReturnsCreated()
    {
        var first = await _client.CreateCompanyAsync();
        var second = await _client.CreateCompanyAsync();
        await _client.CreateStoreAsync(first.Id, "Downtown");

        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(second.Id),
            new { name = "Downtown", countryCode = "BR", currencyCode = "BRL" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
