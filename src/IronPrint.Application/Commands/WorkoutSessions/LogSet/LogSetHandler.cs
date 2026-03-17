using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using MediatR;

namespace IronPrint.Application.Commands.WorkoutSessions.LogSet;

public sealed class LogSetHandler : IRequestHandler<LogSetCommand, Result<Guid>>
{
    private readonly IWorkoutSessionRepository _repo;

    public LogSetHandler(IWorkoutSessionRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(LogSetCommand cmd, CancellationToken ct)
    {
        var session = await _repo.GetByIdAsync(cmd.WorkoutSessionId, cmd.UserId, ct);
        if (session is null) return Result.Failure<Guid>(Error.NotFound("WorkoutSession"));

        var weight = cmd.WeightUnit == WeightUnit.Kg
            ? Weight.FromKg(cmd.WeightValue)
            : Weight.FromLb(cmd.WeightValue);

        var log = session.ExerciseLogs.FirstOrDefault(l => l.ExerciseId == cmd.ExerciseId);
        if (log is null)
        {
            log = ExerciseLog.Create(session.Id, cmd.ExerciseId, session.ExerciseLogs.Count + 1);
            session.AddExerciseLog(log);
        }

        var set = SetLog.Create(log.Id, cmd.SetNumber, weight, cmd.Reps, cmd.Completed);
        log.AddSet(set);

        await _repo.UpdateAsync(session, ct);
        return Result.Success(set.Id);
    }
}
