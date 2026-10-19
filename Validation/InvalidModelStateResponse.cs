using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DndForum.Validation;

/// <summary>
/// Returns 400 when the request body cannot be read as JSON (syntax errors, wrong value types,
/// empty body) and 422 when the JSON is readable but its values break validation rules.
/// </summary>
public static class InvalidModelStateResponse
{
    public static IActionResult Create(ActionContext context)
    {
        var statusCode = IsMalformedRequestBody(context.ModelState)
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status422UnprocessableEntity;

        var problem = new ValidationProblemDetails(context.ModelState) { Status = statusCode };

        return new ObjectResult(problem) { StatusCode = statusCode };
    }

    // System.Text.Json reports read errors under JSON-path keys ("$", "$.title") and an empty
    // body under an empty key, while validation attributes report under property names.
    private static bool IsMalformedRequestBody(ModelStateDictionary modelState) =>
        modelState.Any(entry =>
            entry.Value?.Errors.Count > 0 &&
            (entry.Key.Length == 0 || entry.Key.StartsWith('$')));
}
