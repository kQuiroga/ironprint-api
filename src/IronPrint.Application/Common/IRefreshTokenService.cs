namespace IronPrint.Application.Common;

public interface IRefreshTokenService
{
    /// <summary>Genera un token crudo (random) — se envía al cliente.</summary>
    string GenerateToken();

    /// <summary>Hashea el token crudo con SHA-256 — se guarda en DB.</summary>
    string HashToken(string token);
}
