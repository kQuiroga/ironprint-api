using Dapper;
using IronPrint.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IronPrint.Infrastructure.Health;

public sealed class PostgreSqlHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory _db;

    public PostgreSqlHealthCheck(IDbConnectionFactory db) => _db = db;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            using var conn = await _db.CreateAsync(ct);
            await conn.ExecuteScalarAsync<int>("SELECT 1");
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("No se puede conectar a PostgreSQL.", ex);
        }
    }
}
