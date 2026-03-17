using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.Exercises.GetExerciseById;

public sealed class GetExerciseByIdHandler : IRequestHandler<GetExerciseByIdQuery, Result<ExerciseDto>>
{
    private readonly IExerciseRepository _repo;

    public GetExerciseByIdHandler(IExerciseRepository repo) => _repo = repo;

    public async Task<Result<ExerciseDto>> Handle(GetExerciseByIdQuery query, CancellationToken ct)
    {
        var exercise = await _repo.GetByIdAsync(query.Id, query.UserId, ct);
        if (exercise is null) return Result.Failure<ExerciseDto>(Error.NotFound("Exercise"));

        return Result.Success(new ExerciseDto(exercise.Id, exercise.Name, exercise.MuscleGroup, exercise.Notes, exercise.CreatedAt));
    }
}
