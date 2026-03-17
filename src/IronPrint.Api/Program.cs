using System.Text;
using IronPrint.Api.Endpoints;
using Scalar.AspNetCore;
using IronPrint.Application;
using IronPrint.Infrastructure;
using IronPrint.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Lectura de configuración obligatoria al arrancar
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException("Connection string 'Database' no encontrada.");

var jwtSecret = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey no configurada.");

// Registro de servicios
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// Autenticación JWT Bearer
// Cada petición a endpoints protegidos debe incluir: Authorization: Bearer <token>
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,               // Verifica que el token fue emitido por esta API
            ValidateAudience = true,             // Verifica que el token es para el cliente correcto
            ValidateLifetime = true,             // Rechaza tokens expirados
            ValidateIssuerSigningKey = true,     // Verifica la firma con el secret
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Ejecutar migraciones SQL al arrancar (idempotente — solo aplica scripts nuevos)
DatabaseMigrator.Migrate(connectionString);

// Documentación interactiva disponible solo en Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI en /scalar/v1
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Registro de endpoints agrupados por dominio
app.MapAuthEndpoints();
app.MapExerciseEndpoints();
app.MapRoutineEndpoints();
app.MapWorkoutSessionEndpoints();

app.Run();
