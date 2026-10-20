namespace Stores.Application.Ports;

public interface ICompanyRepository
{
    Task<bool> IsActiveAsync(Guid companyId, CancellationToken cancellationToken);
}
