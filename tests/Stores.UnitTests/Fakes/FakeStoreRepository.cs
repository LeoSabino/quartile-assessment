using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.UnitTests.Fakes;

public class FakeStoreRepository : IStoreRepository
{
    public List<Store> Stores { get; } = [];

    public Task<bool> NameExistsAsync(Guid companyId, string name, CancellationToken cancellationToken)
    {
        var exists = Stores.Any(s =>
            s.IsActive &&
            s.CompanyId == companyId &&
            string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task AddAsync(Store store, CancellationToken cancellationToken)
    {
        Stores.Add(store);
        return Task.CompletedTask;
    }
}
