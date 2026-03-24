using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.DayLogs.DeleteDayLog;

public record DeleteDayLogCommand(string UserId, DateOnly Date) : ICommand;
