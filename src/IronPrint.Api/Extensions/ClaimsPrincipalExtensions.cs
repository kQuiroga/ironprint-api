using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IronPrint.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("UserId no encontrado en el token.");
}
