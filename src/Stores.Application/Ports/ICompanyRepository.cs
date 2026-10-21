using Stores.Domain;

namespace Stores.Application.Ports;

public interface ICompanyRepository
{
    Task<bool> IsActiveAsync(Guid companyId, CancellationToken cancellationToken);
    Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Company company, CancellationToken cancellationToken);
    Task UpdateAsync(Company company, CancellationToken cancellationToken);
}
