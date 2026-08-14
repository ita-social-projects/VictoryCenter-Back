using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.EventNews.Delete;
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

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEventNews(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteEventNewsCommand(id)));
    }
}
