using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.DayLogs.UpsertDayLog;

public sealed class UpsertDayLogHandler : IRequestHandler<UpsertDayLogCommand, Result>
{
    private readonly IDayLogRepository _repo;

    public UpsertDayLogHandler(IDayLogRepository repo) => _repo = repo;

    public async Task<Result> Handle(UpsertDayLogCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.GetByDateAsync(cmd.UserId, cmd.Date, ct);

        if (existing is not null)
            existing.UpdateStatus(cmd.Status);
        else
            existing = DayLog.Create(cmd.UserId, cmd.Date, cmd.Status);

        await _repo.UpsertAsync(existing, ct);
        return Result.Success();
    }
}
