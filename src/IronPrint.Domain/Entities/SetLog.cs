using IronPrint.Domain.ValueObjects;

namespace IronPrint.Domain.Entities;

public class SetLog
{
    public Guid Id { get; private set; }
    public Guid ExerciseLogId { get; private set; }
    public int SetNumber { get; private set; }
    public Weight Weight { get; private set; } = Weight.Zero;
    public int Reps { get; private set; }
    public bool Completed { get; private set; }

    private SetLog() { }

    public static SetLog Create(Guid exerciseLogId, int setNumber, Weight weight, int reps, bool completed = false)
    {
        return new SetLog
        {
            Id = Guid.NewGuid(),
            ExerciseLogId = exerciseLogId,
            SetNumber = setNumber,
            Weight = weight,
            Reps = reps,
            Completed = completed
        };
    }

    public void Complete(Weight weight, int reps)
    {
        Weight = weight;
        Reps = reps;
        Completed = true;
    }

    public static SetLog Reconstitute(Guid id, Guid exerciseLogId, int setNumber, Weight weight, int reps, bool completed)
    {
        return new SetLog
        {
            Id = id,
            ExerciseLogId = exerciseLogId,
            SetNumber = setNumber,
            Weight = weight,
            Reps = reps,
            Completed = completed
        };
    }
}
