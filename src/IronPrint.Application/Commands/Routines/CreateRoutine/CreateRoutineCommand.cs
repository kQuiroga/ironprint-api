using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.CreateRoutine;

public record CreateRoutineExerciseDto(Guid ExerciseId, int Order, int TargetSets, int TargetReps);
public record CreateRoutineDayDto(DayOfWeek DayOfWeek, IEnumerable<CreateRoutineExerciseDto> Exercises);
public record CreateRoutineCommand(string UserId, string Name, int WeeksDuration, IEnumerable<CreateRoutineDayDto>? Days = null) : ICommand<Guid>;
