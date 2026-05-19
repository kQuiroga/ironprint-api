using FluentAssertions;
using IronPrint.Application.Commands.Routines.UpdateRoutine;
using IronPrint.Domain.Common;
using IronPrint.Domain.Entities;
using IronPrint.Domain.Ports;
using IronPrint.Domain.ValueObjects;
using NSubstitute;

namespace IronPrint.Tests.Commands.Routines;

public class UpdateRoutineHandlerTests
{
    private readonly IRoutineRepository _repo = Substitute.For<IRoutineRepository>();
    private readonly UpdateRoutineHandler _sut;

    public UpdateRoutineHandlerTests()
    {
        _sut = new UpdateRoutineHandler(_repo);
    }

    private static Routine CreateRoutine() =>
        Routine.Reconstitute(Guid.NewGuid(), "user-123", "Old Name", 4, false, DateTime.UtcNow);

    // Task 7.2 — Days null → UpdateAsync called, UpdateWithDaysAsync never called
    [Fact]
    public async Task Handle_DaysNull_CallsUpdateAsync_NotUpdateWithDaysAsync()
    {
        var routine = CreateRoutine();
        _repo.GetByIdAsync(routine.Id, "user-123", Arg.Any<CancellationToken>())
            .Returns(routine);

        var cmd = new UpdateRoutineCommand(routine.Id, "user-123", "New Name", 8, Days: null);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repo.Received(1).UpdateAsync(routine, Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateWithDaysAsync(Arg.Any<Routine>(), Arg.Any<CancellationToken>());
    }

    // Task 7.3 — Days with 2 days / 3 exercises each → UpdateWithDaysAsync called, UpdateAsync never called
    [Fact]
    public async Task Handle_DaysNotNull_CallsUpdateWithDaysAsync_NotUpdateAsync()
    {
        var routine = CreateRoutine();
        _repo.GetByIdAsync(routine.Id, "user-123", Arg.Any<CancellationToken>())
            .Returns(routine);

        var exercises = Enumerable.Range(1, 3)
            .Select(i => new UpdateRoutineExerciseDto(Guid.NewGuid(), i, 3, 8))
            .ToList();

        var days = new[]
        {
            new UpdateRoutineDayDto(DayOfWeek.Monday, null, [], exercises),
            new UpdateRoutineDayDto(DayOfWeek.Wednesday, null, [], exercises)
        };

        var cmd = new UpdateRoutineCommand(routine.Id, "user-123", "New Name", 8, days);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Routine>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).UpdateWithDaysAsync(
            Arg.Is<Routine>(r =>
                r.Days.Count == 2 &&
                r.Days.All(d => d.Exercises.Count == 3)),
            Arg.Any<CancellationToken>());
    }

    // Task 7.4 — Days empty → UpdateWithDaysAsync called with empty Days, UpdateAsync never called
    [Fact]
    public async Task Handle_DaysEmptyCollection_CallsUpdateWithDaysAsyncWithEmptyDays()
    {
        var routine = CreateRoutine();
        _repo.GetByIdAsync(routine.Id, "user-123", Arg.Any<CancellationToken>())
            .Returns(routine);

        var cmd = new UpdateRoutineCommand(routine.Id, "user-123", "New Name", 8, Days: []);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Routine>(), Arg.Any<CancellationToken>());
        await _repo.Received(1).UpdateWithDaysAsync(
            Arg.Is<Routine>(r => r.Days.Count == 0),
            Arg.Any<CancellationToken>());
    }

    // Task 7.5 — GetByIdAsync returns null → NotFound, no write call
    [Fact]
    public async Task Handle_RoutineNotFound_ReturnsNotFound_NoWriteCall()
    {
        var missingId = Guid.NewGuid();
        _repo.GetByIdAsync(missingId, "user-123", Arg.Any<CancellationToken>())
            .Returns((Routine?)null);

        var cmd = new UpdateRoutineCommand(missingId, "user-123", "Name", 4);

        var result = await _sut.Handle(cmd, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NotFound("Routine"));
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Routine>(), Arg.Any<CancellationToken>());
        await _repo.DidNotReceive().UpdateWithDaysAsync(Arg.Any<Routine>(), Arg.Any<CancellationToken>());
    }
}
