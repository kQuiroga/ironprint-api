using FluentAssertions;
using IronPrint.Application.Commands.Routines.UpdateRoutine;
using IronPrint.Domain.ValueObjects;

namespace IronPrint.Tests.Commands.Routines;

public class UpdateRoutineValidatorTests
{
    private readonly UpdateRoutineValidator _sut = new();

    private static UpdateRoutineCommand ValidCommand(IEnumerable<UpdateRoutineDayDto>? days = null) =>
        new(Guid.NewGuid(), "user-123", "My Routine", 4, days);

    [Fact]
    public void Validate_NullDays_IsValid()
    {
        var cmd = ValidCommand(days: null);

        var result = _sut.Validate(cmd);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var cmd = new UpdateRoutineCommand(Guid.NewGuid(), "user-123", "", 4);

        var result = _sut.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRoutineCommand.Name));
    }

    [Fact]
    public void Validate_WeeksDurationZero_HasError()
    {
        var cmd = new UpdateRoutineCommand(Guid.NewGuid(), "user-123", "Name", 0);

        var result = _sut.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateRoutineCommand.WeeksDuration));
    }

    [Fact]
    public void Validate_ExerciseWithEmptyExerciseId_HasErrorAtCorrectPath()
    {
        var exercise = new UpdateRoutineExerciseDto(Guid.Empty, 1, 3, 8);
        var day = new UpdateRoutineDayDto(DayOfWeek.Monday, null, [], [exercise]);
        var cmd = ValidCommand([day]);

        var result = _sut.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("ExerciseId"));
    }

    [Fact]
    public void Validate_FullyValidCommandWithDays_IsValid()
    {
        var exercise = new UpdateRoutineExerciseDto(Guid.NewGuid(), 1, 4, 10);
        var day = new UpdateRoutineDayDto(DayOfWeek.Monday, "Push day", [MuscleGroup.Chest], [exercise]);
        var cmd = ValidCommand([day]);

        var result = _sut.Validate(cmd);

        result.IsValid.Should().BeTrue();
    }
}
