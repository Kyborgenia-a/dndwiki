using System.Globalization;
using DndForum.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DndForum.Auth;

public record AccessToken(string Value, DateTime ExpiresAt);

public class AccessTokenFactory
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;
    private readonly JsonWebTokenHandler _tokenHandler = new();

    public AccessTokenFactory(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _signingCredentials = new SigningCredentials(_options.CreateSigningKey(), SecurityAlgorithms.HmacSha256);
    }

    public AccessToken Create(User user, Guid sessionId)
    {
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_options.AccessTokenMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = _signingCredentials,
            Claims = new Dictionary<string, object>
            {
                [ForumClaims.Subject] = user.Id.ToString(CultureInfo.InvariantCulture),
                [ForumClaims.Name] = user.UserName,
                [ForumClaims.Role] = user.Role,
                [ForumClaims.SessionId] = sessionId.ToString(),
                [ForumClaims.TokenId] = Guid.NewGuid().ToString()
            }
        };

        return new AccessToken(_tokenHandler.CreateToken(descriptor), expiresAt);
    }
}
