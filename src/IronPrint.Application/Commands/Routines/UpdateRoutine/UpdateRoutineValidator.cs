using FluentValidation;

namespace IronPrint.Application.Commands.Routines.UpdateRoutine;

public sealed class UpdateRoutineValidator : AbstractValidator<UpdateRoutineCommand>
{
    public UpdateRoutineValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WeeksDuration).GreaterThan(0).LessThanOrEqualTo(52);

        When(x => x.Days != null, () =>
        {
            RuleForEach(x => x.Days).SetValidator(new UpdateRoutineDayDtoValidator());
        });
    }
}

internal sealed class UpdateRoutineDayDtoValidator : AbstractValidator<UpdateRoutineDayDto>
{
    public UpdateRoutineDayDtoValidator()
    {
        RuleFor(x => x.DayOfWeek).IsInEnum();
        RuleFor(x => x.Name).MaximumLength(100).When(x => x.Name != null);
        RuleFor(x => x.MuscleGroups).NotNull();

        RuleForEach(x => x.Exercises).SetValidator(new UpdateRoutineExerciseDtoValidator());
    }
}

internal sealed class UpdateRoutineExerciseDtoValidator : AbstractValidator<UpdateRoutineExerciseDto>
{
    public UpdateRoutineExerciseDtoValidator()
    {
        RuleFor(x => x.ExerciseId).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TargetSets).GreaterThan(0);
        RuleFor(x => x.TargetReps).GreaterThan(0);
    }
}
