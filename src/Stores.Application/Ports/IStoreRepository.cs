using Stores.Domain;

namespace Stores.Application.Ports;

public interface IStoreRepository
{
    Task<bool> NameExistsAsync(Guid companyId, string name, CancellationToken cancellationToken);
    Task AddAsync(Store store, CancellationToken cancellationToken);
}
