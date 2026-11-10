using DndForum.Auth;
using DndForum.Data;
using DndForum.Dtos;
using DndForum.Hypermedia;
using DndForum.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/users")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class UsersController : ControllerBase
{
    private readonly ForumDbContext _dbContext;
    private readonly SessionTokenService _sessions;

    public UsersController(ForumDbContext dbContext, SessionTokenService sessions)
    {
        _dbContext = dbContext;
        _sessions = sessions;
    }

    [EndpointSummary("Gauti naudotojų sąrašą")]
    [EndpointDescription("Tik administratoriui. Grąžina naudotojų puslapį.")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetAll([FromQuery] PaginationQuery query)
    {
        var totalCount = await _dbContext.Users.CountAsync();
        var pageItems = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .Skip(query.ItemsToSkip())
            .Take(query.PageSize)
            .ToListAsync();

        var responses = pageItems.Select(user => user.ToResponse()).ToList();
        return Ok(ForumLinks.ToPage(Url, nameof(GetAll), new { }, query, totalCount, responses));
    }

    [EndpointSummary("Gauti naudotoją")]
    [EndpointDescription("Tik administratoriui. Grąžina vieną naudotoją pagal ID.")]
    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToResponse());
    }

    [EndpointSummary("Pakeisti naudotojo rolę")]
    [EndpointDescription("Tik administratoriui. Pakeičia rolę ir uždaro visas naudotojo sesijas, kad nauja rolė galiotų iškart. Savo rolės keisti negalima.")]
    [HttpPut("{userId:int}/role")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<UserResponse>> ChangeRole(int userId, ChangeRoleRequest request)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (User.HasUserId(userId))
        {
            return OwnAccountForbidden();
        }

        user.Role = request.Role;
        await _dbContext.SaveChangesAsync();
        await _sessions.RevokeAllSessionsAsync(userId);

        return Ok(user.ToResponse());
    }

    [EndpointSummary("Ištrinti naudotoją")]
    [EndpointDescription("Tik administratoriui. Ištrina naudotoją kartu su jo įrašais, komentarais ir sesijomis. Savęs ištrinti negalima.")]
    [HttpDelete("{userId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        if (User.HasUserId(userId))
        {
            return OwnAccountForbidden();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // Prevents an administrator from locking the system out of its last admin by accident.
    private ObjectResult OwnAccountForbidden() =>
        Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Administrators cannot change their own role or delete their own account.");
}
