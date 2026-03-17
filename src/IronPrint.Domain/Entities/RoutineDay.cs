namespace IronPrint.Domain.Entities;

public class RoutineDay
{
    public Guid Id { get; private set; }
    public Guid RoutineId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }

    private readonly List<RoutineExercise> _exercises = [];
    public IReadOnlyCollection<RoutineExercise> Exercises => _exercises.AsReadOnly();

    private RoutineDay() { }

    public static RoutineDay Create(Guid routineId, DayOfWeek dayOfWeek)
    {
        return new RoutineDay
        {
            Id = Guid.NewGuid(),
            RoutineId = routineId,
            DayOfWeek = dayOfWeek
        };
    }

    public void AddExercise(RoutineExercise exercise) => _exercises.Add(exercise);
}
