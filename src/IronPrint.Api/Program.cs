using IronPrint.Infrastructure;
using IronPrint.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Connection string 'Database' no encontrada.");

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

DatabaseMigrator.Migrate(connectionString);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
