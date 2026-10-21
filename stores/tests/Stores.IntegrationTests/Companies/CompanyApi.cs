using System.Net.Http.Json;

namespace Stores.IntegrationTests.Companies;

public record CompanyResponse(Guid Id, string Name);

public record ErrorResponse(int Status, string Title, string Code);

public static class CompanyApi
{
    public static string UniqueName() => $"Company {Guid.NewGuid().ToString()[..8]}";

    public static async Task<CompanyResponse> CreateCompanyAsync(this HttpClient client, string? name = null)
    {
        var response = await client.PostAsJsonAsync("/api/companies", new { name = name ?? UniqueName() });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CompanyResponse>())!;
    }
}
