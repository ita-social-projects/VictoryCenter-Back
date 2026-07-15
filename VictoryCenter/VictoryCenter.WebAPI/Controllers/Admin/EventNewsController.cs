using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
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
}
