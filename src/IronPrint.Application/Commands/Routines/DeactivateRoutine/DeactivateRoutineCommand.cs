using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.DeactivateRoutine;

public record DeactivateRoutineCommand(Guid RoutineId, string UserId) : ICommand;
