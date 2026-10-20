using Stores.Application;
using Stores.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Stores")
    ?? throw new InvalidOperationException("Connection string 'Stores' is not configured.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<StoreService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
