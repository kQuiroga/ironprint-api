using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Routines.CreateRoutine;
using IronPrint.Application.Commands.Routines.DeleteRoutine;
using IronPrint.Application.Commands.Routines.UpdateRoutine;
using IronPrint.Application.Queries.Routines.GetRoutineById;
using IronPrint.Application.Queries.Routines.GetRoutines;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IronPrint.Api.Endpoints;

public static class RoutineEndpoints
{
    public static void MapRoutineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/routines").RequireAuthorization();

        group.MapGet("/", GetAll).WithName("GetRoutines");
        group.MapGet("/{id:guid}", GetById).WithName("GetRoutineById");
        group.MapPost("/", Create).WithName("CreateRoutine");
        group.MapPut("/{id:guid}", Update).WithName("UpdateRoutine");
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteRoutine");
    }

    private static async Task<IResult> GetAll(ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetRoutinesQuery(user.GetUserId()));
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetById(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetRoutineByIdQuery(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    private static async Task<IResult> Create(
        [FromBody] CreateRoutineRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new CreateRoutineCommand(user.GetUserId(), req.Name, req.WeeksDuration));
        return result.ToCreatedResult("GetRoutineById", new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    private static async Task<IResult> Update(
        Guid id, [FromBody] UpdateRoutineRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new UpdateRoutineCommand(id, user.GetUserId(), req.Name, req.WeeksDuration));
        return result.ToHttpResult();
    }

    private static async Task<IResult> Delete(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new DeleteRoutineCommand(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    private record CreateRoutineRequest(string Name, int WeeksDuration);
    private record UpdateRoutineRequest(string Name, int WeeksDuration);
}
