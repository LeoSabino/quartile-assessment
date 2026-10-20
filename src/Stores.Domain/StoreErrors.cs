namespace Stores.Domain;

public static class StoreErrors
{
    public static readonly Error CompanyIdRequired =
        new(ErrorType.Validation, "Store.CompanyIdRequired", "Company id is required.");

    public static readonly Error NameRequired =
        new(ErrorType.Validation, "Store.NameRequired", "Store name is required.");

    public static readonly Error NameTooLong =
        new(ErrorType.Validation, "Store.NameTooLong", $"Store name must have at most {Store.NameMaxLength} characters.");

    public static readonly Error InvalidCountryCode =
        new(ErrorType.Validation, "Store.InvalidCountryCode", "Invalid country code. Use a two-letter code, like US or BR.");

    public static readonly Error InvalidCurrencyCode =
        new(ErrorType.Validation, "Store.InvalidCurrencyCode", "Invalid currency code. Use a three-letter code, like USD or BRL.");
}
