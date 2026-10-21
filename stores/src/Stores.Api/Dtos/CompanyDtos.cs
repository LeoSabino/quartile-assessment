namespace Stores.Api.Dtos;

public record CreateCompanyRequest(string Name);

public record CompanyResponse(Guid Id, string Name);
