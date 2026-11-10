using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DndForum.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    // HS256 needs a key of at least 256 bits.
    private const int MinimumSigningKeyLength = 32;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    [MinLength(MinimumSigningKeyLength, ErrorMessage = "Jwt:SigningKey must be at least 32 characters. Set it with User Secrets.")]
    public string SigningKey { get; set; } = string.Empty;

    [Range(1, 60)]
    public int AccessTokenMinutes { get; set; } = 15;

    [Range(1, 90)]
    public int RefreshTokenDays { get; set; } = 7;

    public SymmetricSecurityKey CreateSigningKey() => new(Encoding.UTF8.GetBytes(SigningKey));
}
