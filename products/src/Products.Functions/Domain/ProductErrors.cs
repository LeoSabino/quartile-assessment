namespace Products.Functions.Domain;

public static class ProductErrors
{
    public static readonly Error NotFound =
        new(ErrorType.NotFound, "Product.NotFound", "Product not found.");

    public static readonly Error StoreNotFound =
        new(ErrorType.NotFound, "Product.StoreNotFound", "Store not found.");

    public static readonly Error ProductCodeAlreadyInUse =
        new(ErrorType.Conflict, "Product.ProductCodeAlreadyInUse", "This store already has a product with this code.");

    public static readonly Error RejectedByDatabase =
        new(ErrorType.Validation, "Product.RejectedByDatabase", "The product data was rejected by a database rule.");

    public static readonly Error StoreIdRequired =
        new(ErrorType.Validation, "Product.StoreIdRequired", "Store id is required.");

    public static readonly Error ProductCodeRequired =
        new(ErrorType.Validation, "Product.ProductCodeRequired", "Product code is required.");

    public static readonly Error ProductCodeTooLong =
        new(ErrorType.Validation, "Product.ProductCodeTooLong", $"Product code must have at most {Product.ProductCodeMaxLength} characters.");

    public static readonly Error ProductCodeCannotChange =
        new(ErrorType.Validation, "Product.ProductCodeCannotChange", "Product code can't be changed.");

    public static readonly Error NameRequired =
        new(ErrorType.Validation, "Product.NameRequired", "Product name is required.");

    public static readonly Error NameTooLong =
        new(ErrorType.Validation, "Product.NameTooLong", $"Product name must have at most {Product.NameMaxLength} characters.");

    public static readonly Error DescriptionTooLong =
        new(ErrorType.Validation, "Product.DescriptionTooLong", $"Description must have at most {Product.DescriptionMaxLength} characters.");

    public static readonly Error PriceRequired =
        new(ErrorType.Validation, "Product.PriceRequired", "Price is required.");

    public static readonly Error InvalidPrice =
        new(ErrorType.Validation, "Product.InvalidPrice", FormattableString.Invariant($"Price must be between 0 and {Product.MaxPrice}, with at most {Product.PriceDecimalPlaces} decimal places."));

    public static readonly Error InvalidQuantity =
        new(ErrorType.Validation, "Product.InvalidQuantity", "Quantity must be zero or positive.");
}
