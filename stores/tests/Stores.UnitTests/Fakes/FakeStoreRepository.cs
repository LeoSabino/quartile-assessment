using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.UnitTests.Fakes;

public class FakeStoreRepository : IStoreRepository
{
    public List<Store> Stores { get; } = [];
    public List<Store> UpdatedStores { get; } = [];

    public Task<Store?> GetByIdAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken)
    {
        var store = Stores.SingleOrDefault(s => s.IsActive && s.CompanyId == companyId && s.Id == storeId);
        return Task.FromResult(store);
    }

    public Task<IReadOnlyList<Store>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken)
    {
        IReadOnlyList<Store> stores = Stores.Where(s => s.IsActive && s.CompanyId == companyId).ToList();
        return Task.FromResult(stores);
    }

    public Task<bool> HasActiveStoresAsync(Guid companyId, CancellationToken cancellationToken) =>
        Task.FromResult(Stores.Any(s => s.IsActive && s.CompanyId == companyId));

    public int NameChecks { get; private set; }

    public Task<bool> NameExistsAsync(Guid companyId, string name, CancellationToken cancellationToken)
    {
        NameChecks++;

        var exists = Stores.Any(s =>
            s.IsActive &&
            s.CompanyId == companyId &&
            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task AddAsync(Store store, CancellationToken cancellationToken)
    {
        typeof(Store).GetProperty(nameof(Store.Id))!.SetValue(store, Guid.NewGuid());
        Stores.Add(store);
        return Task.CompletedTask;
    }

    public Error? UpdateError { get; set; }

    public Task<Result> UpdateAsync(Store store, CancellationToken cancellationToken)
    {
        if (UpdateError is not null)
            return Task.FromResult(Result.Failure(UpdateError));

        UpdatedStores.Add(store);
        return Task.FromResult(Result.Success());
    }
}
