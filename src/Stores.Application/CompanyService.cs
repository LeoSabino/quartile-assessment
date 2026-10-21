using Stores.Application.Ports;
using Stores.Domain;

namespace Stores.Application;

public class CompanyService
{
    private readonly ICompanyRepository _companies;
    private readonly IStoreRepository _stores;

    public CompanyService(ICompanyRepository companies, IStoreRepository stores)
    {
        _companies = companies;
        _stores = stores;
    }

    public async Task<Result<Company>> GetAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _companies.GetByIdAsync(companyId, cancellationToken);
        if (company is null)
            return Result<Company>.Failure(CompanyErrors.NotFound);

        return Result<Company>.Success(company);
    }

    public Task<IReadOnlyList<Company>> ListAsync(CancellationToken cancellationToken = default) =>
        _companies.ListAsync(cancellationToken);

    public async Task<Result<Company>> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var result = Company.Create(name);
        if (result.IsFailure)
            return result;

        var company = result.Value;

        if (await _companies.NameExistsAsync(company.Name, cancellationToken))
            return Result<Company>.Failure(CompanyErrors.NameAlreadyInUse);

        await _companies.AddAsync(company, cancellationToken);
        return result;
    }

    public async Task<Result> DeleteAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var found = await GetAsync(companyId, cancellationToken);
        if (found.IsFailure)
            return Result.Failure(found.Error);

        if (await _stores.HasActiveStoresAsync(companyId, cancellationToken))
            return Result.Failure(CompanyErrors.HasActiveStores);

        found.Value.Deactivate();
        await _companies.UpdateAsync(found.Value, cancellationToken);
        return Result.Success();
    }
}
