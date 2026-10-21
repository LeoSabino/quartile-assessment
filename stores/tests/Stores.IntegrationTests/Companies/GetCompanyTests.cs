using System.Net;
using System.Net.Http.Json;

namespace Stores.IntegrationTests.Companies;

[Collection("Api")]
public class GetCompanyTests
{
    private readonly HttpClient _client;

    public GetCompanyTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_WhenCompanyExists_ReturnsOk()
    {
        var created = await _client.CreateCompanyAsync();

        var response = await _client.GetAsync($"/api/companies/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var company = await response.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.Equal(created, company);
    }

    [Fact]
    public async Task Get_WhenCompanyDoesNotExist_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/companies/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Company.NotFound", error?.Code);
    }

    [Fact]
    public async Task List_ReturnsActiveCompaniesOnly()
    {
        var active = await _client.CreateCompanyAsync();
        var deleted = await _client.CreateCompanyAsync();
        await _client.DeleteAsync($"/api/companies/{deleted.Id}");

        var companies = await _client.GetFromJsonAsync<List<CompanyResponse>>("/api/companies");

        Assert.NotNull(companies);
        Assert.Contains(active, companies);
        Assert.DoesNotContain(deleted, companies);
    }
}
