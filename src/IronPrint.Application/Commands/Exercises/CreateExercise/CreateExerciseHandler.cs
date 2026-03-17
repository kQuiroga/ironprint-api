using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.Exercises.CreateExercise;

public sealed class CreateExerciseHandler : IRequestHandler<CreateExerciseCommand, Result<Guid>>
{
    private readonly IExerciseRepository _repo;

    public CreateExerciseHandler(IExerciseRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(CreateExerciseCommand cmd, CancellationToken ct)
    {
        var exercise = Exercise.Create(cmd.UserId, cmd.Name, cmd.MuscleGroup, cmd.Notes);
        await _repo.AddAsync(exercise, ct);
        return Result.Success(exercise.Id);
    }
}
