using IronPrint.Domain.Entities;

namespace IronPrint.Domain.Ports;

public interface IRoutineRepository
{
    Task<Routine?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<IEnumerable<Routine>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(Routine routine, CancellationToken ct = default);
    Task UpdateAsync(Routine routine, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
