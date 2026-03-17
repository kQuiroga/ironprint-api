using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.Routines.GetRoutineById;

public sealed class GetRoutineByIdHandler : IRequestHandler<GetRoutineByIdQuery, Result<RoutineDto>>
{
    private readonly IRoutineRepository _repo;

    public GetRoutineByIdHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result<RoutineDto>> Handle(GetRoutineByIdQuery query, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(query.Id, query.UserId, ct);
        if (routine is null) return Result.Failure<RoutineDto>(Error.NotFound("Routine"));

        var dto = new RoutineDto(routine.Id, routine.Name, routine.WeeksDuration, routine.CreatedAt,
            routine.Days.Select(d => new RoutineDayDto(d.Id, d.DayOfWeek,
                d.Exercises.Select(e => new RoutineExerciseDto(e.Id, e.ExerciseId, e.Order, e.TargetSets, e.TargetReps)))));

        return Result.Success(dto);
    }
}
