using System.Net;
using System.Net.Http.Json;

namespace Stores.IntegrationTests.Companies;

[Collection("Api")]
public class CreateCompanyTests
{
    private readonly HttpClient _client;

    public CreateCompanyTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_WithValidName_ReturnsCreatedWithLocation()
    {
        var name = CompanyApi.UniqueName();

        var response = await _client.PostAsJsonAsync("/api/companies", new { name });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var company = await response.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.NotNull(company);
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal(name, company.Name);
        Assert.Equal($"/api/companies/{company.Id}", response.Headers.Location?.AbsolutePath);
    }

    [Fact]
    public async Task Post_WithEmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/companies", new { name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Company.NameRequired", error?.Code);
    }

    [Fact]
    public async Task Post_WithDuplicateName_ReturnsConflict()
    {
        var existing = await _client.CreateCompanyAsync();

        var response = await _client.PostAsJsonAsync("/api/companies", new { name = existing.Name.ToUpper() });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Company.NameAlreadyInUse", error?.Code);
    }
}
