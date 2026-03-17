namespace IronPrint.Domain.Entities;

public class ExerciseLog
{
    public Guid Id { get; private set; }
    public Guid WorkoutSessionId { get; private set; }
    public Guid ExerciseId { get; private set; }
    public int Order { get; private set; }

    private readonly List<SetLog> _sets = [];
    public IReadOnlyCollection<SetLog> Sets => _sets.AsReadOnly();

    private ExerciseLog() { }

    public static ExerciseLog Create(Guid workoutSessionId, Guid exerciseId, int order)
    {
        return new ExerciseLog
        {
            Id = Guid.NewGuid(),
            WorkoutSessionId = workoutSessionId,
            ExerciseId = exerciseId,
            Order = order
        };
    }

    public void AddSet(SetLog set) => _sets.Add(set);
}
