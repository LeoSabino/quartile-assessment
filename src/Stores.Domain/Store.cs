namespace Stores.Domain;

public class Store
{
    private Store(Guid companyId, string name, string countryCode, string currencyCode)
    {
        CompanyId = companyId;
        Name = name;
        CountryCode = countryCode;
        CurrencyCode = currencyCode;
    }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public string CountryCode { get; private set; }
    public string CurrencyCode { get; private set; }

    public static Result<Store> Create(Guid companyId, string name, string countryCode, string currencyCode)
    {
        return Result<Store>.Success(new Store(companyId, name, countryCode, currencyCode));
    }
}
