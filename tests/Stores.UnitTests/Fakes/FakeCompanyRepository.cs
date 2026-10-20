using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.UnitTests.Fakes;

public class FakeCompanyRepository : ICompanyRepository
{
    public List<Company> Companies { get; } = [];
    public List<Company> UpdatedCompanies { get; } = [];

    public Guid AddActiveCompany()
    {
        var company = Company.Create($"Company {Companies.Count + 1}").Value;
        SetId(company);
        Companies.Add(company);
        return company.Id;
    }

    public Task<bool> IsActiveAsync(Guid companyId, CancellationToken cancellationToken) =>
        Task.FromResult(Companies.Any(c => c.Id == companyId && c.IsActive));

    public Task<Company?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken) =>
        Task.FromResult(Companies.SingleOrDefault(c => c.Id == companyId && c.IsActive));

    public Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Company> companies = Companies.Where(c => c.IsActive).ToList();
        return Task.FromResult(companies);
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken) =>
        Task.FromResult(Companies.Any(c => c.IsActive && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)));

    public Task AddAsync(Company company, CancellationToken cancellationToken)
    {
        SetId(company);
        Companies.Add(company);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Company company, CancellationToken cancellationToken)
    {
        UpdatedCompanies.Add(company);
        return Task.CompletedTask;
    }

    private static void SetId(Company company) =>
        typeof(Company).GetProperty(nameof(Company.Id))!.SetValue(company, Guid.NewGuid());
}
