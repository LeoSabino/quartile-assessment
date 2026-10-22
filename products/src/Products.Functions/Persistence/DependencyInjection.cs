using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Functions.Application;
using Products.Functions.Application.Ports;

namespace Products.Functions.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddProducts(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ProductService>();

        return services;
    }
}
