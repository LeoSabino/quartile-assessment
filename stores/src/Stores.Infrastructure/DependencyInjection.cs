using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stores.Application.Ports;
using Stores.Infrastructure.Persistence;
using Stores.Infrastructure.Persistence.Repositories;

namespace Stores.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<StoresDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();

        return services;
    }
}
