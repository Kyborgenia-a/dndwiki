namespace DndForum.Dtos;

public record DashboardResponse(
    DashboardTotals Totals,
    IReadOnlyList<CategoryActivity> Categories,
    IReadOnlyList<DiscussedPost> MostDiscussedPosts,
    IReadOnlyList<RecentComment> LatestComments,
    IReadOnlyList<LinkDto> Links);

public record DashboardTotals(int Categories, int Posts, int Comments);

public record CategoryActivity(
    int Id,
    string Name,
    int PostCount,
    int CommentCount,
    DateTime? LatestPostAt,
    IReadOnlyList<LinkDto> Links);

public record DiscussedPost(
    int Id,
    string Title,
    int CategoryId,
    string CategoryName,
    string AuthorName,
    int CommentCount,
    IReadOnlyList<LinkDto> Links);

public record RecentComment(
    int Id,
    string Content,
    DateTime CreatedAt,
    string AuthorName,
    int PostId,
    string PostTitle,
    int CategoryId,
    string CategoryName,
    IReadOnlyList<LinkDto> Links);
