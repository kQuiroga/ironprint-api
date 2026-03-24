using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.Routines.GetActiveRoutine;

public sealed class GetActiveRoutineHandler : IRequestHandler<GetActiveRoutineQuery, Result<RoutineDto?>>
{
    private readonly IRoutineRepository _repo;

    public GetActiveRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result<RoutineDto?>> Handle(GetActiveRoutineQuery query, CancellationToken ct)
    {
        var routine = await _repo.GetActiveByUserIdAsync(query.UserId, ct);
        if (routine is null) return Result.Success<RoutineDto?>(null);

        var dto = new RoutineDto(routine.Id, routine.Name, routine.WeeksDuration, routine.IsActive, routine.CreatedAt,
            routine.Days.Select(d => new RoutineDayDto(d.Id, d.DayOfWeek, d.Name, d.MuscleGroups,
                d.Exercises.Select(e => new RoutineExerciseDto(e.Id, e.ExerciseId, e.Order, e.TargetSets, e.TargetReps)))));

        return Result.Success<RoutineDto?>(dto);
    }
}
