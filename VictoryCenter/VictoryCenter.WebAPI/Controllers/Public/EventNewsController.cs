using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublishedBySlug;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class EventNewsController : BaseApiController
{
    [HttpGet("published")]
    public async Task<IActionResult> GetPublished(
    [FromQuery] long? categoryId, [FromQuery] int? offset, [FromQuery] int? limit)
    => HandleResult(await Mediator.Send(new GetPublishedEventNewsQuery(categoryId, offset, limit)));

    [HttpGet("published/{slug}")]
    public async Task<IActionResult> GetPublishedBySlug([FromRoute] string slug)
    => HandleResult(await Mediator.Send(new GetPublishedEventNewsBySlugQuery(slug)));
}
