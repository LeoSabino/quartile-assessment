using Stores.Domain;

namespace Stores.Application.Ports;

public interface IStoreRepository
{
    Task<Store?> GetByIdAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Store>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(Guid companyId, string name, CancellationToken cancellationToken);
    Task AddAsync(Store store, CancellationToken cancellationToken);
    Task UpdateAsync(Store store, CancellationToken cancellationToken);
}
