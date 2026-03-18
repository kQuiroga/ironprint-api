namespace IronPrint.Domain.Ports;

public interface IRefreshTokenRepository
{
    Task AddAsync(string userId, string tokenHash, DateTime expiresAt, CancellationToken ct = default);
    Task<RefreshTokenRecord?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task RevokeAsync(Guid id, CancellationToken ct = default);
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}

public record RefreshTokenRecord(Guid Id, string UserId, DateTime ExpiresAt, DateTime? RevokedAt)
{
    public bool IsValid => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
