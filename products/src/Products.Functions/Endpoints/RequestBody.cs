using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Products.Functions.Domain;

namespace Products.Functions.Endpoints;

internal static class RequestBody
{
    public static readonly Error Invalid =
        new(ErrorType.Validation, "Request.InvalidBody", "The request body must be a valid JSON object.");

    public static async Task<T?> ReadAsync<T>(HttpRequest request, CancellationToken cancellationToken) where T : class
    {
        try
        {
            return await request.ReadFromJsonAsync<T>(cancellationToken);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return null;
        }
    }
}
