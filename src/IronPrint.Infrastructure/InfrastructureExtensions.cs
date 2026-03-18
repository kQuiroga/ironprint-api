using IronPrint.Application.Common;
using IronPrint.Domain.Ports;
using IronPrint.Infrastructure.Identity;
using IronPrint.Infrastructure.Persistence;
using IronPrint.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IronPrint.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        DapperConfig.Configure();

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IRoutineRepository, RoutineRepository>();
        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

        // Identity con Dapper
        services.AddIdentityCore<IdentityUser>()
            .AddUserStore<DapperUserStore>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // JWT y tokens de autenticación
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        // Handlers de auth (viven en Infrastructure por depender de Identity)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InfrastructureExtensions).Assembly));

        return services;
    }
}
