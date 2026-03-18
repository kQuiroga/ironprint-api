using IronPrint.Application.Commands.Auth.Refresh;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokens>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwt;
    private readonly UserManager<IdentityUser> _userManager;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepo,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwt,
        UserManager<IdentityUser> userManager)
    {
        _refreshTokenRepo = refreshTokenRepo;
        _refreshTokenService = refreshTokenService;
        _jwt = jwt;
        _userManager = userManager;
    }

    public async Task<Result<AuthTokens>> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var tokenHash = _refreshTokenService.HashToken(cmd.RefreshToken);
        var stored = await _refreshTokenRepo.GetByHashAsync(tokenHash, ct);

        if (stored is null || !stored.IsValid)
            return Result.Failure<AuthTokens>(Error.Validation("RefreshToken", "Token inválido o expirado."));

        var user = await _userManager.FindByIdAsync(stored.UserId);
        if (user is null)
            return Result.Failure<AuthTokens>(Error.Validation("RefreshToken", "Token inválido o expirado."));

        // Token rotation: revocar el viejo, emitir uno nuevo
        await _refreshTokenRepo.RevokeAsync(stored.Id, ct);

        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Email!);
        var newRawToken = _refreshTokenService.GenerateToken();
        var newHash = _refreshTokenService.HashToken(newRawToken);
        await _refreshTokenRepo.AddAsync(user.Id, newHash, DateTime.UtcNow.AddDays(30), ct);

        return Result.Success(new AuthTokens(accessToken, newRawToken));
    }
}
