using IronPrint.Application.Commands.Auth.Register;
using IronPrint.Application.Common;
using IronPrint.Domain.Common;
using IronPrint.Domain.Ports;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthTokens>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwt;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public RegisterHandler(
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

    public async Task<Result<AuthTokens>> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(cmd.Email);
        if (existing is not null) return Result.Failure<AuthTokens>(Error.Conflict("User"));

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
            return Result.Failure<AuthTokens>(Error.Validation(first.Code, first.Description));
        }

        var accessToken = _jwt.GenerateAccessToken(user.Id, user.Email!);

        var rawRefreshToken = _refreshTokenService.GenerateToken();
        var tokenHash = _refreshTokenService.HashToken(rawRefreshToken);
        await _refreshTokenRepo.AddAsync(user.Id, tokenHash, DateTime.UtcNow.AddDays(30), ct);

        return Result.Success(new AuthTokens(accessToken, rawRefreshToken));
    }
}
