using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.ActivateRoutine;

public record ActivateRoutineCommand(Guid RoutineId, string UserId) : ICommand;
