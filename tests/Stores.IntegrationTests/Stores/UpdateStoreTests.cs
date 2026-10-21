using System.Net;
using System.Net.Http.Json;
using Stores.IntegrationTests.Companies;

namespace Stores.IntegrationTests.Stores;

[Collection("Api")]
public class UpdateStoreTests
{
    private readonly HttpClient _client;

    public UpdateStoreTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Put_WithValidData_ReturnsUpdatedStore()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id);

        var response = await _client.PutAsJsonAsync(
            StoreApi.StoreUrl(company.Id, store.Id),
            new { name = "Paulista", countryCode = "BR", currencyCode = "BRL" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await _client.GetFromJsonAsync<StoreResponse>(StoreApi.StoreUrl(company.Id, store.Id));
        Assert.Equal(new StoreResponse(store.Id, company.Id, "Paulista", "BR", "BRL"), updated);
    }

    [Fact]
    public async Task Put_WithInvalidCurrencyCode_ReturnsBadRequest()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id);

        var response = await _client.PutAsJsonAsync(
            StoreApi.StoreUrl(company.Id, store.Id),
            new { name = "Downtown", countryCode = "US", currencyCode = "DOLLAR" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.InvalidCurrencyCode", error?.Code);
    }

    [Fact]
    public async Task Put_WithNameOfAnotherStore_ReturnsConflict()
    {
        var company = await _client.CreateCompanyAsync();
        await _client.CreateStoreAsync(company.Id, "Downtown");
        var airport = await _client.CreateStoreAsync(company.Id, "Airport");

        var response = await _client.PutAsJsonAsync(
            StoreApi.StoreUrl(company.Id, airport.Id),
            new { name = "Downtown", countryCode = "US", currencyCode = "USD" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.PutAsJsonAsync(
            StoreApi.StoreUrl(company.Id, Guid.NewGuid()),
            new { name = "Downtown", countryCode = "US", currencyCode = "USD" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
