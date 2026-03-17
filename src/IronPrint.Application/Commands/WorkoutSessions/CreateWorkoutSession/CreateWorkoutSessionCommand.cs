using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.WorkoutSessions.CreateWorkoutSession;

public record CreateWorkoutSessionCommand(string UserId, DateOnly Date, Guid? RoutineDayId) : ICommand<Guid>;
