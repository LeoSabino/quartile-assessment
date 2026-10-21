using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stores.Application.Ports;
using Stores.Domain;
using Stores.Infrastructure.Persistence.Configurations;

namespace Stores.Infrastructure.Persistence.Repositories;

internal class StoreRepository : IStoreRepository
{
    private const int ConstraintViolation = 547;

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

    public async Task<Result> UpdateAsync(Store store, CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException ex) when (IsProductsCurrencyViolation(ex))
        {
            return Result.Failure(StoreErrors.CurrencyLockedByProducts);
        }
    }

    // 547 is also raised by CHECK constraints, so the FK name must match too.
    private static bool IsProductsCurrencyViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException { Number: ConstraintViolation } sqlException &&
        sqlException.Message.Contains(StoreConfiguration.ProductsCurrencyForeignKey, StringComparison.Ordinal);
}
