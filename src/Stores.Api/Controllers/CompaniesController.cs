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
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var companies = await _service.ListAsync(cancellationToken);
        return Ok(companies.Select(c => new CompanyResponse(c.Id, c.Name)));
    }

    [HttpGet("{companyId:guid}")]
    public async Task<IActionResult> GetById(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        var company = result.Value;
        return Ok(new CompanyResponse(company.Id, company.Name));
    }

    [HttpPost]
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
    public async Task<IActionResult> Delete(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return NoContent();
    }
}
