using Microsoft.EntityFrameworkCore;
using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.Infrastructure.Persistence.Repositories;

internal class StoreRepository : IStoreRepository
{
    private readonly StoresDbContext _db;

    public StoreRepository(StoresDbContext db)
    {
        _db = db;
    }

    public Task<Store?> GetByIdAsync(Guid companyId, Guid storeId, CancellationToken cancellationToken) =>
        _db.Stores.SingleOrDefaultAsync(
            s => s.Id == storeId && s.CompanyId == companyId && s.IsActive,
            cancellationToken);

    public async Task<IReadOnlyList<Store>> ListByCompanyAsync(Guid companyId, CancellationToken cancellationToken) =>
        await _db.Stores
            .AsNoTracking()
            .Where(s => s.CompanyId == companyId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> HasActiveStoresAsync(Guid companyId, CancellationToken cancellationToken) =>
        _db.Stores.AnyAsync(s => s.CompanyId == companyId && s.IsActive, cancellationToken);

    public Task<bool> NameExistsAsync(Guid companyId, string name, CancellationToken cancellationToken) =>
        _db.Stores.AnyAsync(s => s.CompanyId == companyId && s.IsActive && s.Name == name, cancellationToken);

    public async Task AddAsync(Store store, CancellationToken cancellationToken)
    {
        _db.Stores.Add(store);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Store store, CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
