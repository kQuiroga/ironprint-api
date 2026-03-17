using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.Exercises.GetExercises;

public record GetExercisesQuery(string UserId) : IQuery<IEnumerable<ExerciseDto>>;
