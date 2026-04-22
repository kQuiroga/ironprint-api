using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Queries.WorkoutSessions;

public record WorkoutCalendarEntryDto(DateOnly Date, bool HasSession, Guid? SessionId, DayLogStatus? DayLogStatus, bool IsPlanned, Guid? RoutineDayId);

public record WorkoutSessionDetailDto(
    Guid Id,
    DateOnly Date,
    Guid? RoutineDayId,
    IEnumerable<ExerciseLogDto> Exercises);

public record ExerciseLogDto(Guid Id, Guid ExerciseId, int Order, IEnumerable<SetLogDto> Sets);

public record SetLogDto(Guid Id, int SetNumber, decimal WeightValue, int Reps, bool Completed);
