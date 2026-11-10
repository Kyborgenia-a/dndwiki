using DndForum.Data;
using DndForum.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DndForum.Auth;

public record AuthTokens(AccessToken AccessToken, string RefreshToken, DateTime RefreshTokenExpiresAt);

public record RefreshedSession(User User, AuthTokens Tokens);

/// <summary>
/// Manages login sessions: issues tokens, rotates the refresh token on every refresh and revokes sessions.
/// </summary>
public class SessionTokenService
{
    private readonly ForumDbContext _dbContext;
    private readonly AccessTokenFactory _accessTokenFactory;
    private readonly JwtOptions _options;

    public SessionTokenService(ForumDbContext dbContext, AccessTokenFactory accessTokenFactory, IOptions<JwtOptions> options)
    {
        _dbContext = dbContext;
        _accessTokenFactory = accessTokenFactory;
        _options = options.Value;
    }

    public async Task<AuthTokens> StartSessionAsync(User user)
    {
        var now = DateTime.UtcNow;
        var secret = RefreshTokenFormat.NewSecret();
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = RefreshTokenFormat.Hash(secret),
            CreatedAt = now,
            ExpiresAt = now.AddDays(_options.RefreshTokenDays)
        };

        _dbContext.UserSessions.Add(session);
        await _dbContext.SaveChangesAsync();

        return IssueTokens(user, session, secret);
    }

    /// <returns>New tokens, or null when the refresh token is invalid, expired, revoked or reused.</returns>
    public async Task<RefreshedSession?> RefreshAsync(string refreshToken)
    {
        if (!RefreshTokenFormat.TryParse(refreshToken, out var sessionId, out var secret))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var session = await _dbContext.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null || session.RevokedAt is not null || session.ExpiresAt <= now)
        {
            return null;
        }

        if (!RefreshTokenFormat.Matches(secret, session.RefreshTokenHash))
        {
            // A refresh token that was already rotated is being replayed, so it was most likely stolen.
            // Closing the whole session logs out both the attacker and the real user.
            session.RevokedAt = now;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        var newSecret = RefreshTokenFormat.NewSecret();
        session.RefreshTokenHash = RefreshTokenFormat.Hash(newSecret);
        session.ExpiresAt = now.AddDays(_options.RefreshTokenDays);
        await _dbContext.SaveChangesAsync();

        return new RefreshedSession(session.User, IssueTokens(session.User, session, newSecret));
    }

    public async Task RevokeSessionAsync(Guid sessionId)
    {
        var session = await _dbContext.UserSessions.FindAsync(sessionId);
        if (session is null || session.RevokedAt is not null)
        {
            return;
        }

        session.RevokedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RevokeAllSessionsAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var activeSessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync();

        activeSessions.ForEach(session => session.RevokedAt = now);
        await _dbContext.SaveChangesAsync();
    }

    public Task<bool> IsSessionActiveAsync(Guid sessionId)
    {
        var now = DateTime.UtcNow;
        return _dbContext.UserSessions.AnyAsync(s => s.Id == sessionId && s.RevokedAt == null && s.ExpiresAt > now);
    }

    private AuthTokens IssueTokens(User user, UserSession session, string refreshSecret) =>
        new(
            _accessTokenFactory.Create(user, session.Id),
            RefreshTokenFormat.Compose(session.Id, refreshSecret),
            session.ExpiresAt);
}
