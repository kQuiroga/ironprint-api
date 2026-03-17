using IronPrint.Domain.Entities;

namespace IronPrint.Domain.Ports;

public interface IWorkoutSessionRepository
{
    Task<WorkoutSession?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<IEnumerable<WorkoutSession>> GetByDateRangeAsync(string userId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<WorkoutSession?> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default);
    Task AddAsync(WorkoutSession session, CancellationToken ct = default);
    Task UpdateAsync(WorkoutSession session, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
