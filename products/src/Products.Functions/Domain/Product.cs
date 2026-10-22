namespace Products.Functions.Domain;

public class Product
{
    public const int ProductCodeMaxLength = 50;
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 1000;
    public const int PriceDecimalPlaces = 2;

    public Guid Id { get; private set; }
    public Guid StoreId { get; private set; }
    public string ProductCode { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    private Product(Guid storeId, string productCode, string name, string? description, decimal price, int quantity)
    {
        StoreId = storeId;
        ProductCode = productCode;
        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
        IsActive = true;
    }

    public static Result<Product> Create(
        Guid storeId,
        string productCode,
        string name,
        string? description,
        decimal price,
        int quantity)
    {
        if (storeId == Guid.Empty)
            return Result<Product>.Failure(ProductErrors.StoreIdRequired);

        productCode = NormalizeText(productCode);
        name = NormalizeText(name);
        description = NormalizeDescription(description);

        var error = FindCodeError(productCode) ?? FindError(name, description, price, quantity);
        if (error is not null)
            return Result<Product>.Failure(error);

        var product = new Product(storeId, productCode, name, description, price, quantity);
        return Result<Product>.Success(product);
    }

    public Result Update(string productCode, string name, string? description, decimal price, int quantity)
    {
        if (!string.Equals(NormalizeText(productCode), ProductCode, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(ProductErrors.ProductCodeCannotChange);

        name = NormalizeText(name);
        description = NormalizeDescription(description);

        var error = FindError(name, description, price, quantity);
        if (error is not null)
            return Result.Failure(error);

        Name = name;
        Description = description;
        Price = price;
        Quantity = quantity;
        return Result.Success();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static Error? FindCodeError(string productCode)
    {
        if (productCode.Length == 0)
            return ProductErrors.ProductCodeRequired;

        if (productCode.Length > ProductCodeMaxLength)
            return ProductErrors.ProductCodeTooLong;

        return null;
    }

    private static Error? FindError(string name, string? description, decimal price, int quantity)
    {
        if (name.Length == 0)
            return ProductErrors.NameRequired;

        if (name.Length > NameMaxLength)
            return ProductErrors.NameTooLong;

        if (description?.Length > DescriptionMaxLength)
            return ProductErrors.DescriptionTooLong;

        if (price < 0 || decimal.Round(price, PriceDecimalPlaces) != price)
            return ProductErrors.InvalidPrice;

        if (quantity < 0)
            return ProductErrors.InvalidQuantity;

        return null;
    }

    private static string NormalizeText(string text) => text?.Trim() ?? string.Empty;

    private static string? NormalizeDescription(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
