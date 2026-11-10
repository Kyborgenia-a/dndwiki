using DndForum.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DndForum.Auth;

public static class AuthServiceCollectionExtensions
{
    private static readonly TimeSpan AllowedClockSkew = TimeSpan.FromSeconds(30);

    public static IServiceCollection AddForumAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<AccessTokenFactory>();
        services.AddScoped<SessionTokenService>();
        services.AddScoped<SessionValidatingJwtBearerEvents>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) => ConfigureJwtBearer(bearer, jwtOptions.Value));

        services.AddSingleton<IAuthorizationHandler, AuthorOrModeratorHandler>();
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.AuthorOrModerator, policy => policy.AddRequirements(new AuthorOrModeratorRequirement()));

        return services;
    }

    private static void ConfigureJwtBearer(JwtBearerOptions bearer, JwtOptions jwt)
    {
        bearer.MapInboundClaims = false;
        bearer.EventsType = typeof(SessionValidatingJwtBearerEvents);
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwt.CreateSigningKey(),
            // Only HS256 is accepted, so a token with "alg": "none" or another algorithm is rejected.
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            ValidateLifetime = true,
            ClockSkew = AllowedClockSkew,
            NameClaimType = ForumClaims.Name,
            RoleClaimType = ForumClaims.Role
        };
    }
}
