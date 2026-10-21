using System.Net;
using System.Net.Http.Json;

namespace Stores.IntegrationTests.Security;

[Collection("Api")]
public class ApiKeyTests
{
    private readonly HttpClient _client;

    public ApiKeyTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Remove(StoresApiFactory.ApiKeyHeader);
    }

    [Fact]
    public async Task Request_WithoutApiKey_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/companies");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Request_WithWrongApiKey_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Add(StoresApiFactory.ApiKeyHeader, "wrong-key");

        var response = await _client.GetAsync("/api/companies");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Request_WithEmptyApiKey_ReturnsUnauthorized()
    {
        _client.DefaultRequestHeaders.Add(StoresApiFactory.ApiKeyHeader, "");

        var response = await _client.GetAsync("/api/companies");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(StoresApiFactory.TestsApiKey)]
    [InlineData(StoresApiFactory.QuartileApiKey)]
    public async Task Request_WithKeyOfAnyClient_ReturnsOk(string apiKey)
    {
        _client.DefaultRequestHeaders.Add(StoresApiFactory.ApiKeyHeader, apiKey);

        var response = await _client.GetAsync("/api/companies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_WithoutApiKey_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/companies", new { name = "Intruder Inc" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task OpenApiDocument_WithoutApiKey_ReturnsOk()
    {
        var response = await _client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
