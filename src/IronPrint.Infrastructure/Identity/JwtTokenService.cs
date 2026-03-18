using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IronPrint.Application.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IronPrint.Infrastructure.Identity;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(IConfiguration config)
    {
        _issuer = config["Jwt:Issuer"]!;
        _audience = config["Jwt:Audience"]!;
        _signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!));
    }

    public string GenerateAccessToken(string userId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
