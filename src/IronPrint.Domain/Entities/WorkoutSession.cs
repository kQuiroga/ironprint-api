namespace IronPrint.Domain.Entities;

public class WorkoutSession
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public DateOnly Date { get; private set; }
    public Guid? RoutineDayId { get; private set; }

    private readonly List<ExerciseLog> _exerciseLogs = [];
    public IReadOnlyCollection<ExerciseLog> ExerciseLogs => _exerciseLogs.AsReadOnly();

    private WorkoutSession() { }

    public static WorkoutSession Create(string userId, DateOnly date, Guid? routineDayId = null)
    {
        return new WorkoutSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = date,
            RoutineDayId = routineDayId
        };
    }

    public void AddExerciseLog(ExerciseLog log) => _exerciseLogs.Add(log);
}
