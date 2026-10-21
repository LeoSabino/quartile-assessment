using System.Net;
using Stores.IntegrationTests.Companies;

namespace Stores.IntegrationTests.Stores;

[Collection("Api")]
public class DeleteStoreTests
{
    private readonly HttpClient _client;

    public DeleteStoreTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_WhenStoreExists_ReturnsNoContent()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id);

        var response = await _client.DeleteAsync(StoreApi.StoreUrl(company.Id, store.Id));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ThenGet_ReturnsNotFound()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id);
        await _client.DeleteAsync(StoreApi.StoreUrl(company.Id, store.Id));

        var response = await _client.GetAsync(StoreApi.StoreUrl(company.Id, store.Id));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithNameOfDeletedStore_ReturnsCreated()
    {
        var company = await _client.CreateCompanyAsync();
        var store = await _client.CreateStoreAsync(company.Id, "Downtown");
        await _client.DeleteAsync(StoreApi.StoreUrl(company.Id, store.Id));

        var recreated = await _client.CreateStoreAsync(company.Id, "Downtown");

        Assert.NotEqual(store.Id, recreated.Id);
    }

    [Fact]
    public async Task DeleteCompany_WhenItHasActiveStores_ReturnsConflict()
    {
        var company = await _client.CreateCompanyAsync();
        await _client.CreateStoreAsync(company.Id);

        var response = await _client.DeleteAsync($"/api/companies/{company.Id}");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
