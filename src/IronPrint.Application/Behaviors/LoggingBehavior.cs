using System.Diagnostics;
using IronPrint.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IronPrint.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("Ejecutando {Request}", name);

        try
        {
            var response = await next(ct);
            sw.Stop();

            if (response is Result result && result.IsFailure)
                _logger.LogWarning("{Request} falló en {Elapsed}ms: [{Code}] {Description}",
                    name, sw.ElapsedMilliseconds, result.Error.Code, result.Error.Description);
            else
                _logger.LogInformation("{Request} completado en {Elapsed}ms", name, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "{Request} lanzó una excepción después de {Elapsed}ms", name, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
