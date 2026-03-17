using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Routines.UpdateRoutine;

public sealed class UpdateRoutineHandler : IRequestHandler<UpdateRoutineCommand, Result>
{
    private readonly IRoutineRepository _repo;

    public UpdateRoutineHandler(IRoutineRepository repo) => _repo = repo;

    public async Task<Result> Handle(UpdateRoutineCommand cmd, CancellationToken ct)
    {
        var routine = await _repo.GetByIdAsync(cmd.Id, cmd.UserId, ct);
        if (routine is null) return Result.Failure(Error.NotFound("Routine"));

        routine.Update(cmd.Name, cmd.WeeksDuration);
        await _repo.UpdateAsync(routine, ct);
        return Result.Success();
    }
}
