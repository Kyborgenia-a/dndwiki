using System.ComponentModel.DataAnnotations;
using DndForum.Hypermedia;
using DndForum.Models;
using Microsoft.AspNetCore.Mvc;

namespace DndForum.Dtos;

public class PostRequest
{
    private const int TitleMinLength = 3;
    private const int ContentMinLength = 10;

    [Required]
    [StringLength(Post.TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(Post.ContentMaxLength, MinimumLength = ContentMinLength)]
    public string Content { get; set; } = string.Empty;
}

public record PostResponse(
    int Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    int CategoryId,
    IReadOnlyList<LinkDto> Links);

public static class PostMappings
{
    public static PostResponse ToResponse(this Post post, IUrlHelper url) =>
        new(post.Id, post.Title, post.Content, post.CreatedAt, post.CategoryId,
            ForumLinks.ForPost(url, post.CategoryId, post.Id));
}
