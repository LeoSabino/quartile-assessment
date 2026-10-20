namespace Stores.Domain;

public class Store
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string CountryCode { get; private set; }
    public string CurrencyCode { get; private set; }

    private Store(Guid companyId, string name, string countryCode, string currencyCode)
    {
        CompanyId = companyId;
        Name = name;
        CountryCode = countryCode;
        CurrencyCode = currencyCode;
    }

    public static Result<Store> Create(Guid companyId, string name, string countryCode, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Store>.Failure(StoreErrors.NameRequired);

        var store = new Store(companyId, name, countryCode, currencyCode);
        return Result<Store>.Success(store);
    }
}
