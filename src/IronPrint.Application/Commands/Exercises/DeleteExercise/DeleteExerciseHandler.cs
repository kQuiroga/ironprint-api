using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Exercises.DeleteExercise;

public sealed class DeleteExerciseHandler : IRequestHandler<DeleteExerciseCommand, Result>
{
    private readonly IExerciseRepository _repo;

    public DeleteExerciseHandler(IExerciseRepository repo) => _repo = repo;

    public async Task<Result> Handle(DeleteExerciseCommand cmd, CancellationToken ct)
    {
        var exercise = await _repo.GetByIdAsync(cmd.Id, cmd.UserId, ct);
        if (exercise is null) return Result.Failure(Error.NotFound("Exercise"));

        await _repo.DeleteAsync(cmd.Id, ct);
        return Result.Success();
    }
}
