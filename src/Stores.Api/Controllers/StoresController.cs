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
    public async Task<IActionResult> List(Guid companyId, CancellationToken cancellationToken)
    {
        var result = await _service.ListAsync(companyId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return Ok(result.Value.Select(ToResponse));
    }

    [HttpGet("{storeId:guid}")]
    public async Task<IActionResult> GetById(Guid companyId, Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _service.GetAsync(companyId, storeId, cancellationToken);
        if (result.IsFailure)
            return ErrorResult(result.Error);

        return Ok(ToResponse(result.Value));
    }

    [HttpPost]
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
