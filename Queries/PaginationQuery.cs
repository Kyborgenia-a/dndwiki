using System.ComponentModel.DataAnnotations;

namespace DndForum.Queries;

public class PaginationQuery
{
    private const int MaxPage = 10_000;
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;

    [Range(1, MaxPage)]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; set; } = DefaultPageSize;

    public int ItemsToSkip() => (Page - 1) * PageSize;

    public int TotalPages(int totalCount) => (int)Math.Ceiling(totalCount / (double)PageSize);

    // Filter values are repeated in pagination links so that "next" keeps the same filter.
    public virtual IEnumerable<KeyValuePair<string, string>> FilterValues() => [];
}
