using IronPrint.Domain.ValueObjects;

namespace IronPrint.Domain.Entities;

public class RoutineDay
{
    public Guid Id { get; private set; }
    public Guid RoutineId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public string? Name { get; private set; }

    private readonly List<MuscleGroup> _muscleGroups = [];
    public IReadOnlyCollection<MuscleGroup> MuscleGroups => _muscleGroups.AsReadOnly();

    private readonly List<RoutineExercise> _exercises = [];
    public IReadOnlyCollection<RoutineExercise> Exercises => _exercises.AsReadOnly();

    private RoutineDay() { }

    public static RoutineDay Create(Guid routineId, DayOfWeek dayOfWeek, string? name = null, IEnumerable<MuscleGroup>? muscleGroups = null)
    {
        var day = new RoutineDay
        {
            Id = Guid.NewGuid(),
            RoutineId = routineId,
            DayOfWeek = dayOfWeek,
            Name = name
        };
        if (muscleGroups is not null)
            day._muscleGroups.AddRange(muscleGroups);
        return day;
    }

    public void AddExercise(RoutineExercise exercise) => _exercises.Add(exercise);

    public static RoutineDay Reconstitute(Guid id, Guid routineId, DayOfWeek dayOfWeek, string? name, IEnumerable<MuscleGroup>? muscleGroups, IEnumerable<RoutineExercise>? exercises = null)
    {
        var day = new RoutineDay
        {
            Id = id,
            RoutineId = routineId,
            DayOfWeek = dayOfWeek,
            Name = name
        };
        if (muscleGroups is not null)
            day._muscleGroups.AddRange(muscleGroups);
        if (exercises is not null)
            foreach (var ex in exercises)
                day._exercises.Add(ex);
        return day;
    }
}
