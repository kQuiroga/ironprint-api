using IronPrint.Application.Commands.Auth.Register;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwt;

    public RegisterHandler(UserManager<IdentityUser> userManager, IJwtTokenService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<Result<string>> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(cmd.Email);
        if (existing is not null) return Result.Failure<string>(Error.Conflict("User"));

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = cmd.Email,
            UserName = cmd.Email,
            EmailConfirmed = true
        };

        var identityResult = await _userManager.CreateAsync(user, cmd.Password);
        if (!identityResult.Succeeded)
        {
            var first = identityResult.Errors.First();
            return Result.Failure<string>(Error.Validation(first.Code, first.Description));
        }

        return Result.Success(_jwt.GenerateToken(user.Id, user.Email!));
    }
}
