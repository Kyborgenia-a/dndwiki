using DndForum.Data;
using DndForum.Dtos;
using DndForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/categories/{categoryId:int}/posts")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly ForumDbContext _dbContext;

    public PostsController(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [EndpointSummary("Gauti kategorijos įrašus")]
    [EndpointDescription("Grąžina visus nurodytos kategorijos įrašus, naujausius pirmiau.")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetAll(int categoryId)
    {
        if (!await CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(post => post.CategoryId == categoryId)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();

        return Ok(posts.Select(post => post.ToResponse()));
    }

    [EndpointSummary("Gauti įrašą")]
    [EndpointDescription("Grąžina vieną įrašą. Įrašas turi priklausyti nurodytai kategorijai, kitaip grąžinamas 404.")]
    [HttpGet("{postId:int}")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostResponse>> GetById(int categoryId, int postId)
    {
        var post = await FindPostAsync(categoryId, postId);
        if (post is null)
        {
            return NotFound();
        }

        return Ok(post.ToResponse());
    }

    [EndpointSummary("Sukurti įrašą")]
    [EndpointDescription("Sukuria naują įrašą nurodytoje kategorijoje. Sukūrimo laiką nustato serveris.")]
    [HttpPost]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PostResponse>> Create(int categoryId, PostRequest request)
    {
        if (!await CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            CategoryId = categoryId
        };
        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { categoryId, postId = post.Id }, post.ToResponse());
    }

    [EndpointSummary("Atnaujinti įrašą")]
    [EndpointDescription("Pakeičia įrašo pavadinimą ir turinį.")]
    [HttpPut("{postId:int}")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PostResponse>> Update(int categoryId, int postId, PostRequest request)
    {
        var post = await FindPostAsync(categoryId, postId);
        if (post is null)
        {
            return NotFound();
        }

        post.Title = request.Title;
        post.Content = request.Content;
        await _dbContext.SaveChangesAsync();

        return Ok(post.ToResponse());
    }

    [EndpointSummary("Ištrinti įrašą")]
    [EndpointDescription("Ištrina įrašą kartu su visais jo komentarais.")]
    [HttpDelete("{postId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int categoryId, int postId)
    {
        var post = await FindPostAsync(categoryId, postId);
        if (post is null)
        {
            return NotFound();
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private Task<bool> CategoryExistsAsync(int categoryId) =>
        _dbContext.Categories.AnyAsync(category => category.Id == categoryId);

    private Task<Post?> FindPostAsync(int categoryId, int postId) =>
        _dbContext.Posts.FirstOrDefaultAsync(post => post.Id == postId && post.CategoryId == categoryId);
}