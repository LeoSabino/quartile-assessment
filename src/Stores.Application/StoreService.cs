using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.Application;

public class StoreService
{
    private readonly ICompanyRepository _companies;
    private readonly IStoreRepository _stores;

    public StoreService(ICompanyRepository companies, IStoreRepository stores)
    {
        _companies = companies;
        _stores = stores;
    }

    public async Task<Result<Store>> CreateAsync(
        Guid companyId,
        string name,
        string countryCode,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        if (!await _companies.IsActiveAsync(companyId, cancellationToken))
            return Result<Store>.Failure(CompanyErrors.NotFound);

        var result = Store.Create(companyId, name, countryCode, currencyCode);
        if (result.IsFailure)
            return result;

        var store = result.Value;

        if (await _stores.NameExistsAsync(companyId, store.Name, cancellationToken))
            return Result<Store>.Failure(StoreErrors.NameAlreadyInUse);

        await _stores.AddAsync(store, cancellationToken);
        return result;
    }
}
