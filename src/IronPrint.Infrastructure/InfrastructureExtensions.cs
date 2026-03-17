using IronPrint.Domain.Ports;
using IronPrint.Infrastructure.Persistence;
using IronPrint.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IronPrint.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IRoutineRepository, RoutineRepository>();
        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();

        return services;
    }
}
