using IronPrint.Api.Extensions;
using IronPrint.Application.Commands.Auth.Login;
using IronPrint.Application.Commands.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IronPrint.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", Register).WithName("Register");
        group.MapPost("/login", Login).WithName("Login");
    }

    private static async Task<IResult> Register([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new RegisterCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { token = result.Value })
            : result.ToHttpResult();
    }

    private static async Task<IResult> Login([FromBody] AuthRequest req, ISender sender)
    {
        var result = await sender.Send(new LoginCommand(req.Email, req.Password));
        return result.IsSuccess
            ? Results.Ok(new { token = result.Value })
            : result.ToHttpResult();
    }

    private record AuthRequest(string Email, string Password);
}
