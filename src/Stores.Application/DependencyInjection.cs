using Microsoft.Extensions.DependencyInjection;

namespace Stores.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CompanyService>();
        services.AddScoped<StoreService>();
        return services;
    }
}
