namespace DndForum.Models;

/// <summary>
/// One login of a user. The access token carries the session ID, and the refresh token is stored here
/// only as a SHA-256 hash, so a leaked database does not leak usable refresh tokens.
/// </summary>
public class UserSession
{
    public const int RefreshTokenHashLength = 64;

    public Guid Id { get; set; }
    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
