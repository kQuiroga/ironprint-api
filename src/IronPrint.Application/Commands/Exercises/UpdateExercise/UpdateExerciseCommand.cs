using IronPrint.Application.Common;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Commands.Exercises.UpdateExercise;

public record UpdateExerciseCommand(Guid Id, string UserId, string Name, MuscleGroup MuscleGroup, string? Notes) : ICommand;
