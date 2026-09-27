using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Queries.Admin.EventsPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class EventsPageController : BaseApiController
{
    [HttpGet("public")]
    [ProducesResponseType(typeof(EventsIntroSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventsIntroSection()
    {
        return HandleResult(await Mediator.Send(new GetEventsIntroSectionQuery()));
    }
}
