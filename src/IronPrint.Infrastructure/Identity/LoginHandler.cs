using IronPrint.Application.Commands.Auth.Login;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<AuthTokens>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public LoginHandler(
        UserManager<IdentityUser> userManager,
        IJwtTokenService jwt,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepo)
    {
        _userManager = userManager;
        _jwt = jwt;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepo = refreshTokenRepo;
    }

    public async Task<Result<AuthTokens>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(cmd.Email);
        if (user is null) return Result.Failure<AuthTokens>(Error.Validation("Credentials", "Email o contraseña incorrectos."));

        var valid = await _userManager.CheckPasswordAsync(user, cmd.Password);
        if (!valid) return Result.Failure<AuthTokens>(Error.Validation("Credentials", "Email o contraseña incorrectos."));

        return Result.Success(await GenerateTokensAsync(user, ct));
    }

    private async Task<AuthTokens> GenerateTokensAsync(IdentityUser user, CancellationToken ct)
    {
        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Email!);

        var rawRefreshToken = _refreshTokenService.GenerateToken();
        var tokenHash = _refreshTokenService.HashToken(rawRefreshToken);
        await _refreshTokenRepo.AddAsync(user.Id, tokenHash, DateTime.UtcNow.AddDays(30), ct);

        return new AuthTokens(accessToken, rawRefreshToken);
    }
}
