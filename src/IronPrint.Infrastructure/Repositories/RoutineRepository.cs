using Dapper;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Infrastructure.Persistence;

namespace IronPrint.Infrastructure.Repositories;

public sealed class RoutineRepository : IRoutineRepository
{
    private readonly IDbConnectionFactory _db;

    public RoutineRepository(IDbConnectionFactory db) => _db = db;

    public async Task<Routine?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);

        var routineRow = await conn.QuerySingleOrDefaultAsync<RoutineRow>(
            "SELECT id, user_id, name, weeks_duration, created_at FROM routines WHERE id = @id AND user_id = @userId",
            new { id, userId });

        if (routineRow is null) return null;

        return await LoadWithDaysAsync(conn, routineRow);
    }

    public async Task<IEnumerable<Routine>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);

        var rows = await conn.QueryAsync<RoutineRow>(
            "SELECT id, user_id, name, weeks_duration, created_at FROM routines WHERE user_id = @userId ORDER BY created_at DESC",
            new { userId });

        var routines = new List<Routine>();
        foreach (var row in rows)
            routines.Add(await LoadWithDaysAsync(conn, row));

        return routines;
    }

    public async Task AddAsync(Routine routine, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(
            """
            INSERT INTO routines (id, user_id, name, weeks_duration, created_at)
            VALUES (@Id, @UserId, @Name, @WeeksDuration, @CreatedAt)
            """,
            new { routine.Id, routine.UserId, routine.Name, routine.WeeksDuration, routine.CreatedAt },
            tx);

        foreach (var day in routine.Days)
        {
            await conn.ExecuteAsync(
                "INSERT INTO routine_days (id, routine_id, day_of_week) VALUES (@Id, @RoutineId, @DayOfWeek)",
                new { day.Id, day.RoutineId, DayOfWeek = (int)day.DayOfWeek },
                tx);

            foreach (var ex in day.Exercises)
                await conn.ExecuteAsync(
                    """
                    INSERT INTO routine_exercises (id, routine_day_id, exercise_id, "order", target_sets, target_reps)
                    VALUES (@Id, @RoutineDayId, @ExerciseId, @Order, @TargetSets, @TargetReps)
                    """,
                    new { ex.Id, ex.RoutineDayId, ex.ExerciseId, ex.Order, ex.TargetSets, ex.TargetReps },
                    tx);
        }

        await tx.CommitAsync(ct);
    }

    public async Task UpdateAsync(Routine routine, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "UPDATE routines SET name = @Name, weeks_duration = @WeeksDuration WHERE id = @Id",
            new { routine.Name, routine.WeeksDuration, routine.Id });
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync("DELETE FROM routines WHERE id = @id", new { id });
    }

    private static async Task<Routine> LoadWithDaysAsync(Npgsql.NpgsqlConnection conn, RoutineRow row)
    {
        var dayRows = await conn.QueryAsync<RoutineDayRow>(
            "SELECT id, routine_id, day_of_week FROM routine_days WHERE routine_id = @routineId ORDER BY day_of_week",
            new { routineId = row.Id });

        var days = new List<RoutineDay>();
        foreach (var dayRow in dayRows)
        {
            var exRows = await conn.QueryAsync<RoutineExerciseRow>(
                """
                SELECT id, routine_day_id, exercise_id, "order", target_sets, target_reps
                FROM routine_exercises WHERE routine_day_id = @dayId ORDER BY "order"
                """,
                new { dayId = dayRow.Id });

            var exercises = exRows.Select(e =>
                RoutineExercise.Reconstitute(e.Id, e.RoutineDayId, e.ExerciseId, e.Order, e.TargetSets, e.TargetReps));

            days.Add(RoutineDay.Reconstitute(dayRow.Id, dayRow.RoutineId, (DayOfWeek)dayRow.DayOfWeek, exercises));
        }

        return Routine.Reconstitute(row.Id, row.UserId, row.Name, row.WeeksDuration, row.CreatedAt, days);
    }

    private sealed record RoutineRow(Guid Id, string UserId, string Name, int WeeksDuration, DateTime CreatedAt);
    private sealed record RoutineDayRow(Guid Id, Guid RoutineId, int DayOfWeek);
    private sealed record RoutineExerciseRow(Guid Id, Guid RoutineDayId, Guid ExerciseId, int Order, int TargetSets, int TargetReps);
}
