using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Application.Commands.WorkoutSessions.CreateWorkoutSession;

public sealed class CreateWorkoutSessionHandler : IRequestHandler<CreateWorkoutSessionCommand, Result<Guid>>
{
    private readonly IWorkoutSessionRepository _repo;

    public CreateWorkoutSessionHandler(IWorkoutSessionRepository repo) => _repo = repo;

    public async Task<Result<Guid>> Handle(CreateWorkoutSessionCommand cmd, CancellationToken ct)
    {
        var existing = await _repo.GetByDateAsync(cmd.UserId, cmd.Date, ct);
        if (existing is not null) return Result.Failure<Guid>(Error.Conflict("WorkoutSession"));

        var session = WorkoutSession.Create(cmd.UserId, cmd.Date, cmd.RoutineDayId);
        await _repo.AddAsync(session, ct);
        return Result.Success(session.Id);
    }
}
