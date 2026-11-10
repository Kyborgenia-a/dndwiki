using System.Security.Cryptography;
using System.Text;

namespace DndForum.Auth;

/// <summary>
/// A refresh token looks like "{sessionId}.{secret}". The session ID tells which session to load,
/// the random secret proves ownership. Only the SHA-256 hash of the secret is stored in the database.
/// </summary>
public static class RefreshTokenFormat
{
    private const int SecretByteLength = 32;
    private const char Separator = '.';
    private const string SessionIdFormat = "N";

    public static string NewSecret() => Convert.ToHexString(RandomNumberGenerator.GetBytes(SecretByteLength));

    public static string Compose(Guid sessionId, string secret) =>
        $"{sessionId.ToString(SessionIdFormat)}{Separator}{secret}";

    public static bool TryParse(string refreshToken, out Guid sessionId, out string secret)
    {
        sessionId = Guid.Empty;
        secret = string.Empty;

        var parts = refreshToken.Split(Separator);
        if (parts.Length != 2 || !Guid.TryParseExact(parts[0], SessionIdFormat, out sessionId))
        {
            return false;
        }

        secret = parts[1];
        return secret.Length > 0;
    }

    public static string Hash(string secret) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));

    // Constant-time comparison so response timing does not reveal how much of the hash matched.
    public static bool Matches(string secret, string expectedHash) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(Hash(secret)),
            Encoding.ASCII.GetBytes(expectedHash));
}
