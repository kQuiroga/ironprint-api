using IronPrint.Domain.Common;

namespace IronPrint.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : MapError(result.Error);

    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? Results.Ok(result.Value)
            : MapError(result.Error);

    public static IResult ToCreatedResult<T>(this Result<T> result, string routeName, object? routeValues = null) =>
        result.IsSuccess
            ? Results.CreatedAtRoute(routeName, routeValues ?? new { id = result.Value }, result.Value)
            : MapError(result.Error);

    private static IResult MapError(Error error)
    {
        if (error.Code.EndsWith(".NotFound"))
            return Results.NotFound(new { error.Code, error.Description });

        if (error.Code.EndsWith(".Conflict"))
            return Results.Conflict(new { error.Code, error.Description });

        if (error.Code.StartsWith("Validation."))
            return Results.UnprocessableEntity(new { error.Code, error.Description });

        return Results.BadRequest(new { error.Code, error.Description });
    }
}
