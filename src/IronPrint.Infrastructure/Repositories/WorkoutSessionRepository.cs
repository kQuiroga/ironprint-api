using Dapper;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using IronPrint.Infrastructure.Persistence;

namespace IronPrint.Infrastructure.Repositories;

public sealed class WorkoutSessionRepository : IWorkoutSessionRepository
{
    private readonly IDbConnectionFactory _db;

    public WorkoutSessionRepository(IDbConnectionFactory db) => _db = db;

    public async Task<WorkoutSession?> GetByIdAsync(Guid id, string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<SessionRow>(
            new CommandDefinition("SELECT id, user_id, date, routine_day_id FROM workout_sessions WHERE id = @id AND user_id = @userId",
            new { id, userId }, cancellationToken: ct));

        return row is null ? null : await LoadWithLogsAsync(conn, row, ct);
    }

    public async Task<IEnumerable<WorkoutSession>> GetByDateRangeAsync(string userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var joinRows = await conn.QueryAsync<SessionExerciseSetRow>(
            new CommandDefinition(
            """
            SELECT ws.id             AS SessionId,
                   ws.user_id        AS UserId,
                   ws.date           AS Date,
                   ws.routine_day_id AS RoutineDayId,
                   el.id             AS ExLogId,
                   el.exercise_id    AS ExerciseId,
                   el."order"        AS ExOrder,
                   sl.id             AS SetId,
                   sl.set_number     AS SetNumber,
                   sl.weight_value   AS WeightValue,
                   sl.weight_unit    AS WeightUnit,
                   sl.reps           AS Reps,
                   sl.completed      AS Completed
            FROM workout_sessions ws
            LEFT JOIN exercise_logs el ON el.workout_session_id = ws.id
            LEFT JOIN set_logs sl ON sl.exercise_log_id = el.id
            WHERE ws.user_id = @userId AND ws.date BETWEEN @from AND @to
            ORDER BY ws.date, el."order", sl.set_number
            """,
            new { userId, from, to }, cancellationToken: ct));

        return joinRows
            .GroupBy(r => r.SessionId)
            .Select(sg =>
            {
                var sf = sg.First();
                var logs = sg
                    .Where(r => r.ExLogId.HasValue)
                    .GroupBy(r => r.ExLogId!.Value)
                    .Select(lg =>
                    {
                        var lf = lg.First();
                        var sets = lg
                            .Where(r => r.SetId.HasValue)
                            .Select(r => SetLog.Reconstitute(
                                r.SetId!.Value, lf.ExLogId!.Value, r.SetNumber!.Value,
                                r.WeightUnit!.Value == 0 ? Weight.FromKg(r.WeightValue!.Value) : Weight.FromLb(r.WeightValue!.Value),
                                r.Reps!.Value, r.Completed!.Value));
                        return ExerciseLog.Reconstitute(lf.ExLogId!.Value, sf.SessionId, lf.ExerciseId!.Value, lf.ExOrder!.Value, sets);
                    });
                return WorkoutSession.Reconstitute(sf.SessionId, sf.UserId, sf.Date, sf.RoutineDayId, logs);
            })
            .ToList();
    }

    public async Task<WorkoutSession?> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<SessionRow>(
            new CommandDefinition("SELECT id, user_id, date, routine_day_id FROM workout_sessions WHERE user_id = @userId AND date = @date",
            new { userId, date }, cancellationToken: ct));

