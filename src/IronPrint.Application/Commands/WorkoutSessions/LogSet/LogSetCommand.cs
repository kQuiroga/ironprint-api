using IronPrint.Application.Common;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Application.Commands.WorkoutSessions.LogSet;

public record LogSetCommand(
    string UserId,
    Guid WorkoutSessionId,
    Guid ExerciseId,
    int SetNumber,
    decimal WeightValue,
    WeightUnit WeightUnit,
    int Reps,
    bool Completed) : ICommand<Guid>;
