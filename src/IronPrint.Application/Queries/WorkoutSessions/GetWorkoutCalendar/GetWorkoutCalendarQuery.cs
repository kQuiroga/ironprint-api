using IronPrint.Application.Common;

namespace IronPrint.Application.Queries.WorkoutSessions.GetWorkoutCalendar;

public record GetWorkoutCalendarQuery(string UserId, DateOnly From, DateOnly To) : IQuery<IEnumerable<WorkoutCalendarEntryDto>>;
