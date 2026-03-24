using FluentValidation;

namespace IronPrint.Application.Commands.DayLogs.DeleteDayLog;

public sealed class DeleteDayLogValidator : AbstractValidator<DeleteDayLogCommand>
{
    public DeleteDayLogValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
    }
}
