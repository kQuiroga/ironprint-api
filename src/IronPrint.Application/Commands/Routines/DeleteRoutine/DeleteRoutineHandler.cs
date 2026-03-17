using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.DeleteRoutine;

public sealed class DeleteRoutineHandler : IRequestHandler<DeleteRoutineCommand, Result>
{
    private readonly IRoutineRepository _repo;

    public DeleteRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result> Handle(DeleteRoutineCommand cmd, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(cmd.Id, cmd.UserId, ct);
        if (routine is null) return Result.Failure(Error.NotFound("Routine"));

        await _repo.DeleteAsync(cmd.Id, ct);
        return Result.Success();
    }
}
