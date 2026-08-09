using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class EventNewsController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateEventNews([FromBody] CreateEventNewsDto createEventNewsDto)
    {
        return HandleResult(await Mediator.Send(new CreateEventNewsCommand(createEventNewsDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(EventNewsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEventNews(long id, [FromBody] UpdateEventNewsDto updateEventNewsDto)
    {
        return HandleResult(await Mediator.Send(new UpdateEventNewsCommand(id, updateEventNewsDto)));
    }
}
