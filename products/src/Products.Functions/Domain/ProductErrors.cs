namespace Products.Functions.Domain;

public static class ProductErrors
{
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

    public static readonly Error InvalidPrice =
        new(ErrorType.Validation, "Product.InvalidPrice", $"Price must be zero or positive, with at most {Product.PriceDecimalPlaces} decimal places.");

    public static readonly Error InvalidQuantity =
        new(ErrorType.Validation, "Product.InvalidQuantity", "Quantity must be zero or positive.");
}
