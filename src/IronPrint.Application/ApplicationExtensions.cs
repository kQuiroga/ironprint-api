using FluentValidation;
using IronPrint.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IronPrint.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationExtensions).Assembly));
        services.AddValidatorsFromAssembly(typeof(ApplicationExtensions).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
