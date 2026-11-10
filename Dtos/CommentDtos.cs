using System.ComponentModel.DataAnnotations;
using DndForum.Hypermedia;
using DndForum.Models;
using Microsoft.AspNetCore.Mvc;

namespace DndForum.Dtos;

public class CommentRequest
{
    [Required]
    [StringLength(Comment.ContentMaxLength)]
    public string Content { get; set; } = string.Empty;
}

public record CommentResponse(
    int Id,
    string Content,
    DateTime CreatedAt,
    int PostId,
    int AuthorId,
    string AuthorName,
    IReadOnlyList<LinkDto> Links);

public static class CommentMappings
{
    public static CommentResponse ToResponse(this Comment comment, int categoryId, IUrlHelper url) =>
        new(comment.Id, comment.Content, comment.CreatedAt, comment.PostId,
            comment.AuthorId, comment.Author.UserName,
            ForumLinks.ForComment(url, categoryId, comment.PostId, comment.Id));
}
