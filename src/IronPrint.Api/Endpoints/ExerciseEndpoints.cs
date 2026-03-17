using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Exercises.CreateExercise;
using IronPrint.Application.Commands.Exercises.DeleteExercise;
using IronPrint.Application.Commands.Exercises.UpdateExercise;
using IronPrint.Application.Queries.Exercises;
using IronPrint.Application.Queries.Exercises.GetExerciseById;
using IronPrint.Application.Queries.Exercises.GetExercises;
using IronPrint.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IronPrint.Api.Endpoints;

/// <summary>
/// Endpoints del catálogo personal de ejercicios.
/// Cada usuario gestiona sus propios ejercicios — no se comparten entre usuarios.
/// Todos los endpoints requieren autenticación.
/// </summary>
public static class ExerciseEndpoints
{
    public static void MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/exercises").RequireAuthorization();

        group.MapGet("/", GetAll).WithName("GetExercises");
        group.MapGet("/{id:guid}", GetById).WithName("GetExerciseById");
        group.MapPost("/", Create).WithName("CreateExercise");
        group.MapPut("/{id:guid}", Update).WithName("UpdateExercise");
        group.MapDelete("/{id:guid}", Delete).WithName("DeleteExercise");
    }

    /// <summary>
    /// Devuelve todos los ejercicios del usuario autenticado.
    /// </summary>
    private static async Task<IResult> GetAll(ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetExercisesQuery(user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Devuelve un ejercicio por su ID. Falla con 404 si no existe o pertenece a otro usuario.
    /// </summary>
    private static async Task<IResult> GetById(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetExerciseByIdQuery(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Crea un nuevo ejercicio en el catálogo personal del usuario.
    /// Devuelve 201 Created con la URL del recurso creado.
    /// </summary>
    private static async Task<IResult> Create(
        [FromBody] CreateExerciseRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new CreateExerciseCommand(user.GetUserId(), req.Name, req.MuscleGroup, req.Notes));
        return result.ToCreatedResult("GetExerciseById", new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    /// <summary>
    /// Actualiza el nombre, grupo muscular y notas de un ejercicio existente.
    /// Falla con 404 si el ejercicio no pertenece al usuario.
    /// </summary>
    private static async Task<IResult> Update(
        Guid id, [FromBody] UpdateExerciseRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new UpdateExerciseCommand(id, user.GetUserId(), req.Name, req.MuscleGroup, req.Notes));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Elimina un ejercicio del catálogo. Falla con 404 si no pertenece al usuario.
    /// </summary>
    private static async Task<IResult> Delete(Guid id, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new DeleteExerciseCommand(id, user.GetUserId()));
        return result.ToHttpResult();
    }

    private record CreateExerciseRequest(string Name, MuscleGroup MuscleGroup, string? Notes);
    private record UpdateExerciseRequest(string Name, MuscleGroup MuscleGroup, string? Notes);
}
