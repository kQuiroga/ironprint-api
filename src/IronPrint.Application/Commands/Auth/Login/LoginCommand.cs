using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AuthTokens>;
