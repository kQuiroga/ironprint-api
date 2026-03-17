using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Queries.Exercises;

public record ExerciseDto(Guid Id, string Name, MuscleGroup MuscleGroup, string? Notes, DateTime CreatedAt);
