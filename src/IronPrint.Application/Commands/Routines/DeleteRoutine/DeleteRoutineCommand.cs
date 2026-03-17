using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.DeleteRoutine;

public record DeleteRoutineCommand(Guid Id, string UserId) : ICommand;
