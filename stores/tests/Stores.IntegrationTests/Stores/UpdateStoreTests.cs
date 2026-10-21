using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Stores.Infrastructure.Persistence;
using Stores.IntegrationTests.Companies;
using Stores.IntegrationTests.Contracts;

namespace Stores.IntegrationTests.Stores;

[Collection("Api")]
public class UpdateStoreTests
{
    private readonly StoresApiFactory _factory;
    private readonly HttpClient _client;

    public UpdateStoreTests(StoresApiFactory factory)
    {
        _factory = factory;
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

    [Fact]
    public async Task Put_ChangingCurrencyOfStoreWithProducts_ReturnsConflict_ProductsContract()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id, currencyCode: "USD");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StoresDbContext>();
            await ProductsTableContract.EnsureCreatedAsync(db);
            await ProductsTableContract.AddProductAsync(db, store.Id, "USD");
        }

        var response = await _client.PutAsJsonAsync(
            StoreApi.StoreUrl(company.Id, store.Id),
            new { name = store.Name, countryCode = "US", currencyCode = "EUR" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.CurrencyLockedByProducts", error?.Code);

        var current = await _client.GetFromJsonAsync<StoreResponse>(StoreApi.StoreUrl(company.Id, store.Id));
        Assert.Equal("USD", current?.CurrencyCode);
    }
}
