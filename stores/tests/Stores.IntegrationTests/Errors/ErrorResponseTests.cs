using System.Net;
using System.Net.Http.Json;
using System.Text;
using Stores.IntegrationTests.Companies;
using Stores.IntegrationTests.Stores;

namespace Stores.IntegrationTests.Errors;

[Collection("Api")]
public class ErrorResponseTests
{
    private readonly HttpClient _client;

    public ErrorResponseTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostCompany_WithoutName_ReturnsDomainError()
    {
        var response = await _client.PostAsJsonAsync("/api/companies", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Company.NameRequired", error?.Code);
    }

    [Fact]
    public async Task PostStore_WithNullFields_ReturnsDomainError()
    {
        var company = await _client.CreateCompanyAsync();

        var response = await _client.PostAsJsonAsync(
            StoreApi.StoresUrl(company.Id),
            new { name = (string?)null, countryCode = (string?)null, currencyCode = (string?)null });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Store.NameRequired", error?.Code);
    }

    [Theory]
    [InlineData("{name:")]
    [InlineData("")]
    [InlineData("[]")]
    public async Task PostCompany_WithInvalidBody_ReturnsInvalidBody(string body)
    {
        var response = await _client.PostAsync(
            "/api/companies",
            new StringContent(body, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.Equal("Request.InvalidBody", error?.Code);
    }

    [Theory]
    [InlineData("/api/companies/not-a-guid")]
    [InlineData("/api/does-not-exist")]
    public async Task Get_UnknownRoute_ReturnsProblemDetails(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
