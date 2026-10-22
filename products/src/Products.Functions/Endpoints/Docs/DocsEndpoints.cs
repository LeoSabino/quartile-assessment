using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Products.Functions.Endpoints.Docs;

// Same Swagger UI as the Stores API: the files come embedded in the Swashbuckle.AspNetCore.SwaggerUI package.
public class DocsEndpoints
{
    private const string DocumentUrl = "/api/openapi/v1.json";
    private const string SwaggerUiResourcePrefix = "Swashbuckle.AspNetCore.SwaggerUI.node_modules.swagger_ui_dist.";

    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".js"] = "application/javascript",
        [".css"] = "text/css",
        [".png"] = "image/png"
    };

    private const string IndexPage =
        $$"""
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8">
          <title>Products API</title>
          <link rel="stylesheet" href="swagger-ui.css">
          <link rel="icon" type="image/png" href="favicon-32x32.png" sizes="32x32">
        </head>
        <body>
          <div id="swagger-ui"></div>
          <script src="swagger-ui-bundle.js"></script>
          <script src="swagger-ui-standalone-preset.js"></script>
          <script>
            SwaggerUIBundle({
              urls: [{ url: "{{DocumentUrl}}", name: "Products API v1" }],
              dom_id: "#swagger-ui",
              presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
              layout: "StandaloneLayout"
            });
          </script>
        </body>
        </html>
        """;

    [Function("OpenApiDocument")]
    public IActionResult Document(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi/v1.json")] HttpRequest request) =>
        new ContentResult
        {
            Content = OpenApiDocument.Json,
            ContentType = "application/json",
            StatusCode = StatusCodes.Status200OK
        };

    [Function("SwaggerUi")]
    public IActionResult SwaggerUi(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/{*file}")] HttpRequest request,
        string? file)
    {
        if (string.IsNullOrEmpty(file))
            return new RedirectResult("/api/swagger/index.html");

        if (file == "index.html")
            return new ContentResult { Content = IndexPage, ContentType = "text/html", StatusCode = StatusCodes.Status200OK };

        if (!ContentTypes.TryGetValue(Path.GetExtension(file), out var contentType))
            return new NotFoundResult();

        var stream = typeof(SwaggerUIOptions).Assembly.GetManifestResourceStream(SwaggerUiResourcePrefix + file);
        if (stream is null)
            return new NotFoundResult();

        return new FileStreamResult(new GZipStream(stream, CompressionMode.Decompress), contentType);
    }
}
