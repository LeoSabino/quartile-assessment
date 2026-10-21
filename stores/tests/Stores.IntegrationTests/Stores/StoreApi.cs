using System.Net.Http.Json;

namespace Stores.IntegrationTests.Stores;

public record StoreResponse(Guid Id, Guid CompanyId, string Name, string CountryCode, string CurrencyCode);

public static class StoreApi
{
    public static string StoresUrl(Guid companyId) => $"/api/companies/{companyId}/stores";

    public static string StoreUrl(Guid companyId, Guid storeId) => $"{StoresUrl(companyId)}/{storeId}";

    public static async Task<StoreResponse> CreateStoreAsync(
        this HttpClient client,
        Guid companyId,
        string name = "Downtown",
        string countryCode = "US",
        string currencyCode = "USD")
    {
        var response = await client.PostAsJsonAsync(StoresUrl(companyId), new { name, countryCode, currencyCode });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StoreResponse>())!;
    }
}
