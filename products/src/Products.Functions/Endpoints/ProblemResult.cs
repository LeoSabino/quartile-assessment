using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Products.Functions.Domain;

namespace Products.Functions.Endpoints;

public static class ProblemResult
{
    public static ObjectResult From(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails { Status = statusCode, Title = error.Message };
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };
    }
}
