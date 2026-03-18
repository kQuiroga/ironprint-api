using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Auth.Refresh;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthTokens>;
