using System.ComponentModel.DataAnnotations;
using DndForum.Models;

namespace DndForum.Queries;

public class CommentQuery : PaginationQuery
{
    [StringLength(Comment.ContentMaxLength)]
    public string? Search { get; set; }

    public override IEnumerable<KeyValuePair<string, string>> FilterValues()
    {
        if (!string.IsNullOrWhiteSpace(Search))
        {
            yield return KeyValuePair.Create("search", Search);
        }
    }
}
