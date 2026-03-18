using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Auth.Login;
using IronPrint.Application.Commands.Auth.Refresh;
using IronPrint.Application.Commands.Auth.Register;
using IronPrint.Application.Commands.Auth.Revoke;
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
        group.MapPost("/refresh", Refresh).WithName("RefreshToken");
        group.MapPost("/revoke", Revoke).WithName("RevokeToken");
    }

    /// <summary>
    /// Registra un nuevo usuario y devuelve un par de tokens (access + refresh).
    /// Falla con 409 si el email ya existe, o 422 si la contraseña no cumple los requisitos.
    /// </summary>
    private static async Task<IResult> Register([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new RegisterCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken })
            : result.ToHttpResult();
    }

    /// <summary>
    /// Autentica un usuario existente y devuelve un par de tokens (access + refresh).
    /// Falla con 422 si las credenciales son incorrectas.
    /// </summary>
    private static async Task<IResult> Login([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new LoginCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken })
            : result.ToHttpResult();
    }

    /// <summary>
    /// Emite un nuevo par de tokens a partir de un refresh token válido (token rotation).
    /// Falla con 422 si el refresh token es inválido, revocado o expirado.
    /// </summary>
    private static async Task<IResult> Refresh([FromBody] RefreshRequest req, ISender sender)
    {
        var result = await sender.Send(new RefreshTokenCommand(req.RefreshToken));
        return result.IsSuccess
            ? Results.Ok(new { accessToken = result.Value.AccessToken, refreshToken = result.Value.RefreshToken })
            : result.ToHttpResult();
    }

    /// <summary>
    /// Revoca un refresh token (logout). Idempotente — no falla si el token ya fue revocado.
    /// </summary>
    private static async Task<IResult> Revoke([FromBody] RefreshRequest req, ISender sender)
    {
        var result = await sender.Send(new RevokeTokenCommand(req.RefreshToken));
        return result.ToHttpResult();
    }

    private record AuthRequest(string Email, string Password);
    private record RefreshRequest(string RefreshToken);
}
