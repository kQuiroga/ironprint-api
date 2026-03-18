namespace IronPrint.Application.Common;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string email);
}
