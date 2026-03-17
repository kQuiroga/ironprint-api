using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.WorkoutSessions.GetWorkoutSessionByDate;

public record GetWorkoutSessionByDateQuery(string UserId, DateOnly Date) : IQuery<WorkoutSessionDetailDto>;
