using System.ComponentModel.DataAnnotations;
using DndForum.Auth;
using DndForum.Models;

namespace DndForum.Dtos;

public record UserResponse(int Id, string UserName, string Email, string Role, DateTime CreatedAt);

public class ChangeRoleRequest
{
    [Required]
    [AllowedValues(Roles.Admin, Roles.Moderator, Roles.Member)]
    public string Role { get; set; } = string.Empty;
}

public static class UserMappings
{
    public static UserResponse ToResponse(this User user) =>
        new(user.Id, user.UserName, user.Email, user.Role, user.CreatedAt);
}
