using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DndForum.Models;

namespace DndForum.Queries;

public class PostQuery : PaginationQuery, IValidatableObject
{
    private const string SortableDateFormat = "s";

    [StringLength(Post.TitleMaxLength)]
    public string? Search { get; set; }

    public DateTime? CreatedFrom { get; set; }

    public DateTime? CreatedTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CreatedFrom > CreatedTo)
        {
            yield return new ValidationResult(
                "CreatedFrom negali būti vėlesnė data nei CreatedTo.",
                [nameof(CreatedFrom), nameof(CreatedTo)]);
        }
    }

    public override IEnumerable<KeyValuePair<string, string>> FilterValues()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            yield return KeyValuePair.Create("search", Search);
        }

        if (CreatedFrom is { } createdFrom)
        {
            yield return KeyValuePair.Create("createdFrom", createdFrom.ToString(SortableDateFormat, CultureInfo.InvariantCulture));
        }

        if (CreatedTo is { } createdTo)
        {
            yield return KeyValuePair.Create("createdTo", createdTo.ToString(SortableDateFormat, CultureInfo.InvariantCulture));
        }
    }
}
