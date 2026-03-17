using FluentValidation;

namespace IronPrint.Application.Commands.WorkoutSessions.CreateWorkoutSession;

public sealed class CreateWorkoutSessionValidator : AbstractValidator<CreateWorkoutSessionCommand>
{
    public CreateWorkoutSessionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
