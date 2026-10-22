using System.Reflection;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Products.Functions.Endpoints;
using Products.Functions.Endpoints.Docs;

namespace Products.UnitTests.Endpoints;

public class OpenApiDocumentTests
{
    [Fact]
    public void Json_IsValidOpenApi3Document()
    {
        using var document = JsonDocument.Parse(OpenApiDocument.Json);

        Assert.StartsWith("3.", document.RootElement.GetProperty("openapi").GetString());
    }

    [Fact]
    public void Json_DocumentsEveryProductEndpoint()
    {
        using var document = JsonDocument.Parse(OpenApiDocument.Json);
        var paths = document.RootElement.GetProperty("paths");

        var triggers = typeof(ProductEndpoints)
            .GetMethods()
            .SelectMany(m => m.GetParameters())
            .Select(p => p.GetCustomAttribute<HttpTriggerAttribute>())
            .OfType<HttpTriggerAttribute>()
            .ToList();

        Assert.NotEmpty(triggers);
        foreach (var trigger in triggers)
        {
            Assert.True(paths.TryGetProperty($"/{trigger.Route}", out var path), $"Missing path /{trigger.Route}");
            foreach (var method in trigger.Methods!)
                Assert.True(path.TryGetProperty(method, out _), $"Missing {method.ToUpperInvariant()} /{trigger.Route}");
        }
    }

    [Fact]
    public void Json_DoesNotDocumentRoutesThatDoNotExist()
    {
        using var document = JsonDocument.Parse(OpenApiDocument.Json);

        var documented = document.RootElement.GetProperty("paths").EnumerateObject()
            .SelectMany(path => path.Value.EnumerateObject()
                .Where(operation => operation.Name != "parameters")
                .Select(operation => $"{operation.Name} {path.Name}"))
            .ToHashSet();

        var implemented = typeof(ProductEndpoints)
            .GetMethods()
            .SelectMany(m => m.GetParameters())
            .Select(p => p.GetCustomAttribute<HttpTriggerAttribute>())
            .OfType<HttpTriggerAttribute>()
            .SelectMany(t => t.Methods!.Select(method => $"{method} /{t.Route}"))
            .ToHashSet();

        Assert.Equal(implemented.Order(), documented.Order());
    }
}
