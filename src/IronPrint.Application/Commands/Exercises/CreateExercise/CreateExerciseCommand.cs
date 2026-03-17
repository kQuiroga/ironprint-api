using IronPrint.Application.Common;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Commands.Exercises.CreateExercise;

public record CreateExerciseCommand(string UserId, string Name, MuscleGroup MuscleGroup, string? Notes) : ICommand<Guid>;
