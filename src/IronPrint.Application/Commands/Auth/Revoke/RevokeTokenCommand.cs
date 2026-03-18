using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Auth.Revoke;

public record RevokeTokenCommand(string RefreshToken) : ICommand;
