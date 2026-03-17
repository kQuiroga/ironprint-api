using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.Routines.GetRoutines;

public sealed class GetRoutinesHandler : IRequestHandler<GetRoutinesQuery, Result<IEnumerable<RoutineDto>>>
{
    private readonly IRoutineRepository _repo;

    public GetRoutinesHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<RoutineDto>>> Handle(GetRoutinesQuery query, CancellationToken ct)
    {
        var routines = await _repo.GetByUserIdAsync(query.UserId, ct);
        return Result.Success(routines.Select(ToDto));
    }

    private static RoutineDto ToDto(Routine r) => new(
        r.Id, r.Name, r.WeeksDuration, r.CreatedAt,
        r.Days.Select(d => new RoutineDayDto(d.Id, d.DayOfWeek,
            d.Exercises.Select(e => new RoutineExerciseDto(e.Id, e.ExerciseId, e.Order, e.TargetSets, e.TargetReps)))));
}
