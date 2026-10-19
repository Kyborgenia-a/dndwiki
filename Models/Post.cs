namespace DndForum.Models;

public class Post
{
    public const int TitleMaxLength = 200;
    public const int ContentMaxLength = 10000;

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public List<Comment> Comments { get; set; } = new();
}
