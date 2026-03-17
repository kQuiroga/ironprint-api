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
            "SELECT id, user_id, date, routine_day_id FROM workout_sessions WHERE id = @id AND user_id = @userId",
            new { id, userId });

        return row is null ? null : await LoadWithLogsAsync(conn, row);
    }

    public async Task<IEnumerable<WorkoutSession>> GetByDateRangeAsync(string userId, DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var rows = await conn.QueryAsync<SessionRow>(
            """
            SELECT id, user_id, date, routine_day_id FROM workout_sessions
            WHERE user_id = @userId AND date BETWEEN @from AND @to
            ORDER BY date
            """,
            new { userId, from, to });

        var sessions = new List<WorkoutSession>();
        foreach (var row in rows)
            sessions.Add(await LoadWithLogsAsync(conn, row));

        return sessions;
    }

    public async Task<WorkoutSession?> GetByDateAsync(string userId, DateOnly date, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        var row = await conn.QuerySingleOrDefaultAsync<SessionRow>(
            "SELECT id, user_id, date, routine_day_id FROM workout_sessions WHERE user_id = @userId AND date = @date",
            new { userId, date });

        return row is null ? null : await LoadWithLogsAsync(conn, row);
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
        await conn.ExecuteAsync(
            "UPDATE workout_sessions SET routine_day_id = @RoutineDayId WHERE id = @Id",
            new { session.RoutineDayId, session.Id });
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync("DELETE FROM workout_sessions WHERE id = @id", new { id });
    }

    private static async Task<WorkoutSession> LoadWithLogsAsync(Npgsql.NpgsqlConnection conn, SessionRow row)
    {
        var logRows = await conn.QueryAsync<ExerciseLogRow>(
            """SELECT id, workout_session_id, exercise_id, "order" FROM exercise_logs WHERE workout_session_id = @sessionId ORDER BY "order" """,
            new { sessionId = row.Id });

        var logs = new List<ExerciseLog>();
        foreach (var logRow in logRows)
        {
            var setRows = await conn.QueryAsync<SetLogRow>(
                "SELECT id, exercise_log_id, set_number, weight_value, weight_unit, reps, completed FROM set_logs WHERE exercise_log_id = @logId ORDER BY set_number",
                new { logId = logRow.Id });

            var sets = setRows.Select(s =>
                SetLog.Reconstitute(s.Id, s.ExerciseLogId, s.SetNumber,
                    s.WeightUnit == 0 ? Weight.FromKg(s.WeightValue) : Weight.FromLb(s.WeightValue),
                    s.Reps, s.Completed));

            logs.Add(ExerciseLog.Reconstitute(logRow.Id, logRow.WorkoutSessionId, logRow.ExerciseId, logRow.Order, sets));
        }

        return WorkoutSession.Reconstitute(row.Id, row.UserId, row.Date, row.RoutineDayId, logs);
    }

    private sealed record SessionRow(Guid Id, string UserId, DateOnly Date, Guid? RoutineDayId);
    private sealed record ExerciseLogRow(Guid Id, Guid WorkoutSessionId, Guid ExerciseId, int Order);
    private sealed record SetLogRow(Guid Id, Guid ExerciseLogId, int SetNumber, decimal WeightValue, int WeightUnit, int Reps, bool Completed);
}
