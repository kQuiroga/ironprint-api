using Dapper;
using IronPrint.Domain.Ports;
using IronPrint.Infrastructure.Persistence;

namespace IronPrint.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _db;

    public RefreshTokenRepository(IDbConnectionFactory db) => _db = db;

    public async Task AddAsync(string userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "INSERT INTO refresh_tokens (user_id, token_hash, expires_at) VALUES (@userId, @tokenHash, @expiresAt)",
            new { userId, tokenHash, expiresAt });
    }

    public async Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<RefreshTokenRecord>(
            "SELECT id, user_id, expires_at, revoked_at FROM refresh_tokens WHERE token_hash = @tokenHash",
            new { tokenHash });
    }

    public async Task RevokeAsync(Guid id, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET revoked_at = now() WHERE id = @id",
            new { id });
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            "UPDATE refresh_tokens SET revoked_at = now() WHERE user_id = @userId AND revoked_at IS NULL",
            new { userId });
    }
}
