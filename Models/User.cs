namespace DndForum.Models;

public class User
{
    public const int UserNameMaxLength = 50;
    public const int EmailMaxLength = 254;
    public const int RoleMaxLength = 20;

    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<UserSession> Sessions { get; set; } = new();
}
