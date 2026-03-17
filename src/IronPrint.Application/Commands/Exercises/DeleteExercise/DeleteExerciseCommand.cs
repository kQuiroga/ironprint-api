using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Exercises.DeleteExercise;

public record DeleteExerciseCommand(Guid Id, string UserId) : ICommand;
