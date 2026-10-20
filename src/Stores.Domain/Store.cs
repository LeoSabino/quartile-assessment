namespace Stores.Domain;

public class Store
{
    public const int NameMaxLength = 200;

    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string CountryCode { get; private set; }
    public string CurrencyCode { get; private set; }
    public bool IsActive { get; private set; }

    private Store(Guid companyId, string name, string countryCode, string currencyCode)
    {
        CompanyId = companyId;
        Name = name;
        CountryCode = countryCode;
        CurrencyCode = currencyCode;
        IsActive = true;
    }

    public static Result<Store> Create(Guid companyId, string name, string countryCode, string currencyCode)
    {
        if (companyId == Guid.Empty)
            return Result<Store>.Failure(StoreErrors.CompanyIdRequired);

        name = NormalizeName(name);
        countryCode = NormalizeCode(countryCode);
        currencyCode = NormalizeCode(currencyCode);

        var error = FindError(name, countryCode, currencyCode);
        if (error is not null)
            return Result<Store>.Failure(error);

        var store = new Store(companyId, name, countryCode, currencyCode);
        return Result<Store>.Success(store);
    }

    public Result Update(string name, string countryCode, string currencyCode)
    {
        name = NormalizeName(name);
        countryCode = NormalizeCode(countryCode);
        currencyCode = NormalizeCode(currencyCode);

        var error = FindError(name, countryCode, currencyCode);
        if (error is not null)
            return Result.Failure(error);

        Name = name;
        CountryCode = countryCode;
        CurrencyCode = currencyCode;
        return Result.Success();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public static Result Validate(string name, string countryCode, string currencyCode)
    {
        var error = FindError(NormalizeName(name), NormalizeCode(countryCode), NormalizeCode(currencyCode));
        return error is null ? Result.Success() : Result.Failure(error);
    }

    public static string NormalizeName(string name) => name?.Trim() ?? string.Empty;

    private static Error? FindError(string name, string countryCode, string currencyCode)
    {
        if (name.Length == 0)
            return StoreErrors.NameRequired;

        if (name.Length > NameMaxLength)
            return StoreErrors.NameTooLong;

        if (!IsoCodes.Countries.Contains(countryCode))
            return StoreErrors.InvalidCountryCode;

        if (!IsoCodes.Currencies.Contains(currencyCode))
            return StoreErrors.InvalidCurrencyCode;

        return null;
    }

    private static string NormalizeCode(string code) => code?.Trim().ToUpperInvariant() ?? string.Empty;
}
