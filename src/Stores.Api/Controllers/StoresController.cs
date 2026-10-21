using Microsoft.AspNetCore.Mvc;
using Stores.Api.Dtos;
using Stores.Application;
using Stores.Domain;

namespace Stores.Api.Controllers;

[Route("api/companies/{companyId:guid}/stores")]
public class StoresController : ApiControllerBase
{
    private readonly StoreService _service;

    public StoresController(StoreService service)
    {
        _service = service;
    }

    [HttpGet]
    [EndpointSummary("List the stores of a company")]
    [ProducesResponseType<IEnumerable<StoreResponse>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> List(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.ListAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return Ok(result.Value.Select(ToResponse));
    }

    [HttpGet("{storeId:guid}")]
    [EndpointSummary("Get a store")]
    [ProducesResponseType<StoreResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> GetById(Guid companyId, Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(companyId, storeId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return Ok(ToResponse(result.Value));
    }

    [HttpPost]
    [EndpointSummary("Create a store")]
    [EndpointDescription("Country is an ISO 3166-1 alpha-2 code (US, BR) and currency an ISO 4217 code (USD, BRL). The name must be unique within the company.")]
    [ProducesResponseType<StoreResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<IActionResult> Create(Guid companyId, CreateStoreRequest request, CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            companyId, request.Name, request.CountryCode, request.CurrencyCode, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        var store = result.Value;
        return CreatedAtAction(nameof(GetById), new { companyId, storeId = store.Id }, ToResponse(store));
    }

    [HttpPut("{storeId:guid}")]
    [EndpointSummary("Update a store")]
    [ProducesResponseType<StoreResponse>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public async Task<IActionResult> Update(
        Guid companyId,
        Guid storeId,
        UpdateStoreRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(
            companyId, storeId, request.Name, request.CountryCode, request.CurrencyCode, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return Ok(ToResponse(result.Value));
    }

    [HttpDelete("{storeId:guid}")]
    [EndpointSummary("Delete a store")]
    [EndpointDescription("Soft delete. The name can be used again by a new store.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> Delete(Guid companyId, Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(companyId, storeId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return NoContent();
    }

    private static StoreResponse ToResponse(Store store) =>
        new(store.Id, store.CompanyId, store.Name, store.CountryCode, store.CurrencyCode);
}
