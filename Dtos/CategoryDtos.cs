using System.ComponentModel.DataAnnotations;
using DndForum.Hypermedia;
using DndForum.Models;
using Microsoft.AspNetCore.Mvc;

namespace DndForum.Dtos;

public class CategoryRequest
{
    private const int NameMinLength = 2;

    [Required]
    [StringLength(Category.NameMaxLength, MinimumLength = NameMinLength)]
    public string Name { get; set; } = string.Empty;

    [StringLength(Category.DescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;
}

public record CategoryResponse(
    int Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    IReadOnlyList<LinkDto> Links);

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category category, IUrlHelper url) =>
        new(category.Id, category.Name, category.Description, category.CreatedAt,
            ForumLinks.ForCategory(url, category.Id));
}
