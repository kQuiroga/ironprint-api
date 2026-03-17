using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Queries.WorkoutSessions.GetWorkoutCalendar;

public sealed class GetWorkoutCalendarHandler : IRequestHandler<GetWorkoutCalendarQuery, Result<IEnumerable<WorkoutCalendarEntryDto>>>
{
    private readonly IWorkoutSessionRepository _repo;

    public GetWorkoutCalendarHandler(IWorkoutSessionRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<WorkoutCalendarEntryDto>>> Handle(GetWorkoutCalendarQuery query, CancellationToken ct)
    {
        var sessions = await _repo.GetByDateRangeAsync(query.UserId, query.From, query.To, ct);
        var sessionsByDate = sessions.ToDictionary(s => s.Date, s => s.Id);

        var entries = Enumerable
            .Range(0, query.To.DayNumber - query.From.DayNumber + 1)
            .Select(offset =>
            {
                var date = query.From.AddDays(offset);
                var hasSession = sessionsByDate.TryGetValue(date, out var sessionId);
                return new WorkoutCalendarEntryDto(date, hasSession, hasSession ? sessionId : null);
            });

        return Result.Success(entries);
    }
}
