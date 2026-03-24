namespace IronPrint.Domain.Entities;

public class Routine
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int WeeksDuration { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<RoutineDay> _days = [];
    public IReadOnlyCollection<RoutineDay> Days => _days.AsReadOnly();

    private Routine() { }

    public static Routine Create(string userId, string name, int weeksDuration)
    {
        return new Routine
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            WeeksDuration = weeksDuration,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Activate() => IsActive = true;

    public void Update(string name, int weeksDuration)
    {
        Name = name;
        WeeksDuration = weeksDuration;
    }

    public void AddDay(RoutineDay day) => _days.Add(day);

    public static Routine Reconstitute(Guid id, string userId, string name, int weeksDuration, bool isActive, DateTime createdAt, IEnumerable<RoutineDay>? days = null)
    {
        var routine = new Routine
        {
            Id = id,
            UserId = userId,
            Name = name,
            WeeksDuration = weeksDuration,
            IsActive = isActive,
            CreatedAt = createdAt
        };
        if (days is not null)
            foreach (var day in days)
                routine._days.Add(day);
        return routine;
    }
}
