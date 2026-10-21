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

    public async Task<Result<Store>> GetAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        if (!await _companies.IsActiveAsync(companyId, cancellationToken))
            return Result<Store>.Failure(CompanyErrors.NotFound);

        var store = await _stores.GetByIdAsync(companyId, storeId, cancellationToken);
        if (store is null)
            return Result<Store>.Failure(StoreErrors.NotFound);

        return Result<Store>.Success(store);
    }

    public async Task<Result<IReadOnlyList<Store>>> ListAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        if (!await _companies.IsActiveAsync(companyId, cancellationToken))
            return Result<IReadOnlyList<Store>>.Failure(CompanyErrors.NotFound);

        var stores = await _stores.ListByCompanyAsync(companyId, cancellationToken);
        return Result<IReadOnlyList<Store>>.Success(stores);
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

    public async Task<Result<Store>> UpdateAsync(
        Guid companyId,
        Guid storeId,
        string name,
        string countryCode,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(companyId, storeId, cancellationToken);
        if (found.IsFailure)
            return found;

        var validation = Store.Validate(name, countryCode, currencyCode);
        if (validation.IsFailure)
            return Result<Store>.Failure(validation.Error);

        var store = found.Value;
        var newName = Store.NormalizeName(name);

        var nameChanged = !string.Equals(store.Name, newName, StringComparison.OrdinalIgnoreCase);
        if (nameChanged && await _stores.NameExistsAsync(companyId, newName, cancellationToken))
            return Result<Store>.Failure(StoreErrors.NameAlreadyInUse);

        store.Update(name, countryCode, currencyCode);
        var saved = await _stores.UpdateAsync(store, cancellationToken);
        if (saved.IsFailure)
            return Result<Store>.Failure(saved.Error);

        return found;
    }

    public async Task<Result> DeleteAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(companyId, storeId, cancellationToken);
        if (found.IsFailure)
            return Result.Failure(found.Error);

        found.Value.Deactivate();
        return await _stores.UpdateAsync(found.Value, cancellationToken);
    }
}
