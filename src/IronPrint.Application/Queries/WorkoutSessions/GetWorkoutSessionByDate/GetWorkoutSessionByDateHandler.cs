using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.WorkoutSessions.GetWorkoutSessionByDate;

public sealed class GetWorkoutSessionByDateHandler : IRequestHandler<GetWorkoutSessionByDateQuery, Result<WorkoutSessionDetailDto>>
{
    private readonly IWorkoutSessionRepository _repo;

    public GetWorkoutSessionByDateHandler(IWorkoutSessionRepository repo) => _repo = repo;

    public async Task<Result<WorkoutSessionDetailDto>> Handle(GetWorkoutSessionByDateQuery query, CancellationToken ct)
    {
        var session = await _repo.GetByDateAsync(query.UserId, query.Date, ct);
        if (session is null) return Result.Failure<WorkoutSessionDetailDto>(Error.NotFound("WorkoutSession"));

        var dto = new WorkoutSessionDetailDto(
            session.Id, session.Date, session.RoutineDayId,
            session.ExerciseLogs.Select(l => new ExerciseLogDto(
                l.Id, l.ExerciseId, l.Order,
                l.Sets.Select(s => new SetLogDto(s.Id, s.SetNumber, s.Weight.Value, s.Weight.Unit, s.Reps, s.Completed)))));

        return Result.Success(dto);
    }
}
