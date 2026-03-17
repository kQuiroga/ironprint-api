using FluentValidation;

namespace IronPrint.Application.Commands.Exercises.CreateExercise;

public sealed class CreateExerciseValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.MuscleGroup).IsInEnum();
    }
}
