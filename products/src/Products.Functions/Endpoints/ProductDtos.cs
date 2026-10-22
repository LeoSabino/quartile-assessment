namespace Products.Functions.Endpoints;

public record CreateProductRequest(
    Guid StoreId,
    string ProductCode,
    string Name,
    string? Description,
    decimal? Price,
    int Quantity = 0);

public record UpdateProductRequest(
    string ProductCode,
    string Name,
    string? Description,
    decimal? Price,
    int Quantity = 0);

public record ProductResponse(
    Guid Id,
    Guid StoreId,
    string ProductCode,
    string Name,
    string? Description,
    decimal Price,
    string CurrencyCode,
    int Quantity);
