using DndForum.Auth;
using DndForum.Data;
using DndForum.Dtos;
using DndForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private const string BearerTokenType = "Bearer";

    private readonly ForumDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly SessionTokenService _sessions;

    public AuthController(ForumDbContext dbContext, IPasswordHasher<User> passwordHasher, SessionTokenService sessions)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _sessions = sessions;
    }

    [EndpointSummary("Registruotis")]
    [EndpointDescription("Sukuria naują naudotoją. Naujam naudotojui visada suteikiama Member rolė, kitos rolės laukas ignoruojamas.")]
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var isTaken = await _dbContext.Users.AnyAsync(user =>
            user.UserName == request.UserName || user.Email == request.Email);
        if (isTaken)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "User already exists.",
                detail: "The user name or email is already registered.");
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            Role = Roles.Member,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(UsersController.GetById), "Users", new { userId = user.Id }, user.ToResponse());
    }

    [EndpointSummary("Prisijungti")]
    [EndpointDescription("Patikrina vardą ir slaptažodį. Grąžina access token atsakyme, o refresh token nustato HttpOnly cookie.")]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user is null || !PasswordMatches(user, request.Password))
        {
            // The same answer for an unknown user and a wrong password, so user names cannot be guessed.
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Invalid user name or password.");
        }

        var tokens = await _sessions.StartSessionAsync(user);
        return Ok(SignIn(user, tokens));
    }

    [EndpointSummary("Atnaujinti tokenus")]
    [EndpointDescription("Pagal refresh token cookie išduoda naują access token ir pakeičia refresh token nauju (rotacija). Pakartotinai panaudotas senas refresh token uždaro visą sesiją.")]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        var refreshToken = RefreshTokenCookie.Read(Request);
        var refreshed = refreshToken is null ? null : await _sessions.RefreshAsync(refreshToken);

        if (refreshed is null)
        {
            RefreshTokenCookie.Delete(Response);
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Refresh token is missing, invalid or expired.");
        }

        return Ok(SignIn(refreshed.User, refreshed.Tokens));
    }

    [EndpointSummary("Atsijungti")]
    [EndpointDescription("Panaikina dabartinę sesiją: nebeveikia nei refresh token, nei šios sesijos access token.")]
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _sessions.RevokeSessionAsync(User.GetSessionId());
        RefreshTokenCookie.Delete(Response);
        return NoContent();
    }

    [EndpointSummary("Gauti prisijungusio naudotojo duomenis")]
    [EndpointDescription("Grąžina naudotoją, kurio ID nurodytas access token sub lauke.")]
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var user = await _dbContext.Users.FindAsync(User.GetUserId());
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToResponse());
    }

    private bool PasswordMatches(User user, string password) =>
        _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;

    private AuthResponse SignIn(User user, AuthTokens tokens)
    {
        RefreshTokenCookie.Append(Response, tokens.RefreshToken, tokens.RefreshTokenExpiresAt);
        return new AuthResponse(tokens.AccessToken.Value, BearerTokenType, tokens.AccessToken.ExpiresAt, user.ToResponse());
    }
}
