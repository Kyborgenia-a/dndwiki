using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DndForum.Auth;

/// <summary>
/// A signed JWT stays valid until it expires, so after the signature check this also verifies
/// that the token's session is still active. Logout, role changes and user deletion take effect immediately.
/// </summary>
public class SessionValidatingJwtBearerEvents : JwtBearerEvents
{
    private readonly SessionTokenService _sessions;

    public SessionValidatingJwtBearerEvents(SessionTokenService sessions)
    {
        _sessions = sessions;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var sessionClaim = context.Principal?.FindFirst(ForumClaims.SessionId)?.Value;
        var isActive = Guid.TryParse(sessionClaim, out var sessionId) && await _sessions.IsSessionActiveAsync(sessionId);

        if (!isActive)
        {
            context.Fail("The session of this access token is no longer active.");
        }
    }
}
