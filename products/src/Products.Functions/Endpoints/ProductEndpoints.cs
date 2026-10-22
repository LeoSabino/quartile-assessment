using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Products.Functions.Application;
using Products.Functions.Domain;

namespace Products.Functions.Endpoints;

public class ProductEndpoints
{
    private readonly ProductService _service;

    public ProductEndpoints(ProductService service)
    {
        _service = service;
    }

    [Function("CreateProduct")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        var body = await RequestBody.ReadAsync<CreateProductRequest>(request, cancellationToken);
        if (body is null)
            return ProblemResult.From(RequestBody.Invalid);

        if (body.Price is null)
            return ProblemResult.From(ProductErrors.PriceRequired);

        var result = await _service.CreateAsync(
            body.StoreId, body.ProductCode, body.Name, body.Description, body.Price.Value, body.Quantity, cancellationToken);
        if (result.IsFailure)
            return ProblemResult.From(result.Error);

        var product = result.Value;
        return new CreatedResult($"/api/products/{product.Id}", ToResponse(product));
    }

    [Function("ListStoreProducts")]
    public async Task<IActionResult> ListByStore(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "stores/{storeId}/products")] HttpRequest request,
        string storeId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(storeId, out var id))
            return ProblemResult.From(ProductErrors.StoreNotFound);

        var result = await _service.ListByStoreAsync(id, cancellationToken);
        if (result.IsFailure)
            return ProblemResult.From(result.Error);

        return new ContentResult
        {
            Content = result.Value,
            ContentType = "application/json",
            StatusCode = StatusCodes.Status200OK
        };
    }

    [Function("GetProduct")]
    public async Task<IActionResult> GetById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequest request,
        string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var productId))
            return ProblemResult.From(ProductErrors.NotFound);

        var result = await _service.GetAsync(productId, cancellationToken);
        if (result.IsFailure)
            return ProblemResult.From(result.Error);

        return new OkObjectResult(ToResponse(result.Value));
    }

    [Function("UpdateProduct")]
    public async Task<IActionResult> Update(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequest request,
        string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var productId))
            return ProblemResult.From(ProductErrors.NotFound);

        var body = await RequestBody.ReadAsync<UpdateProductRequest>(request, cancellationToken);
        if (body is null)
            return ProblemResult.From(RequestBody.Invalid);

        if (body.Price is null)
            return ProblemResult.From(ProductErrors.PriceRequired);

        var result = await _service.UpdateAsync(
            productId, body.ProductCode, body.Name, body.Description, body.Price.Value, body.Quantity, cancellationToken);
        if (result.IsFailure)
            return ProblemResult.From(result.Error);

        return new OkObjectResult(ToResponse(result.Value));
    }

    [Function("DeleteProduct")]
    public async Task<IActionResult> Delete(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequest request,
        string id,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var productId))
            return ProblemResult.From(ProductErrors.NotFound);

        var result = await _service.DeleteAsync(productId, cancellationToken);
        if (result.IsFailure)
            return ProblemResult.From(result.Error);

        return new NoContentResult();
    }

    private static ProductResponse ToResponse(Product product) =>
        new(product.Id,
            product.StoreId,
            product.ProductCode,
            product.Name,
            product.Description,
            product.Price,
            product.CurrencyCode,
            product.Quantity);
}
