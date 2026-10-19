namespace DndForum.Models;

public class Comment
{
    public const int ContentMaxLength = 2000;

    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}
