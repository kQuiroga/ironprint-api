namespace IronPrint.Application.Common;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email);
}
