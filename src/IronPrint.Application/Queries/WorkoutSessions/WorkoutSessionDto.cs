using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Queries.WorkoutSessions;

public record WorkoutCalendarEntryDto(DateOnly Date, bool HasSession, Guid? SessionId);

public record WorkoutSessionDetailDto(
    Guid Id,
    DateOnly Date,
    Guid? RoutineDayId,
    IEnumerable<ExerciseLogDto> ExerciseLogs);

public record ExerciseLogDto(Guid Id, Guid ExerciseId, int Order, IEnumerable<SetLogDto> Sets);

public record SetLogDto(Guid Id, int SetNumber, decimal WeightValue, WeightUnit WeightUnit, int Reps, bool Completed);
