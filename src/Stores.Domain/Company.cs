namespace Stores.Domain;

public class Company
{
    public const int NameMaxLength = 200;

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    private Company(string name)
    {
        Name = name;
        IsActive = true;
    }

    public static Result<Company> Create(string name)
    {
        name = name?.Trim() ?? string.Empty;

        if (name.Length == 0)
            return Result<Company>.Failure(CompanyErrors.NameRequired);

        if (name.Length > NameMaxLength)
            return Result<Company>.Failure(CompanyErrors.NameTooLong);

        return Result<Company>.Success(new Company(name));
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
