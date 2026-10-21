using Microsoft.AspNetCore.Mvc;
using Stores.Api.Dtos;
using Stores.Application;

namespace Stores.Api.Controllers;

[Route("api/companies")]
public class CompaniesController : ApiControllerBase
{
    private readonly CompanyService _service;

    public CompaniesController(CompanyService service)
    {
        _service = service;
    }

    [HttpGet]
    [EndpointSummary("List active companies")]
    [ProducesResponseType<IEnumerable<CompanyResponse>>(StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var companies = await _service.ListAsync(cancellationToken);
        return Ok(companies.Select(c => new CompanyResponse(c.Id, c.Name)));
    }

    [HttpGet("{companyId:guid}")]
    [EndpointSummary("Get a company")]
    [ProducesResponseType<CompanyResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> GetById(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        var company = result.Value;
        return Ok(new CompanyResponse(company.Id, company.Name));
    }

    [HttpPost]
    [EndpointSummary("Create a company")]
    [EndpointDescription("The name must be unique among active companies.")]
    [ProducesResponseType<CompanyResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<IActionResult> Create(CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(request.Name, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        var company = result.Value;
        var response = new CompanyResponse(company.Id, company.Name);
        return CreatedAtAction(nameof(GetById), new { companyId = company.Id }, response);
    }

    [HttpDelete("{companyId:guid}")]
    [EndpointSummary("Delete a company")]
    [EndpointDescription("Soft delete. Returns 409 while the company still has active stores.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<IActionResult> Delete(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return NoContent();
    }
}
