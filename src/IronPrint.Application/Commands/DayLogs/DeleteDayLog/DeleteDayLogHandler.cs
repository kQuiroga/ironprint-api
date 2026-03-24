using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.DayLogs.DeleteDayLog;

public sealed class DeleteDayLogHandler : IRequestHandler<DeleteDayLogCommand, Result>
{
    private readonly IDayLogRepository _repo;

    public DeleteDayLogHandler(IDayLogRepository repo) => _repo = repo;

    public async Task<Result> Handle(DeleteDayLogCommand cmd, CancellationToken ct)
    {
        await _repo.DeleteAsync(cmd.UserId, cmd.Date, ct);
        return Result.Success();
    }
}
