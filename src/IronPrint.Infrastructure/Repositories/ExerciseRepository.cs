using Dapper;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using IronPrint.Infrastructure.Persistence;

namespace IronPrint.Infrastructure.Repositories;

public sealed class ExerciseRepository : IExerciseRepository
{
    private readonly IDbConnectionFactory _db;

    public ExerciseRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Exercise?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<ExerciseRow>(
            "SELECT id, user_id, name, muscle_group, notes, created_at FROM exercises WHERE id = @id AND user_id = @userId",
            new { id, userId });

        return row is null ? null : MapToDomain(row);
    }

    public async Task<IEnumerable<Exercise>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var rows = await conn.QueryAsync<ExerciseRow>(
            "SELECT id, user_id, name, muscle_group, notes, created_at FROM exercises WHERE user_id = @userId ORDER BY name",
            new { userId });

        return rows.Select(MapToDomain);
    }

    public async Task AddAsync(Exercise exercise, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            """
            INSERT INTO exercises (id, user_id, name, muscle_group, notes, created_at)
            VALUES (@Id, @UserId, @Name, @MuscleGroup, @Notes, @CreatedAt)
            """,
            new { exercise.Id, exercise.UserId, exercise.Name, MuscleGroup = (int)exercise.MuscleGroup, exercise.Notes, exercise.CreatedAt });
    }

    public async Task UpdateAsync(Exercise exercise, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "UPDATE exercises SET name = @Name, muscle_group = @MuscleGroup, notes = @Notes WHERE id = @Id",
            new { exercise.Name, MuscleGroup = (int)exercise.MuscleGroup, exercise.Notes, exercise.Id });
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync("DELETE FROM exercises WHERE id = @id", new { id });
    }

    private static Exercise MapToDomain(ExerciseRow r) =>
        Exercise.Reconstitute(r.Id, r.UserId, r.Name, (MuscleGroup)r.MuscleGroup, r.Notes, r.CreatedAt);

    private sealed record ExerciseRow(Guid Id, string UserId, string Name, int MuscleGroup, string? Notes, DateTime CreatedAt);
}
