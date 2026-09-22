using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventsPage.Update;
using VictoryCenter.BLL.DTOs.Admin.EventsPage;
using VictoryCenter.BLL.Queries.Admin.EventsPage.Get;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

[Route("api/EventsPage")]
public class EventsPageController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(EventsIntroSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventsIntroSection()
        => HandleResult(await Mediator.Send(new GetEventsIntroSectionQuery()));

    [HttpPut("description")]
    [ProducesResponseType(typeof(EventsIntroSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDescription([FromBody] UpdateEventsPageDescriptionDto dto)
        => HandleResult(await Mediator.Send(new UpdateEventsPageDescriptionCommand(dto)));

    [HttpPut("events-block-title")]
    [ProducesResponseType(typeof(EventsIntroSectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEventsBlockTitle([FromBody] UpdateEventsBlockTitleDto dto)
        => HandleResult(await Mediator.Send(new UpdateEventsBlockTitleCommand(dto)));
}
