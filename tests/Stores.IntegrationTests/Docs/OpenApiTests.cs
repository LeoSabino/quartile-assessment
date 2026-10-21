using System.Net;
using System.Text.Json;

namespace Stores.IntegrationTests.Docs;

[Collection("Api")]
public class OpenApiTests
{
    private readonly HttpClient _client;

    public OpenApiTests(StoresApiFactory factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Remove(StoresApiFactory.ApiKeyHeader);
    }

    [Fact]
    public async Task SwaggerUi_WithoutApiKey_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApiDocument_DeclaresApiKeyHeader()
    {
        using var document = JsonDocument.Parse(await _client.GetStringAsync("/openapi/v1.json"));

        var scheme = document.RootElement.GetProperty("components").GetProperty("securitySchemes").GetProperty("ApiKey");
        Assert.Equal("apiKey", scheme.GetProperty("type").GetString());
        Assert.Equal("header", scheme.GetProperty("in").GetString());
        Assert.Equal(StoresApiFactory.ApiKeyHeader, scheme.GetProperty("name").GetString());
    }

    [Fact]
    public async Task OpenApiDocument_ListsStoreEndpoints()
    {
        using var document = JsonDocument.Parse(await _client.GetStringAsync("/openapi/v1.json"));

        var paths = document.RootElement.GetProperty("paths");
        Assert.True(paths.TryGetProperty("/api/companies/{companyId}/stores/{storeId}", out _));
    }
}
