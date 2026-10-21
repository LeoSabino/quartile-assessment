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
        var response = await _client.PostAsJsonAsync("/api/companies", new { name = "Contoso Retail" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var company = await response.Content.ReadFromJsonAsync<CompanyResponse>();
        Assert.NotNull(company);
        Assert.NotEqual(Guid.Empty, company.Id);
        Assert.Equal("Contoso Retail", company.Name);
        Assert.Equal($"/api/companies/{company.Id}", response.Headers.Location?.AbsolutePath);
    }

    private record CompanyResponse(Guid Id, string Name);
}
