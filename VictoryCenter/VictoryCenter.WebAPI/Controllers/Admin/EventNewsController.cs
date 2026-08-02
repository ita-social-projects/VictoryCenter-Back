using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetById;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class EventNewsController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<EventNewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFilters([FromQuery] EventNewsFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetEventNewsByFiltersQuery(filter)));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(EventNewsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        return HandleResult(await Mediator.Send(new GetEventNewsByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEventNews([FromBody] CreateEventNewsDto createEventNewsDto)
    {
        return HandleResult(await Mediator.Send(new CreateEventNewsCommand(createEventNewsDto)));
    }
}
