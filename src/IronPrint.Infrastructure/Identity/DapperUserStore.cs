using Dapper;
using IronPrint.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace IronPrint.Infrastructure.Identity;

public sealed class DapperUserStore :
    IUserStore<IdentityUser>,
    IUserEmailStore<IdentityUser>,
    IUserPasswordStore<IdentityUser>
{
    private readonly IDbConnectionFactory _db;

    public DapperUserStore(IDbConnectionFactory db) => _db = db;

    // ── IUserStore ──────────────────────────────────────────────────────────

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            """
            INSERT INTO users (id, username, normalized_username, email, normalized_email,
                email_confirmed, password_hash, security_stamp, concurrency_stamp)
            VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail,
                @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp)
            """, user);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync(
            """
            UPDATE users SET
                username = @UserName, normalized_username = @NormalizedUserName,
                email = @Email, normalized_email = @NormalizedEmail,
                email_confirmed = @EmailConfirmed, password_hash = @PasswordHash,
                security_stamp = @SecurityStamp, concurrency_stamp = @ConcurrencyStamp,
                lockout_end = @LockoutEnd, lockout_enabled = @LockoutEnabled,
                access_failed_count = @AccessFailedCount
            WHERE id = @Id
            """, user);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        await conn.ExecuteAsync("DELETE FROM users WHERE id = @Id", new { user.Id });
        return IdentityResult.Success;
    }

    public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<IdentityUser>(
            "SELECT * FROM users WHERE id = @userId", new { userId });
    }

    public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<IdentityUser>(
            "SELECT * FROM users WHERE normalized_username = @normalizedUserName", new { normalizedUserName });
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.Id);

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.UserName);

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken ct)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.NormalizedUserName);

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken ct)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    // ── IUserEmailStore ──────────────────────────────────────────────────────

    public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken ct)
    {
        user.Email = email;
        return Task.CompletedTask;
    }

    public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.Email);

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.EmailConfirmed);

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken ct)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
    {
        using var conn = await _db.CreateAsync(ct);
        return await conn.QuerySingleOrDefaultAsync<IdentityUser>(
            "SELECT * FROM users WHERE normalized_email = @normalizedEmail", new { normalizedEmail });
    }

    public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.NormalizedEmail);

    public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken ct)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    // ── IUserPasswordStore ───────────────────────────────────────────────────

    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken ct)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.PasswordHash);

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken ct) =>
        Task.FromResult(user.PasswordHash is not null);

    public void Dispose() { }
}
