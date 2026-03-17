using IronPrint.Domain.Entities;

namespace IronPrint.Domain.Ports;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default);
    Task<IEnumerable<Exercise>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(Exercise exercise, CancellationToken ct = default);
    Task UpdateAsync(Exercise exercise, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
