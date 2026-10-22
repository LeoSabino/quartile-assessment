using Microsoft.AspNetCore.Mvc;
using Stores.Domain;

namespace Stores.Api.Controllers;

[ApiController]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ErrorResult(Error error) => ErrorResults.From(error, HttpContext);
}
