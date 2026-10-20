namespace Stores.Domain;

public static class StoreErrors
{
    public static readonly Error NameRequired =
        new(ErrorType.Validation, "Store.NameRequired", "Store name is required.");
}
