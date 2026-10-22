using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Stores.Domain;

namespace Stores.Api.Controllers;

public static class ErrorResults
{
    public static readonly Error InvalidBody =
        new(ErrorType.Validation, "Request.InvalidBody", "The request body must be a valid JSON object.");

    public static ObjectResult From(Error error, HttpContext httpContext)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = httpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>()
            .CreateProblemDetails(httpContext, statusCode, error.Message);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
