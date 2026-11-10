using System.Globalization;
using System.Security.Claims;

namespace DndForum.Auth;

/// <summary>
/// Short JWT claim names. JwtBearer is configured with MapInboundClaims = false,
/// so these names reach ClaimsPrincipal unchanged.
/// </summary>
public static class ForumClaims
{
    public const string Subject = "sub";
    public const string Name = "name";
    public const string Role = "role";
    public const string SessionId = "sid";
    public const string TokenId = "jti";

    public static int GetUserId(this ClaimsPrincipal principal) =>
        int.Parse(RequiredValue(principal, Subject), CultureInfo.InvariantCulture);

    public static Guid GetSessionId(this ClaimsPrincipal principal) =>
        Guid.Parse(RequiredValue(principal, SessionId));

    public static bool HasUserId(this ClaimsPrincipal principal, int userId) =>
        int.TryParse(principal.FindFirstValue(Subject), CultureInfo.InvariantCulture, out var id) && id == userId;

    private static string RequiredValue(ClaimsPrincipal principal, string claimType) =>
        principal.FindFirstValue(claimType)
        ?? throw new InvalidOperationException($"The access token has no '{claimType}' claim.");
}
