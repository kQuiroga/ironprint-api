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

/// <summary>
/// Endpoints de sesiones de entrenamiento.
/// Una sesión representa un entrenamiento completado en una fecha concreta.
/// Dentro de cada sesión se registran los ejercicios y series realizadas (sets).
/// Todos los endpoints requieren autenticación.
/// </summary>
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

    /// <summary>
    /// Devuelve un listado de fechas con sesión registrada dentro del rango indicado.
    /// Útil para pintar el calendario de entrenamientos en el frontend.
    /// Parámetros: from y to en formato yyyy-MM-dd.
    /// </summary>
    private static async Task<IResult> GetCalendar(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to,
        ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetWorkoutCalendarQuery(user.GetUserId(), from, to));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Devuelve el detalle completo de una sesión: ejercicios y series registradas.
    /// Parámetro date en formato yyyy-MM-dd. Falla con 404 si no hay sesión ese día.
    /// </summary>
    private static async Task<IResult> GetByDate(DateOnly date, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new GetWorkoutSessionByDateQuery(user.GetUserId(), date));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Crea una nueva sesión de entrenamiento para la fecha indicada.
    /// Opcionalmente puede asociarse a un día de rutina (RoutineDayId).
    /// Falla con 409 si ya existe una sesión para esa fecha.
    /// </summary>
    private static async Task<IResult> Create(
        [FromBody] CreateWorkoutSessionRequest req, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new CreateWorkoutSessionCommand(user.GetUserId(), req.Date, req.RoutineDayId));
        return result.ToCreatedResult("GetWorkoutSessionByDate", new { date = result.IsSuccess ? req.Date.ToString() : string.Empty });
    }

    /// <summary>
    /// Registra una serie (set) dentro de una sesión de entrenamiento.
    /// Cada set indica el ejercicio, número de serie, peso, unidad (kg/lb), repeticiones
    /// y si fue completado o no.
    /// </summary>
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
