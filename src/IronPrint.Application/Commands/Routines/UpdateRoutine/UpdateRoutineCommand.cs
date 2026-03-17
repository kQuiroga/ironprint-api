using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.UpdateRoutine;

public record UpdateRoutineCommand(Guid Id, string UserId, string Name, int WeeksDuration) : ICommand;
