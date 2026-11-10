using DndForum.Models;
using Microsoft.AspNetCore.Authorization;

namespace DndForum.Auth;

public static class Policies
{
    public const string AuthorOrModerator = nameof(AuthorOrModerator);
}

public class AuthorOrModeratorRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Resource-based check: a post or comment can be changed by its author, a moderator or an admin.
/// </summary>
public class AuthorOrModeratorHandler : AuthorizationHandler<AuthorOrModeratorRequirement, IAuthoredContent>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorOrModeratorRequirement requirement,
        IAuthoredContent content)
    {
        var user = context.User;
        var canModerate = user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Moderator);

        if (canModerate || user.HasUserId(content.AuthorId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
