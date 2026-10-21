using Microsoft.EntityFrameworkCore;
using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.Infrastructure.Persistence.Repositories;

internal class CompanyRepository : ICompanyRepository
{
    private readonly StoresDbContext _db;

    public CompanyRepository(StoresDbContext db)
    {
        _db = db;
    }

    public Task<bool> IsActiveAsync(Guid companyId, CancellationToken cancellationToken) =>
        _db.Companies.AnyAsync(c => c.Id == companyId && c.IsActive, cancellationToken);

    public Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken) =>
        _db.Companies.SingleOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);

    public async Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken) =>
        await _db.Companies
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken) =>
        _db.Companies.AnyAsync(c => c.IsActive && c.Name == name, cancellationToken);

    public async Task AddAsync(Company company, CancellationToken cancellationToken)
    {
        _db.Companies.Add(company);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Company company, CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
