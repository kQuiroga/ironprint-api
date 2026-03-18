using IronPrint.Application.Commands.Auth.Revoke;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;

namespace IronPrint.Infrastructure.Identity;

public sealed class RevokeTokenHandler : IRequestHandler<RevokeTokenCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IRefreshTokenService _refreshTokenService;

    public RevokeTokenHandler(IRefreshTokenRepository refreshTokenRepo, IRefreshTokenService refreshTokenService)
    {
        _refreshTokenRepo = refreshTokenRepo;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result> Handle(RevokeTokenCommand cmd, CancellationToken ct)
    {
        var tokenHash = _refreshTokenService.HashToken(cmd.RefreshToken);
        var stored = await _refreshTokenRepo.GetByHashAsync(tokenHash, ct);

        // Idempotente: si el token no existe o ya fue revocado, devolvemos éxito.
        // No tiene sentido fallar en un logout.
        if (stored is null || stored.RevokedAt is not null)
            return Result.Success();

        await _refreshTokenRepo.RevokeAsync(stored.Id, ct);
        return Result.Success();
    }
}
