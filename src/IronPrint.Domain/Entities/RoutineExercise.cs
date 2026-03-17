namespace IronPrint.Domain.Entities;

public class RoutineExercise
{
    public Guid Id { get; private set; }
    public Guid RoutineDayId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int Order { get; private set; }
    public int TargetSets { get; private set; }
    public int TargetReps { get; private set; }

    private RoutineExercise() { }

    public static RoutineExercise Create(Guid routineDayId, Guid exerciseId, int order, int targetSets, int targetReps)
    {
        return new RoutineExercise
        {
            Id = Guid.NewGuid(),
            RoutineDayId = routineDayId,
            ExerciseId = exerciseId,
            Order = order,
            TargetSets = targetSets,
            TargetReps = targetReps
        };
    }

    public void Update(int order, int targetSets, int targetReps)
    {
        Order = order;
        TargetSets = targetSets;
        TargetReps = targetReps;
    }

    public static RoutineExercise Reconstitute(Guid id, Guid routineDayId, Guid exerciseId, int order, int targetSets, int targetReps)
    {
        return new RoutineExercise
        {
            Id = id,
            RoutineDayId = routineDayId,
            ExerciseId = exerciseId,
            Order = order,
            TargetSets = targetSets,
            TargetReps = targetReps
        };
    }
}
