using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.EventNews;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublished;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class EventNewsController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PublishedEventNewsDto>))]
    public async Task<IActionResult> GetPublishedEventNews([FromQuery] int? take = null)
    {
        return HandleResult(await Mediator.Send(new GetPublishedEventNewsQuery(take)));
    }
}