        return row is null ? null : await LoadWithLogsAsync(conn, row, ct);
    }

    public async Task AddAsync(WorkoutSession session, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(
            "INSERT INTO workout_sessions (id, user_id, date, routine_day_id) VALUES (@Id, @UserId, @Date, @RoutineDayId)",
            new { session.Id, session.UserId, session.Date, session.RoutineDayId },
            tx);

        foreach (var log in session.ExerciseLogs)
        {
            await conn.ExecuteAsync(
                """INSERT INTO exercise_logs (id, workout_session_id, exercise_id, "order") VALUES (@Id, @WorkoutSessionId, @ExerciseId, @Order)""",
                new { log.Id, log.WorkoutSessionId, log.ExerciseId, log.Order },
                tx);

            foreach (var set in log.Sets)
                await conn.ExecuteAsync(
                    """
                    INSERT INTO set_logs (id, exercise_log_id, set_number, weight_value, weight_unit, reps, completed)
                    VALUES (@Id, @ExerciseLogId, @SetNumber, @WeightValue, @WeightUnit, @Reps, @Completed)
                    """,
                    new { set.Id, set.ExerciseLogId, set.SetNumber, WeightValue = set.Weight.Value, WeightUnit = (int)set.Weight.Unit, set.Reps, set.Completed },
                    tx);
        }

        await tx.CommitAsync(ct);
    }

    public async Task UpdateAsync(WorkoutSession session, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        using var tx = await conn.BeginTransactionAsync(ct);

        await conn.ExecuteAsync(
            "UPDATE workout_sessions SET routine_day_id = @RoutineDayId WHERE id = @Id",
            new { session.RoutineDayId, session.Id }, tx);

        foreach (var log in session.ExerciseLogs)
        {
            await conn.ExecuteAsync(
                """INSERT INTO exercise_logs (id, workout_session_id, exercise_id, "order") VALUES (@Id, @WorkoutSessionId, @ExerciseId, @Order) ON CONFLICT (id) DO NOTHING""",
                new { log.Id, log.WorkoutSessionId, log.ExerciseId, log.Order }, tx);

            foreach (var set in log.Sets)
                await conn.ExecuteAsync(
                    """
                    INSERT INTO set_logs (id, exercise_log_id, set_number, weight_value, weight_unit, reps, completed)
                    VALUES (@Id, @ExerciseLogId, @SetNumber, @WeightValue, @WeightUnit, @Reps, @Completed)
                    ON CONFLICT (id) DO NOTHING
                    """,
                    new { set.Id, set.ExerciseLogId, set.SetNumber, WeightValue = set.Weight.Value, WeightUnit = (int)set.Weight.Unit, set.Reps, set.Completed }, tx);
        }

        await tx.CommitAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(new CommandDefinition("DELETE FROM workout_sessions WHERE id = @id", new { id }, cancellationToken: ct));
    }

    private static async Task<WorkoutSession> LoadWithLogsAsync(Npgsql.NpgsqlConnection conn, SessionRow row, CancellationToken ct = default)
    {
        var joinRows = await conn.QueryAsync<ExerciseSetJoinRow>(
            new CommandDefinition(
            """
            SELECT el.id               AS ExLogId,
                   el.workout_session_id AS WorkoutSessionId,
                   el.exercise_id      AS ExerciseId,
                   el."order"          AS "Order",
                   sl.id               AS SetId,
                   sl.set_number       AS SetNumber,
                   sl.weight_value     AS WeightValue,
                   sl.weight_unit      AS WeightUnit,
                   sl.reps             AS Reps,
                   sl.completed        AS Completed
            FROM exercise_logs el
            LEFT JOIN set_logs sl ON sl.exercise_log_id = el.id
            WHERE el.workout_session_id = @sessionId
            ORDER BY el."order", sl.set_number
            """,
            new { sessionId = row.Id }, cancellationToken: ct));

        var logs = joinRows
            .GroupBy(r => r.ExLogId)
            .Select(g =>
            {
                var first = g.First();
                var sets = g
                    .Where(r => r.SetId.HasValue)
                    .Select(r => SetLog.Reconstitute(
                        r.SetId!.Value, first.ExLogId, r.SetNumber,
                        r.WeightUnit == 0 ? Weight.FromKg(r.WeightValue) : Weight.FromLb(r.WeightValue),
                        r.Reps, r.Completed));
                return ExerciseLog.Reconstitute(first.ExLogId, first.WorkoutSessionId, first.ExerciseId, first.Order, sets);
            })
            .ToList();

        return WorkoutSession.Reconstitute(row.Id, row.UserId, row.Date, row.RoutineDayId, logs);
    }

    private sealed record SessionRow(Guid Id, string UserId, DateOnly Date, Guid? RoutineDayId);
    private sealed record ExerciseSetJoinRow(
        Guid ExLogId, Guid WorkoutSessionId, Guid ExerciseId, int Order,
        Guid? SetId, int SetNumber, decimal WeightValue, int WeightUnit, int Reps, bool Completed);
    private sealed record SessionExerciseSetRow(
        Guid SessionId, string UserId, DateOnly Date, Guid? RoutineDayId,
        Guid? ExLogId, Guid? ExerciseId, int? ExOrder,
        Guid? SetId, int? SetNumber, decimal? WeightValue, int? WeightUnit, int? Reps, bool? Completed);
}
