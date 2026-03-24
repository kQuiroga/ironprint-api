using Dapper;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using IronPrint.Infrastructure.Persistence;

namespace IronPrint.Infrastructure.Repositories;

public sealed class DayLogRepository : IDayLogRepository
{
    private readonly IDbConnectionFactory _db;

    public DayLogRepository(IDbConnectionFactory db) => _db = db;

    public async Task<DayLog?> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<DayLogRow>(
            "SELECT id, user_id, date, status FROM day_logs WHERE user_id = @userId AND date = @date",
            new { userId, date });

        return row is null ? null : DayLog.Reconstitute(row.Id, row.UserId, row.Date, (DayLogStatus)row.Status);
    }

    public async Task<IEnumerable<DayLog>> GetByDateRangeAsync(string userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var rows = await conn.QueryAsync<DayLogRow>(
            """
            SELECT id, user_id, date, status FROM day_logs
            WHERE user_id = @userId AND date BETWEEN @from AND @to
            ORDER BY date
            """,
            new { userId, from, to });

        return rows.Select(r => DayLog.Reconstitute(r.Id, r.UserId, r.Date, (DayLogStatus)r.Status));
    }

    public async Task UpsertAsync(DayLog dayLog, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            """
            INSERT INTO day_logs (id, user_id, date, status)
            VALUES (@Id, @UserId, @Date, @Status)
            ON CONFLICT (user_id, date) DO UPDATE SET status = @Status
            """,
            new { dayLog.Id, dayLog.UserId, dayLog.Date, Status = (int)dayLog.Status });
    }

    public async Task DeleteAsync(string userId, DateOnly date, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "DELETE FROM day_logs WHERE user_id = @userId AND date = @date",
            new { userId, date });
    }

    private sealed record DayLogRow(Guid Id, string UserId, DateOnly Date, int Status);
}
