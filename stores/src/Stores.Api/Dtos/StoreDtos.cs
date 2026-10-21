namespace Stores.Api.Dtos;

public record CreateStoreRequest(string Name, string CountryCode, string CurrencyCode);

public record UpdateStoreRequest(string Name, string CountryCode, string CurrencyCode);

public record StoreResponse(Guid Id, Guid CompanyId, string Name, string CountryCode, string CurrencyCode);
