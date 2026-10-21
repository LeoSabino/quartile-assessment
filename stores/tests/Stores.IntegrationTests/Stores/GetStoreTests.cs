using System.Net;
using System.Net.Http.Json;
using Stores.IntegrationTests.Companies;

namespace Stores.IntegrationTests.Stores;

[Collection("Api")]
public class GetStoreTests
{
    private readonly HttpClient _client;

    public GetStoreTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WhenStoreExists_ReturnsOk()
    {
        var company = await _client.CreateCompanyAsync();
        var created = await _client.CreateStoreAsync(company.Id);

        var response = await _client.GetAsync(StoreApi.StoreUrl(company.Id, created.Id));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var store = await response.Content.ReadFromJsonAsync<StoreResponse>();
        Assert.Equal(created, store);
    }

    [Fact]
    public async Task Get_WhenStoreDoesNotExist_ReturnsNotFound()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.GetAsync(StoreApi.StoreUrl(company.Id, Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.NotFound", error?.Code);
    }

    [Fact]
    public async Task Get_WhenStoreBelongsToAnotherCompany_ReturnsNotFound()
    {
        var owner = await _client.CreateCompanyAsync();
        var other = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(owner.Id);

        var response = await _client.GetAsync(StoreApi.StoreUrl(other.Id, store.Id));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_ReturnsOnlyStoresOfTheCompany()
    {
        var company = await _client.CreateCompanyAsync();
        var other = await _client.CreateCompanyAsync();
        var downtown = await _client.CreateStoreAsync(company.Id, "Downtown");
        var airport = await _client.CreateStoreAsync(company.Id, "Airport");
        await _client.CreateStoreAsync(other.Id, "Mall");

        var stores = await _client.GetFromJsonAsync<List<StoreResponse>>(StoreApi.StoresUrl(company.Id));

        Assert.Equal([airport, downtown], stores);
    }

    [Fact]
    public async Task List_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync(StoreApi.StoresUrl(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
