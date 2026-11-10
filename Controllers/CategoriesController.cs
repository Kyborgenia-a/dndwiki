using DndForum.Auth;
using DndForum.Data;
using DndForum.Dtos;
using DndForum.Hypermedia;
using DndForum.Models;
using DndForum.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ForumDbContext _dbContext;

    public CategoriesController(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [EndpointSummary("Gauti kategorijų sąrašą")]
    [EndpointDescription("Grąžina kategorijų puslapį. Filtras search ieško pavadinime ir aprašyme. Puslapiavimas: page (nuo 1), pageSize (1–50).")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PagedResponse<CategoryResponse>>> GetAll([FromQuery] CategoryQuery query)
    {
        var categories = _dbContext.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            categories = categories.Where(category =>
                category.Name.Contains(search) || category.Description.Contains(search));
        }

        var totalCount = await categories.CountAsync();
        var pageItems = await categories
            .OrderBy(category => category.Id)
            .Skip(query.ItemsToSkip())
            .Take(query.PageSize)
            .ToListAsync();

        var responses = pageItems.Select(category => category.ToResponse(Url)).ToList();
        return Ok(ForumLinks.ToPage(Url, nameof(GetAll), new { }, query, totalCount, responses));
    }

    [EndpointSummary("Gauti kategoriją")]
    [EndpointDescription("Grąžina vieną kategoriją pagal ID kartu su nuorodomis į jos įrašus ir galimus veiksmus.")]
    [HttpGet("{categoryId:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetById(int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId);
        if (category is null)
        {
            return NotFound();
        }

        return Ok(category.ToResponse(Url));
    }

    [EndpointSummary("Sukurti kategoriją")]
    [EndpointDescription("Tik administratoriui. Sukuria naują kategoriją. Pavadinimas turi būti unikalus, kitaip grąžinamas 409.")]
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request)
    {
        if (await CategoryNameExistsAsync(request.Name))
        {
            return CategoryNameConflict(request.Name);
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { categoryId = category.Id }, category.ToResponse(Url));
    }

    [EndpointSummary("Atnaujinti kategoriją")]
    [EndpointDescription("Tik administratoriui. Pakeičia kategorijos pavadinimą ir aprašymą.")]
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{categoryId:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CategoryResponse>> Update(int categoryId, CategoryRequest request)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId);
        if (category is null)
        {
            return NotFound();
        }

        if (await CategoryNameExistsAsync(request.Name, exceptCategoryId: categoryId))
        {
            return CategoryNameConflict(request.Name);
        }

        category.Name = request.Name;
        category.Description = request.Description;
        await _dbContext.SaveChangesAsync();

        return Ok(category.ToResponse(Url));
    }

    [EndpointSummary("Ištrinti kategoriją")]
    [EndpointDescription("Tik administratoriui. Ištrina kategoriją kartu su visais jos įrašais ir komentarais.")]
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{categoryId:int}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId);
        if (category is null)
        {
            return NotFound();
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private Task<bool> CategoryNameExistsAsync(string name, int? exceptCategoryId = null) =>
        _dbContext.Categories.AnyAsync(category => category.Name == name && category.Id != exceptCategoryId);

    private ObjectResult CategoryNameConflict(string name) =>
        Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Category name already exists.",
            detail: $"A category named '{name}' already exists.");
}
