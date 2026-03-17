using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.WorkoutSessions.CreateWorkoutSession;
using IronPrint.Application.Commands.WorkoutSessions.LogSet;
using IronPrint.Application.Queries.WorkoutSessions.GetWorkoutCalendar;
using IronPrint.Application.Queries.WorkoutSessions.GetWorkoutSessionByDate;
using IronPrint.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IronPrint.Api.Endpoints;

public static class WorkoutSessionEndpoints
{
    public static void MapWorkoutSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/workout-sessions").RequireAuthorization();

        group.MapGet("/calendar", GetCalendar).WithName("GetWorkoutCalendar");
        group.MapGet("/{date}", GetByDate).WithName("GetWorkoutSessionByDate");
        group.MapPost("/", Create).WithName("CreateWorkoutSession");
        group.MapPost("/{id:guid}/sets", LogSet).WithName("LogSet");
    }

    private static async Task<IResult> GetCalendar(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to,
        ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetWorkoutCalendarQuery(user.GetUserId(), from, to));
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetByDate(DateOnly date, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetWorkoutSessionByDateQuery(user.GetUserId(), date));
        return result.ToHttpResult();
    }

    private static async Task<IResult> Create(
        [FromBody] CreateWorkoutSessionRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new CreateWorkoutSessionCommand(user.GetUserId(), req.Date, req.RoutineDayId));
        return result.ToCreatedResult("GetWorkoutSessionByDate", new { date = result.IsSuccess ? req.Date.ToString() : string.Empty });
    }

    private static async Task<IResult> LogSet(
        Guid id, [FromBody] LogSetRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new LogSetCommand(
            user.GetUserId(), id, req.ExerciseId,
            req.SetNumber, req.WeightValue, req.WeightUnit, req.Reps, req.Completed));
        return result.ToHttpResult();
    }

    private record CreateWorkoutSessionRequest(DateOnly Date, Guid? RoutineDayId);
    private record LogSetRequest(Guid ExerciseId, int SetNumber, decimal WeightValue, WeightUnit WeightUnit, int Reps, bool Completed);
}
