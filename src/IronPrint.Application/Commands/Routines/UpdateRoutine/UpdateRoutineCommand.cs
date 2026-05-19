using IronPrint.Application.Common;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Commands.Routines.UpdateRoutine;

public record UpdateRoutineExerciseDto(Guid ExerciseId, int Order, int TargetSets, int TargetReps);
public record UpdateRoutineDayDto(DayOfWeek DayOfWeek, string? Name, IEnumerable<MuscleGroup> MuscleGroups, IEnumerable<UpdateRoutineExerciseDto> Exercises);
public record UpdateRoutineCommand(Guid Id, string UserId, string Name, int WeeksDuration, IEnumerable<UpdateRoutineDayDto>? Days = null) : ICommand;
