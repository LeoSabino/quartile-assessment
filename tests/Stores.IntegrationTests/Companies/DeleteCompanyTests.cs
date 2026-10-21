using System.Net;

namespace Stores.IntegrationTests.Companies;

[Collection("Api")]
public class DeleteCompanyTests
{
    private readonly HttpClient _client;

    public DeleteCompanyTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Delete_WhenCompanyHasNoStores_ReturnsNoContent()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.DeleteAsync($"/api/companies/{company.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ThenGet_ReturnsNotFound()
    {
        var company = await _client.CreateCompanyAsync();
        await _client.DeleteAsync($"/api/companies/{company.Id}");

        var response = await _client.GetAsync($"/api/companies/{company.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/companies/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithNameOfDeletedCompany_ReturnsCreated()
    {
        var company = await _client.CreateCompanyAsync();
        await _client.DeleteAsync($"/api/companies/{company.Id}");

        var recreated = await _client.CreateCompanyAsync(company.Name);

        Assert.NotEqual(company.Id, recreated.Id);
    }
}
