using System.Net;

namespace Stores.IntegrationTests;

[Collection("Api")]
public class HealthCheckTests
{
    private readonly HttpClient _client;

    public HealthCheckTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Remove(StoresApiFactory.ApiKeyHeader);
    }

    [Fact]
    public async Task Health_WithoutApiKey_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Ready_WithoutApiKey_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Ready_WithApiKey_ChecksDatabaseAndReturnsHealthy()
    {
        _client.DefaultRequestHeaders.Add(StoresApiFactory.ApiKeyHeader, StoresApiFactory.TestsApiKey);

        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
