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
}
