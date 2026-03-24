using IronPrint.Domain.ValueObjects;

namespace IronPrint.Domain.Entities;

public class DayLog
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public DateOnly Date { get; private set; }
    public DayLogStatus Status { get; private set; }

    private DayLog() { }

    public static DayLog Create(string userId, DateOnly date, DayLogStatus status)
    {
        return new DayLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = date,
            Status = status
        };
    }

    public void UpdateStatus(DayLogStatus status) => Status = status;

    public static DayLog Reconstitute(Guid id, string userId, DateOnly date, DayLogStatus status)
    {
        return new DayLog
        {
            Id = id,
            UserId = userId,
            Date = date,
            Status = status
        };
    }
}
