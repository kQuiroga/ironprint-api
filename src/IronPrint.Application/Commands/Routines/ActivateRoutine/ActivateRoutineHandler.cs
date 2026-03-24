using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.ActivateRoutine;

public sealed class ActivateRoutineHandler : IRequestHandler<ActivateRoutineCommand, Result>
{
    private readonly IRoutineRepository _repo;

    public ActivateRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result> Handle(ActivateRoutineCommand cmd, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(cmd.RoutineId, cmd.UserId, ct);
        if (routine is null) return Result.Failure(Error.NotFound("Routine"));

        await _repo.ActivateAsync(cmd.RoutineId, cmd.UserId, ct);
        return Result.Success();
    }
}
