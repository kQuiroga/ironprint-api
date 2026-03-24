using IronPrint.Domain.Entities;

namespace IronPrint.Domain.Ports;

public interface IDayLogRepository
{
    Task<DayLog?> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default);
    Task<IEnumerable<DayLog>> GetByDateRangeAsync(string userId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task UpsertAsync(DayLog dayLog, CancellationToken ct = default);
    Task DeleteAsync(string userId, DateOnly date, CancellationToken ct = default);
}
