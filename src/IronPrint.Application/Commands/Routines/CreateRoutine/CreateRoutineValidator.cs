using FluentValidation;

namespace IronPrint.Application.Commands.Routines.CreateRoutine;

public sealed class CreateRoutineValidator : AbstractValidator<CreateRoutineCommand>
{
    public CreateRoutineValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WeeksDuration).GreaterThan(0).LessThanOrEqualTo(52);
    }
}
