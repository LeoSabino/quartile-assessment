namespace Stores.Domain;

public static class CompanyErrors
{
    public static readonly Error NotFound =
        new(ErrorType.NotFound, "Company.NotFound", "Company not found.");
}
