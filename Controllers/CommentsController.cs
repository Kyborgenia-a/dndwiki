using DndForum.Data;
using DndForum.Dtos;
using DndForum.Hypermedia;
using DndForum.Models;
using DndForum.Queries;
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
    [EndpointDescription("Grąžina įrašo komentarų puslapį chronologine tvarka. Filtras search ieško komentaro tekste. Puslapiavimas: page, pageSize.")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<CommentResponse>>> GetAll(
        int categoryId, int postId, [FromQuery] CommentQuery query)
    {
        if (!await PostExistsAsync(categoryId, postId))
        {
            return NotFound();
        }

        var comments = _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            comments = comments.Where(comment => comment.Content.Contains(search));
        }

        var totalCount = await comments.CountAsync();
        var pageItems = await comments
            .OrderBy(comment => comment.CreatedAt)
            .ThenBy(comment => comment.Id)
            .Skip(query.ItemsToSkip())
            .Take(query.PageSize)
            .ToListAsync();

        var responses = pageItems.Select(comment => comment.ToResponse(categoryId, Url)).ToList();
        return Ok(ForumLinks.ToPage(Url, nameof(GetAll), new { categoryId, postId }, query, totalCount, responses));
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

        return Ok(comment.ToResponse(categoryId, Url));
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
            comment.ToResponse(categoryId, Url));
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

        return Ok(comment.ToResponse(categoryId, Url));
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
