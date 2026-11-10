namespace DndForum.Auth;

/// <summary>
/// The refresh token travels only in a cookie:
/// HttpOnly hides it from JavaScript (XSS), Secure keeps it on HTTPS,
/// SameSite=Strict stops other sites from sending it (CSRF), and the path limits it to the auth endpoints.
/// </summary>
public static class RefreshTokenCookie
{
    public const string Name = "refreshToken";
    private const string Path = "/api/auth";

    public static void Append(HttpResponse response, string refreshToken, DateTime expiresAt) =>
        response.Cookies.Append(Name, refreshToken, CreateOptions(new DateTimeOffset(expiresAt)));

    public static string? Read(HttpRequest request) => request.Cookies[Name];

    public static void Delete(HttpResponse response) => response.Cookies.Delete(Name, CreateOptions(expires: null));

    private static CookieOptions CreateOptions(DateTimeOffset? expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = Path,
        Expires = expires
    };
}
