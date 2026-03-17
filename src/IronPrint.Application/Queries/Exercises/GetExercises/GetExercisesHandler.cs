using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.Exercises.GetExercises;

public sealed class GetExercisesHandler : IRequestHandler<GetExercisesQuery, Result<IEnumerable<ExerciseDto>>>
{
    private readonly IExerciseRepository _repo;

    public GetExercisesHandler(IExerciseRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<ExerciseDto>>> Handle(GetExercisesQuery query, CancellationToken ct)
    {
        var exercises = await _repo.GetByUserIdAsync(query.UserId, ct);
        var dtos = exercises.Select(e => new ExerciseDto(e.Id, e.Name, e.MuscleGroup, e.Notes, e.CreatedAt));
        return Result.Success(dtos);
    }
}
