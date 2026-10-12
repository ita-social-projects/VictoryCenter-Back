using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.EventsPage;
using VictoryCenter.BLL.Queries.Public.EventsPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class EventsPageController : BaseApiController
{
    [HttpGet("public")]
    [ProducesResponseType(typeof(PublicEventsPageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPublic()
        => HandleResult(await Mediator.Send(new GetPublicEventsPageQuery()));
}
