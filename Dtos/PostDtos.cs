using System.ComponentModel.DataAnnotations;
using DndForum.Models;

namespace DndForum.Dtos;

public class PostRequest
{
    private const int TitleMinLength = 3;
    private const int ContentMinLength = 10;

    [Required]
    [StringLength(Post.TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [StringLength(Post.ContentMaxLength, MinimumLength = ContentMinLength)]
    public string Content { get; init; } = string.Empty;
}

public record PostResponse(int Id, string Title, string Content, DateTime CreatedAt, int CategoryId);

public static class PostMappings
{
    public static PostResponse ToResponse(this Post post) =>
        new(post.Id, post.Title, post.Content, post.CreatedAt, post.CategoryId);
}
