using Npgsql;

namespace IronPrint.Infrastructure.Persistence;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default);
}
