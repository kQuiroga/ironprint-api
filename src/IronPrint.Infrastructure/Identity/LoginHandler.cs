using IronPrint.Application.Commands.Auth.Login;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwt;

    public LoginHandler(UserManager<IdentityUser> userManager, IJwtTokenService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<Result<string>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(cmd.Email);
        if (user is null) return Result.Failure<string>(Error.Validation("Credentials", "Email o contraseña incorrectos."));

        var valid = await _userManager.CheckPasswordAsync(user, cmd.Password);
        if (!valid) return Result.Failure<string>(Error.Validation("Credentials", "Email o contraseña incorrectos."));

        return Result.Success(_jwt.GenerateToken(user.Id, user.Email!));
    }
}
