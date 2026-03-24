using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.DayLogs.DeleteDayLog;
using IronPrint.Application.Commands.DayLogs.UpsertDayLog;
using IronPrint.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IronPrint.Api.Endpoints;

/// <summary>
/// Endpoints para marcar días como completados o no completados.
/// Un DayLog es independiente de WorkoutSession — permite marcar un día sin crear sesión.
/// Solo se pueden marcar días presentes o pasados (no futuros).
/// Todos los endpoints requieren autenticación.
/// </summary>
public static class DayLogEndpoints
{
    public static void MapDayLogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/day-logs").RequireAuthorization();

        group.MapPut("/{date}", Upsert).WithName("UpsertDayLog");
        group.MapDelete("/{date}", Delete).WithName("DeleteDayLog");
    }

    /// <summary>
    /// Crea o actualiza el estado de un día (completado/no completado).
    /// Upsert: si ya existe un log para esa fecha, lo sobreescribe.
    /// Falla con 422 si la fecha es futura.
    /// </summary>
    private static async Task<IResult> Upsert(
        DateOnly date, [FromBody] UpsertDayLogRequest req,
        ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new UpsertDayLogCommand(user.GetUserId(), date, req.Status));
        return result.ToHttpResult();
    }

    /// <summary>
    /// Elimina el log de un día, dejándolo sin estado.
    /// Idempotente: no falla si no existe log para esa fecha.
    /// </summary>
    private static async Task<IResult> Delete(
        DateOnly date, ISender sender, ClaimsPrincipal user)
    {
        var result = await sender.Send(new DeleteDayLogCommand(user.GetUserId(), date));
        return result.ToHttpResult();
    }

    private record UpsertDayLogRequest(DayLogStatus Status);
}
