using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.UpdateRoutine;

public sealed class UpdateRoutineHandler : IRequestHandler<UpdateRoutineCommand, Result>
{
    private readonly IRoutineRepository _repo;

    public UpdateRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result> Handle(UpdateRoutineCommand cmd, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(cmd.Id, cmd.UserId, ct);
        if (routine is null) return Result.Failure(Error.NotFound("Routine"));

        routine.Update(cmd.Name, cmd.WeeksDuration);

        if (cmd.Days is null)
        {
            await _repo.UpdateAsync(routine, ct);
        }
        else
        {
            routine.ClearDays();
            foreach (var dayDto in cmd.Days)
            {
                var day = RoutineDay.Create(routine.Id, dayDto.DayOfWeek, dayDto.Name, dayDto.MuscleGroups);
                foreach (var exDto in dayDto.Exercises)
                {
                    var exercise = RoutineExercise.Create(day.Id, exDto.ExerciseId, exDto.Order, exDto.TargetSets, exDto.TargetReps);
                    day.AddExercise(exercise);
                }
                routine.AddDay(day);
            }

            await _repo.UpdateWithDaysAsync(routine, ct);
        }

        return Result.Success();
    }
}
