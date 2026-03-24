using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.DeactivateRoutine;

public sealed class DeactivateRoutineHandler : IRequestHandler<DeactivateRoutineCommand, Result>
{
    private readonly IRoutineRepository _repo;

    public DeactivateRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result> Handle(DeactivateRoutineCommand cmd, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(cmd.RoutineId, cmd.UserId, ct);
        if (routine is null) return Result.Failure(Error.NotFound("Routine"));

        await _repo.DeactivateAsync(cmd.RoutineId, cmd.UserId, ct);
        return Result.Success();
    }
}
