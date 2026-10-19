namespace DndForum.Models;

public class Category
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Post> Posts { get; set; } = new();
}
