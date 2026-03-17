using FluentValidation;
using IronPrint.Domain.Common;
using MediatR;

namespace IronPrint.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next(ct);

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0) return await next(ct);

        // Si TResponse es Result o Result<T>, devolvemos Failure
        var firstError = failures[0];
        var error = Error.Validation(firstError.PropertyName, firstError.ErrorMessage);

        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var resultType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result).GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
            .MakeGenericMethod(resultType);

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}
