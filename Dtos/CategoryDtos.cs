using System.ComponentModel.DataAnnotations;
using DndForum.Models;

namespace DndForum.Dtos;

public class CategoryRequest
{
    private const int NameMinLength = 2;

    [Required]
    [StringLength(Category.NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; init; } = string.Empty;

    [StringLength(Category.DescriptionMaxLength)]
    public string Description { get; init; } = string.Empty;
}

public record CategoryResponse(int Id, string Name, string Description);

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category category) =>
        new(category.Id, category.Name, category.Description);
}
