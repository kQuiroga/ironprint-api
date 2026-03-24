using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using MediatR;

namespace IronPrint.Application.Queries.WorkoutSessions.GetWorkoutCalendar;

public sealed class GetWorkoutCalendarHandler : IRequestHandler<GetWorkoutCalendarQuery, Result<IEnumerable<WorkoutCalendarEntryDto>>>
{
    private readonly IWorkoutSessionRepository _sessionRepo;
    private readonly IDayLogRepository _dayLogRepo;
    private readonly IRoutineRepository _routineRepo;

    public GetWorkoutCalendarHandler(IWorkoutSessionRepository sessionRepo, IDayLogRepository dayLogRepo, IRoutineRepository routineRepo)
    {
        _sessionRepo = sessionRepo;
        _dayLogRepo = dayLogRepo;
        _routineRepo = routineRepo;
    }

    public async Task<Result<IEnumerable<WorkoutCalendarEntryDto>>> Handle(GetWorkoutCalendarQuery query, CancellationToken ct)
    {
        var sessionsTask = _sessionRepo.GetByDateRangeAsync(query.UserId, query.From, query.To, ct);
        var dayLogsTask = _dayLogRepo.GetByDateRangeAsync(query.UserId, query.From, query.To, ct);
        var activeRoutineTask = _routineRepo.GetActiveByUserIdAsync(query.UserId, ct);

        await Task.WhenAll(sessionsTask, dayLogsTask, activeRoutineTask);

        var sessionsByDate = sessionsTask.Result.ToDictionary(s => s.Date, s => s.Id);
        var dayLogsByDate = dayLogsTask.Result.ToDictionary(d => d.Date, d => d.Status);
        var plannedDays = activeRoutineTask.Result?.Days.Select(d => d.DayOfWeek).ToHashSet() ?? [];

        var entries = Enumerable
            .Range(0, query.To.DayNumber - query.From.DayNumber + 1)
            .Select(offset =>
            {
                var date = query.From.AddDays(offset);
                var hasSession = sessionsByDate.TryGetValue(date, out var sessionId);
                var hasDayLog = dayLogsByDate.TryGetValue(date, out var dayLogStatus);
                var isPlanned = plannedDays.Contains((DayOfWeek)date.DayOfWeek);
                return new WorkoutCalendarEntryDto(
                    date,
                    hasSession,
                    hasSession ? sessionId : null,
                    hasDayLog ? dayLogStatus : null,
                    isPlanned);
            });

        return Result.Success(entries);
    }
}
