using IronPrint.Domain.ValueObjects;

namespace IronPrint.Domain.Entities;

public class Exercise
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public MuscleGroup MuscleGroup { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Exercise() { }

    public static Exercise Create(string userId, string name, MuscleGroup muscleGroup, string? notes = null)
    {
        return new Exercise
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            MuscleGroup = muscleGroup,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, MuscleGroup muscleGroup, string? notes)
    {
        Name = name;
        MuscleGroup = muscleGroup;
        Notes = notes;
    }

    public static Exercise Reconstitute(Guid id, string userId, string name, MuscleGroup muscleGroup, string? notes, DateTime createdAt)
    {
        return new Exercise
        {
            Id = id,
            UserId = userId,
            Name = name,
            MuscleGroup = muscleGroup,
            Notes = notes,
            CreatedAt = createdAt
        };
    }
}
