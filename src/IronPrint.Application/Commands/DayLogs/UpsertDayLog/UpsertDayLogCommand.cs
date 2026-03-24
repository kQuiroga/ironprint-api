using IronPrint.Application.Common;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Commands.DayLogs.UpsertDayLog;

public record UpsertDayLogCommand(string UserId, DateOnly Date, DayLogStatus Status) : ICommand;
