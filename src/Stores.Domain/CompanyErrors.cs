namespace Stores.Domain;

public static class CompanyErrors
{
    public static readonly Error NotFound =
        new(ErrorType.NotFound, "Company.NotFound", "Company not found.");

    public static readonly Error NameRequired =
        new(ErrorType.Validation, "Company.NameRequired", "Company name is required.");

    public static readonly Error NameTooLong =
        new(ErrorType.Validation, "Company.NameTooLong", $"Company name must have at most {Company.NameMaxLength} characters.");

    public static readonly Error NameAlreadyInUse =
        new(ErrorType.Conflict, "Company.NameAlreadyInUse", "There is already a company with this name.");

    public static readonly Error HasActiveStores =
        new(ErrorType.Conflict, "Company.HasActiveStores", "This company still has active stores. Delete them first.");
}
