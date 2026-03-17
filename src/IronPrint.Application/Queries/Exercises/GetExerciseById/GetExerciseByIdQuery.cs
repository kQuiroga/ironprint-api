using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.Exercises.GetExerciseById;

public record GetExerciseByIdQuery(Guid Id, string UserId) : IQuery<ExerciseDto>;
