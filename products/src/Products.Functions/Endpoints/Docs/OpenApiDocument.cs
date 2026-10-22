namespace Products.Functions.Endpoints.Docs;

public static class OpenApiDocument
{
    private const string ResourceName = "Products.Functions.Endpoints.Docs.openapi.json";

    private static readonly Lazy<string> Content = new(() =>
    {
        using var stream = typeof(OpenApiDocument).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    });

    public static string Json => Content.Value;
}
