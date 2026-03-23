using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.CreateRoutine;

public sealed class CreateRoutineHandler : IRequestHandler<CreateRoutineCommand, Result<Guid>>
{
    private readonly IRoutineRepository _repo;

    public CreateRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(CreateRoutineCommand cmd, CancellationToken ct)
    {
        var routine = Routine.Create(cmd.UserId, cmd.Name, cmd.WeeksDuration);

        if (cmd.Days is not null)
        {
            foreach (var dayDto in cmd.Days)
            {
                var day = RoutineDay.Create(routine.Id, dayDto.DayOfWeek);
                foreach (var exDto in dayDto.Exercises)
                {
                    var exercise = RoutineExercise.Create(day.Id, exDto.ExerciseId, exDto.Order, exDto.TargetSets, exDto.TargetReps);
                    day.AddExercise(exercise);
                }
                routine.AddDay(day);
            }
        }

        await _repo.AddAsync(routine, ct);
        return Result.Success(routine.Id);
    }
}
