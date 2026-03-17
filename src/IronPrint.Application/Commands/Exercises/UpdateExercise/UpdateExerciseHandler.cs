using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Exercises.UpdateExercise;

public sealed class UpdateExerciseHandler : IRequestHandler<UpdateExerciseCommand, Result>
{
    private readonly IExerciseRepository _repo;

    public UpdateExerciseHandler(IExerciseRepository repo) => _repo = repo;

    public async Task<Result> Handle(UpdateExerciseCommand cmd, CancellationToken ct)
    {
        var exercise = await _repo.GetByIdAsync(cmd.Id, cmd.UserId, ct);
        if (exercise is null) return Result.Failure(Error.NotFound("Exercise"));

        exercise.Update(cmd.Name, cmd.MuscleGroup, cmd.Notes);
        await _repo.UpdateAsync(exercise, ct);
        return Result.Success();
    }
}
