using DndForum.Dtos;
using DndForum.Hypermedia;
using Microsoft.AspNetCore.Mvc;

namespace DndForum.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class ApiRootController : ControllerBase
{
    [EndpointSummary("API pradžios taškas")]
    [EndpointDescription("Grąžina nuorodas į pagrindinius API resursus, nuo kurių klientas gali naršyti visą API.")]
    [HttpGet]
    [ProducesResponseType(typeof(ApiRootResponse), StatusCodes.Status200OK)]
    public ActionResult<ApiRootResponse> Get() =>
        Ok(new ApiRootResponse("D&D Forum API", "v1", ForumLinks.ForApiRoot(Url)));
}
