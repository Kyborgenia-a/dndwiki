using DndForum.Data;
using DndForum.Dtos;
using DndForum.Models;
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

    [EndpointSummary("Gauti visas kategorijas")]
    [EndpointDescription("Grąžina visų forumo kategorijų sąrašą, surikiuotą pagal ID.")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .ToListAsync();

        return Ok(categories.Select(category => category.ToResponse()));
    }

    [EndpointSummary("Gauti kategoriją")]
    [EndpointDescription("Grąžina vieną kategoriją pagal ID.")]
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

        return Ok(category.ToResponse());
    }

    [EndpointSummary("Sukurti kategoriją")]
    [EndpointDescription("Sukuria naują kategoriją. Pavadinimas turi būti unikalus, kitaip grąžinamas 409.")]
    [HttpPost]
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

        var category = new Category { Name = request.Name, Description = request.Description };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { categoryId = category.Id }, category.ToResponse());
    }

    [EndpointSummary("Atnaujinti kategoriją")]
    [EndpointDescription("Pakeičia kategorijos pavadinimą ir aprašymą.")]
    [HttpPut("{categoryId:int}")]
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

        return Ok(category.ToResponse());
    }

    [EndpointSummary("Ištrinti kategoriją")]
    [EndpointDescription("Ištrina kategoriją kartu su visais jos įrašais ir komentarais.")]
    [HttpDelete("{categoryId:int}")]
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