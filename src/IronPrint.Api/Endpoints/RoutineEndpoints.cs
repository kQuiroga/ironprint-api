using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Routines.ActivateRoutine;
using IronPrint.Application.Commands.Routines.DeactivateRoutine;
using IronPrint.Application.Commands.Routines.CreateRoutine;
using IronPrint.Application.Commands.Routines.DeleteRoutine;
using IronPrint.Application.Commands.Routines.UpdateRoutine;
using IronPrint.Application.Queries.Routines.GetActiveRoutine;
using IronPrint.Application.Queries.Routines.GetRoutineById;
using IronPrint.Application.Queries.Routines.GetRoutines;
using IronPrint.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IronPrint.Api.Endpoints;

/// <summary>
/// Endpoints de gestión de rutinas de entrenamiento.
/// Una rutina define los días de la semana y los ejercicios asignados a cada día.
/// Todos los endpoints requieren autenticación.
/// </summary>
public static class RoutineEndpoints
{
    public static void MapRoutineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/routines").RequireAuthorization();

        group.MapGet("/", GetAll).WithName("GetRoutines");
        group.MapGet("/active", GetActive).WithName("GetActiveRoutine");
        group.MapGet("/{id:guid}", GetById).WithName("GetRoutineById");
        group.MapPost("/", Create).WithName("CreateRoutine");
        group.MapPost("/{id:guid}/activate", Activate).WithName("ActivateRoutine");
        group.MapPost("/{id:guid}/deactivate", Deactivate).WithName("DeactivateRoutine");
        group.MapPut("/{id:guid}", Update).WithName("UpdateRoutine");
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteRoutine");
    }

    /// <summary>
    /// Devuelve la rutina activa del usuario, o 204 No Content si no hay ninguna.
    /// </summary>
    private static async Task<IResult> GetActive(ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetActiveRoutineQuery(user.GetUserId()));
        if (!result.IsSuccess) return Results.Problem();
        return result.Value is null ? Results.NoContent() : Results.Ok(result.Value);
    }

    /// <summary>
    /// Activa una rutina y desactiva la anterior si existía.
    /// </summary>
    private static async Task<IResult> Activate(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new ActivateRoutineCommand(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Desactiva una rutina sin activar ninguna otra.
    /// </summary>
    private static async Task<IResult> Deactivate(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new DeactivateRoutineCommand(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Devuelve todas las rutinas del usuario autenticado.
    /// </summary>
    private static async Task<IResult> GetAll(ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetRoutinesQuery(user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Devuelve una rutina por su ID incluyendo sus días y ejercicios asignados.
    /// Falla con 404 si no existe o pertenece a otro usuario.
    /// </summary>
    private static async Task<IResult> GetById(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetRoutineByIdQuery(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Crea una nueva rutina con nombre, duración en semanas y días opcionales con ejercicios.
    /// Devuelve 201 Created con la URL de la rutina creada.
    /// </summary>
    private static async Task<IResult> Create(
        [FromBody] CreateRoutineRequest req, ISender sender, ClaimsPrincipal user)
    {
        var cmd = new CreateRoutineCommand(
            user.GetUserId(), req.Name, req.WeeksDuration,
            req.Days?.Select(d => new CreateRoutineDayDto(
                d.DayOfWeek, d.Name, d.MuscleGroups ?? [],
                d.Exercises.Select(e => new CreateRoutineExerciseDto(e.ExerciseId, e.Order, e.TargetSets, e.TargetReps))
            ))
        );
        var result = await sender.Send(cmd);
        return result.ToCreatedResult("GetRoutineById", new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    /// <summary>
    /// Actualiza el nombre y duración de una rutina existente.
    /// Falla con 404 si la rutina no pertenece al usuario.
    /// </summary>
    private static async Task<IResult> Update(
        Guid id, [FromBody] UpdateRoutineRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new UpdateRoutineCommand(id, user.GetUserId(), req.Name, req.WeeksDuration));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Elimina una rutina y todos sus días y ejercicios asociados.
    /// Falla con 404 si no pertenece al usuario.
    /// </summary>
    private static async Task<IResult> Delete(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new DeleteRoutineCommand(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    private record CreateRoutineExerciseRequest(Guid ExerciseId, int Order, int TargetSets, int TargetReps);
    private record CreateRoutineDayRequest(DayOfWeek DayOfWeek, string? Name, IEnumerable<MuscleGroup>? MuscleGroups, IEnumerable<CreateRoutineExerciseRequest> Exercises);
    private record CreateRoutineRequest(string Name, int WeeksDuration, IEnumerable<CreateRoutineDayRequest>? Days = null);
    private record UpdateRoutineRequest(string Name, int WeeksDuration);
}
