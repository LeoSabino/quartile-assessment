using Microsoft.AspNetCore.Mvc;
using Products.Functions.Domain;
using Products.Functions.Endpoints;

namespace Products.UnitTests.Endpoints;

public class ProblemResultTests
{
    [Theory]
    [InlineData(ErrorType.Validation, 400)]
    [InlineData(ErrorType.NotFound, 404)]
    [InlineData(ErrorType.Conflict, 409)]
    public void From_MapsErrorTypeToStatusCode(ErrorType type, int expectedStatus)
    {
        var result = ProblemResult.From(new Error(type, "Product.Test", "Test message."));

        Assert.Equal(expectedStatus, result.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal(expectedStatus, problem.Status);
    }

    [Fact]
    public void From_UsesMessageAsTitleAndCodeAsExtension()
    {
        var result = ProblemResult.From(ProductErrors.NotFound);

        var problem = Assert.IsType<ProblemDetails>(result.Value);
        Assert.Equal("Product not found.", problem.Title);
        Assert.Equal("Product.NotFound", problem.Extensions["code"]);
    }

    [Fact]
    public void From_UsesProblemJsonContentType()
    {
        var result = ProblemResult.From(ProductErrors.NotFound);

        Assert.Contains("application/problem+json", result.ContentTypes);
    }
}
