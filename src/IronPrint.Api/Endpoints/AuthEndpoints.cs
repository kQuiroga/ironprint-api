using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Auth.Login;
using IronPrint.Application.Commands.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IronPrint.Api.Endpoints;

/// <summary>
/// Endpoints de autenticación. No requieren token — son el punto de entrada al sistema.
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", Register).WithName("Register").RequireRateLimiting("auth-register");
        group.MapPost("/login", Login).WithName("Login").RequireRateLimiting("auth-login");
    }

    /// <summary>
    /// Registra un nuevo usuario y devuelve un JWT.
    /// Falla con 409 si el email ya existe, o 422 si la contraseña no cumple los requisitos.
    /// </summary>
    private static async Task<IResult> Register([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new RegisterCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { token = result.Value })
            : result.ToHttpResult();
    }

    /// <summary>
    /// Autentica un usuario existente y devuelve un JWT.
    /// Falla con 422 si las credenciales son incorrectas (mismo mensaje para email y contraseña
    /// para no dar pistas sobre qué campo falló).
    /// </summary>
    private static async Task<IResult> Login([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new LoginCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { token = result.Value })
            : result.ToHttpResult();
    }

    private record AuthRequest(string Email, string Password);
}
