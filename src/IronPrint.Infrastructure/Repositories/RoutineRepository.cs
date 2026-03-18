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

        var rows = await conn.QueryAsync<FlatRoutineRow>(
            RoutineJoinSql + " WHERE r.id = @id AND r.user_id = @userId",
            new { id, userId });

        return BuildRoutines(rows).FirstOrDefault();
    }

    public async Task<IEnumerable<Routine>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);

        var rows = await conn.QueryAsync<FlatRoutineRow>(
            RoutineJoinSql + " WHERE r.user_id = @userId ORDER BY r.created_at DESC, rd.day_of_week, re.\"order\"",
            new { userId });

        return BuildRoutines(rows);
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

    private const string RoutineJoinSql =
        """
        SELECT
            r.id              AS RoutineId,
            r.user_id         AS UserId,
            r.name            AS Name,
            r.weeks_duration  AS WeeksDuration,
            r.created_at      AS CreatedAt,
            rd.id             AS DayId,
            rd.routine_id     AS DayRoutineId,
            rd.day_of_week    AS DayOfWeek,
            re.id             AS ExId,
            re.routine_day_id AS ExRoutineDayId,
            re.exercise_id    AS ExerciseId,
            re."order"        AS ExOrder,
            re.target_sets    AS TargetSets,
            re.target_reps    AS TargetReps
        FROM routines r
        LEFT JOIN routine_days rd ON rd.routine_id = r.id
        LEFT JOIN routine_exercises re ON re.routine_day_id = rd.id
        """;

    private static IEnumerable<Routine> BuildRoutines(IEnumerable<FlatRoutineRow> rows)
    {
        var routineOrder = new List<Guid>();
        var routineRows = new Dictionary<Guid, FlatRoutineRow>();
        var routineDays = new Dictionary<Guid, Dictionary<Guid, FlatRoutineRow>>();
        var dayExercises = new Dictionary<Guid, List<FlatRoutineRow>>();

        foreach (var row in rows)
        {
            if (!routineRows.ContainsKey(row.RoutineId))
            {
                routineOrder.Add(row.RoutineId);
                routineRows[row.RoutineId] = row;
                routineDays[row.RoutineId] = [];
            }

            if (row.DayId is null) continue;

            if (!routineDays[row.RoutineId].ContainsKey(row.DayId.Value))
            {
                routineDays[row.RoutineId][row.DayId.Value] = row;
                dayExercises[row.DayId.Value] = [];
            }

            if (row.ExId is not null)
                dayExercises[row.DayId.Value].Add(row);
        }

        return routineOrder.Select(routineId =>
        {
            var r = routineRows[routineId];
            var days = routineDays[routineId].Select(kv =>
            {
                var d = kv.Value;
                var exercises = dayExercises.TryGetValue(kv.Key, out var exRows)
                    ? exRows.Select(e => RoutineExercise.Reconstitute(
                        e.ExId!.Value, e.ExRoutineDayId!.Value, e.ExerciseId!.Value,
                        e.ExOrder!.Value, e.TargetSets!.Value, e.TargetReps!.Value))
                    : [];
                return RoutineDay.Reconstitute(d.DayId!.Value, d.DayRoutineId!.Value, (DayOfWeek)d.DayOfWeek!.Value, exercises);
            });
            return Routine.Reconstitute(r.RoutineId, r.UserId, r.Name, r.WeeksDuration, r.CreatedAt, days);
        });
    }

    private sealed record FlatRoutineRow(
        Guid RoutineId, string UserId, string Name, int WeeksDuration, DateTime CreatedAt,
        Guid? DayId, Guid? DayRoutineId, int? DayOfWeek,
        Guid? ExId, Guid? ExRoutineDayId, Guid? ExerciseId, int? ExOrder, int? TargetSets, int? TargetReps);
}
