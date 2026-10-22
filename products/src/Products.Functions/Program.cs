using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Products.Functions.Persistence;

var builder = FunctionsApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Products")
    ?? throw new InvalidOperationException("Connection string 'Products' is not configured.");

builder.ConfigureFunctionsWebApplication();

builder.Services.AddProducts(connectionString);

builder.Build().Run();
