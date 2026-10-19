using DndForum.Data;
using DndForum.Dtos;
using DndForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/categories/{categoryId:int}/posts/{postId:int}/comments")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly ForumDbContext _dbContext;

    public CommentsController(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [EndpointSummary("Gauti įrašo komentarus")]
    [EndpointDescription("Grąžina visus įrašo komentarus chronologine tvarka.")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetAll(int categoryId, int postId)
    {
        if (!await PostExistsAsync(categoryId, postId))
        {
            return NotFound();
        }

        var comments = await _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderBy(comment => comment.CreatedAt)
            .ToListAsync();

        return Ok(comments.Select(comment => comment.ToResponse()));
    }

    [EndpointSummary("Gauti komentarą")]
    [EndpointDescription("Grąžina vieną komentarą. Komentaras turi priklausyti nurodytam įrašui ir kategorijai, kitaip grąžinamas 404.")]
    [HttpGet("{commentId:int}")]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentResponse>> GetById(int categoryId, int postId, int commentId)
    {
        var comment = await FindCommentAsync(categoryId, postId, commentId);
        if (comment is null)
        {
            return NotFound();
        }

        return Ok(comment.ToResponse());
    }

    [EndpointSummary("Sukurti komentarą")]
    [EndpointDescription("Prideda naują komentarą prie įrašo. Sukūrimo laiką nustato serveris.")]
    [HttpPost]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CommentResponse>> Create(int categoryId, int postId, CommentRequest request)
    {
        if (!await PostExistsAsync(categoryId, postId))
        {
            return NotFound();
        }

        var comment = new Comment
        {
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            PostId = postId
        };
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { categoryId, postId, commentId = comment.Id },
            comment.ToResponse());
    }

    [EndpointSummary("Atnaujinti komentarą")]
    [EndpointDescription("Pakeičia komentaro turinį.")]
    [HttpPut("{commentId:int}")]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CommentResponse>> Update(
        int categoryId, int postId, int commentId, CommentRequest request)
    {
        var comment = await FindCommentAsync(categoryId, postId, commentId);
        if (comment is null)
        {
            return NotFound();
        }

        comment.Content = request.Content;
        await _dbContext.SaveChangesAsync();

        return Ok(comment.ToResponse());
    }

    [EndpointSummary("Ištrinti komentarą")]
    [EndpointDescription("Ištrina komentarą.")]
    [HttpDelete("{commentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int categoryId, int postId, int commentId)
    {
        var comment = await FindCommentAsync(categoryId, postId, commentId);
        if (comment is null)
        {
            return NotFound();
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private Task<bool> PostExistsAsync(int categoryId, int postId) =>
        _dbContext.Posts.AnyAsync(post => post.Id == postId && post.CategoryId == categoryId);

    private Task<Comment?> FindCommentAsync(int categoryId, int postId, int commentId) =>
        _dbContext.Comments.FirstOrDefaultAsync(comment =>
            comment.Id == commentId &&
            comment.PostId == postId &&
            comment.Post.CategoryId == categoryId);
}