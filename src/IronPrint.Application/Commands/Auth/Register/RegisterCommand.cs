using IronPrint.Application.Common;

namespace IronPrint.Application.Commands.Auth.Register;

public record RegisterCommand(string Email, string Password) : ICommand<string>;
