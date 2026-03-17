using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Routines.CreateRoutine;

public record CreateRoutineCommand(string UserId, string Name, int WeeksDuration) : ICommand<Guid>;
