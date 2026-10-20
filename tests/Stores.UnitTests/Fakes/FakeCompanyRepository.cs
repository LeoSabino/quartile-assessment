using Stores.Application.Ports;

namespace Stores.UnitTests.Fakes;

public class FakeCompanyRepository : ICompanyRepository
{
    private readonly HashSet<Guid> _activeCompanies = [];

    public Guid AddActiveCompany()
    {
        var id = Guid.NewGuid();
        _activeCompanies.Add(id);
        return id;
    }

    public Task<bool> IsActiveAsync(Guid companyId, CancellationToken cancellationToken) =>
        Task.FromResult(_activeCompanies.Contains(companyId));
}
