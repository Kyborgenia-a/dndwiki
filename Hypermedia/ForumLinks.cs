using DndForum.Controllers;
using DndForum.Dtos;
using DndForum.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DndForum.Hypermedia;

/// <summary>
/// Builds HATEOAS links from the controllers' routes, so links stay correct if a route template changes.
/// </summary>
public static class ForumLinks
{
    private const string Categories = "Categories";
    private const string Posts = "Posts";
    private const string Comments = "Comments";
    private const string Dashboard = "Dashboard";

    public static LinkDto CategorySelf(IUrlHelper url, int categoryId) =>
        Link(url, "self", HttpMethods.Get, Categories, nameof(CategoriesController.GetById), new { categoryId });

    public static LinkDto PostSelf(IUrlHelper url, int categoryId, int postId) =>
        Link(url, "self", HttpMethods.Get, Posts, nameof(PostsController.GetById), new { categoryId, postId });

    public static LinkDto CommentSelf(IUrlHelper url, int categoryId, int postId, int commentId) =>
        Link(url, "self", HttpMethods.Get, Comments, nameof(CommentsController.GetById), new { categoryId, postId, commentId });

    public static IReadOnlyList<LinkDto> ForCategory(IUrlHelper url, int categoryId)
    {
        var route = new { categoryId };
        return
        [
            CategorySelf(url, categoryId),
            Link(url, "update", HttpMethods.Put, Categories, nameof(CategoriesController.Update), route),
            Link(url, "delete", HttpMethods.Delete, Categories, nameof(CategoriesController.Delete), route),
            Link(url, "posts", HttpMethods.Get, Posts, nameof(PostsController.GetAll), route),
            Link(url, "create-post", HttpMethods.Post, Posts, nameof(PostsController.Create), route)
        ];
    }

    public static IReadOnlyList<LinkDto> ForPost(IUrlHelper url, int categoryId, int postId)
    {
        var route = new { categoryId, postId };
        return
        [
            PostSelf(url, categoryId, postId),
            Link(url, "update", HttpMethods.Put, Posts, nameof(PostsController.Update), route),
            Link(url, "delete", HttpMethods.Delete, Posts, nameof(PostsController.Delete), route),
            Link(url, "category", HttpMethods.Get, Categories, nameof(CategoriesController.GetById), new { categoryId }),
            Link(url, "comments", HttpMethods.Get, Comments, nameof(CommentsController.GetAll), route),
            Link(url, "create-comment", HttpMethods.Post, Comments, nameof(CommentsController.Create), route)
        ];
    }

    public static IReadOnlyList<LinkDto> ForComment(IUrlHelper url, int categoryId, int postId, int commentId)
    {
        var route = new { categoryId, postId, commentId };
        return
        [
            CommentSelf(url, categoryId, postId, commentId),
            Link(url, "update", HttpMethods.Put, Comments, nameof(CommentsController.Update), route),
            Link(url, "delete", HttpMethods.Delete, Comments, nameof(CommentsController.Delete), route),
            Link(url, "post", HttpMethods.Get, Posts, nameof(PostsController.GetById), new { categoryId, postId })
        ];
    }

    public static IReadOnlyList<LinkDto> ForApiRoot(IUrlHelper url) =>
    [
        Link(url, "categories", HttpMethods.Get, Categories, nameof(CategoriesController.GetAll), new { }),
        Link(url, "create-category", HttpMethods.Post, Categories, nameof(CategoriesController.Create), new { }),
        Link(url, "dashboard", HttpMethods.Get, Dashboard, nameof(DashboardController.Get), new { }),
        new LinkDto("openapi", "/openapi/v1.json", HttpMethods.Get)
    ];

    public static IReadOnlyList<LinkDto> ForDashboard(IUrlHelper url) =>
    [
        Link(url, "self", HttpMethods.Get, Dashboard, nameof(DashboardController.Get), new { }),
        Link(url, "categories", HttpMethods.Get, Categories, nameof(CategoriesController.GetAll), new { })
    ];

    public static PagedResponse<T> ToPage<T>(
        IUrlHelper url,
        string listAction,
        object routeValues,
        PaginationQuery query,
        int totalCount,
        IReadOnlyList<T> items)
    {
        var totalPages = query.TotalPages(totalCount);
        return new PagedResponse<T>(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages,
            ForPage(url, listAction, routeValues, query, totalPages));
    }

    private static IReadOnlyList<LinkDto> ForPage(
        IUrlHelper url,
        string listAction,
        object routeValues,
        PaginationQuery query,
        int totalPages)
    {
        var lastPage = Math.Max(totalPages, 1);
        var links = new List<LinkDto>
        {
            PageLink("self", query.Page),
            PageLink("first", 1),
            PageLink("last", lastPage)
        };

        if (query.Page > 1)
        {
            links.Add(PageLink("previous", Math.Min(query.Page - 1, lastPage)));
        }

        if (query.Page < lastPage)
        {
            links.Add(PageLink("next", query.Page + 1));
        }

        return links;

        LinkDto PageLink(string rel, int page)
        {
            var values = new RouteValueDictionary(routeValues);
            foreach (var (name, value) in query.FilterValues())
            {
                values[name] = value;
            }

            values["page"] = page;
            values["pageSize"] = query.PageSize;

            return new LinkDto(rel, url.Action(listAction, values) ?? string.Empty, HttpMethods.Get);
        }
    }

    private static LinkDto Link(IUrlHelper url, string rel, string method, string controller, string action, object values) =>
        new(rel, url.Action(action, controller, values) ?? string.Empty, method);
}
