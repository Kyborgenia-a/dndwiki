using System.ComponentModel.DataAnnotations;
using DndForum.Models;

namespace DndForum.Dtos;

public class CommentRequest
{
    [Required]
    [StringLength(Comment.ContentMaxLength)]
    public string Content { get; init; } = string.Empty;
}

public record CommentResponse(int Id, string Content, DateTime CreatedAt, int PostId);

public static class CommentMappings
{
    public static CommentResponse ToResponse(this Comment comment) =>
        new(comment.Id, comment.Content, comment.CreatedAt, comment.PostId);
}
