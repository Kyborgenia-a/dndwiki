using DndForum.Data;
using DndForum.Dtos;
using DndForum.Hypermedia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private const int TopListSize = 5;

    private readonly ForumDbContext _dbContext;

    public DashboardController(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [EndpointSummary("Gauti forumo panelę")]
    [EndpointDescription("Forumo apžvalga, sudaryta iš kategorijų, įrašų ir komentarų: bendri skaičiai, kiekvienos kategorijos aktyvumas, labiausiai komentuojami įrašai ir naujausi komentarai.")]
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        var totals = new DashboardTotals(
            await _dbContext.Categories.CountAsync(),
            await _dbContext.Posts.CountAsync(),
            await _dbContext.Comments.CountAsync());

        return Ok(new DashboardResponse(
            totals,
            await GetCategoryActivityAsync(),
            await GetMostDiscussedPostsAsync(),
            await GetLatestCommentsAsync(),
            ForumLinks.ForDashboard(Url)));
    }

    private async Task<IReadOnlyList<CategoryActivity>> GetCategoryActivityAsync()
    {
        var postStats = await _dbContext.Posts
            .GroupBy(post => post.CategoryId)
            .Select(group => new
            {
                CategoryId = group.Key,
                PostCount = group.Count(),
                LatestPostAt = group.Max(post => post.CreatedAt)
            })
            .ToDictionaryAsync(stat => stat.CategoryId);

        var commentCounts = await _dbContext.Comments
            .GroupBy(comment => comment.Post.CategoryId)
            .Select(group => new { CategoryId = group.Key, CommentCount = group.Count() })
            .ToDictionaryAsync(stat => stat.CategoryId, stat => stat.CommentCount);

        var categories = await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .Select(category => new { category.Id, category.Name })
            .ToListAsync();

        return categories
            .Select(category =>
            {
                postStats.TryGetValue(category.Id, out var postStat);
                return new CategoryActivity(
                    category.Id,
                    category.Name,
                    postStat?.PostCount ?? 0,
                    commentCounts.GetValueOrDefault(category.Id),
                    postStat?.LatestPostAt,
                    [ForumLinks.CategorySelf(Url, category.Id)]);
            })
            .ToList();
    }

    private async Task<IReadOnlyList<DiscussedPost>> GetMostDiscussedPostsAsync()
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.Comments.Count)
            .ThenByDescending(post => post.CreatedAt)
            .Take(TopListSize)
            .Select(post => new
            {
                post.Id,
                post.Title,
                post.CategoryId,
                CategoryName = post.Category.Name,
                CommentCount = post.Comments.Count
            })
            .ToListAsync();

        return posts
            .Select(post => new DiscussedPost(
                post.Id,
                post.Title,
                post.CategoryId,
                post.CategoryName,
                post.CommentCount,
                [ForumLinks.PostSelf(Url, post.CategoryId, post.Id)]))
            .ToList();
    }

    private async Task<IReadOnlyList<RecentComment>> GetLatestCommentsAsync()
    {
        var comments = await _dbContext.Comments
            .AsNoTracking()
            .OrderByDescending(comment => comment.CreatedAt)
            .Take(TopListSize)
            .Select(comment => new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.PostId,
                PostTitle = comment.Post.Title,
                comment.Post.CategoryId,
                CategoryName = comment.Post.Category.Name
            })
            .ToListAsync();

        return comments
            .Select(comment => new RecentComment(
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.PostId,
                comment.PostTitle,
                comment.CategoryId,
                comment.CategoryName,
                [ForumLinks.CommentSelf(Url, comment.CategoryId, comment.PostId, comment.Id)]))
            .ToList();
    }
}
