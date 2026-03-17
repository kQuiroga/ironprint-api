using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Queries.Routines;

public record RoutineDto(Guid Id, string Name, int WeeksDuration, DateTime CreatedAt, IEnumerable<RoutineDayDto> Days);
public record RoutineDayDto(Guid Id, DayOfWeek DayOfWeek, IEnumerable<RoutineExerciseDto> Exercises);
public record RoutineExerciseDto(Guid Id, Guid ExerciseId, int Order, int TargetSets, int TargetReps);
