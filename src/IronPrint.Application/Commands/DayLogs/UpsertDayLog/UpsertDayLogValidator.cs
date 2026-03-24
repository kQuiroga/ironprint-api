using FluentValidation;

namespace IronPrint.Application.Commands.DayLogs.UpsertDayLog;

public sealed class UpsertDayLogValidator : AbstractValidator<UpsertDayLogCommand>
{
    public UpsertDayLogValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty()
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("No se puede marcar un día futuro.");
        RuleFor(x => x.Status).IsInEnum();
    }
}
