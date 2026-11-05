using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Admin.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.EventNews.Delete;
using VictoryCenter.BLL.Commands.Admin.EventNews.Update;
using VictoryCenter.BLL.Commands.Admin.EventNews.Reorder;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetByFilters;
using VictoryCenter.BLL.Queries.Admin.EventNews.GetById;
using VictoryCenter.BLL.Queries.Admin.EventNews.Search;
using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.WebAPI.Filters;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class EventNewsController : AuthorizedApiController
{
    private readonly IContentLockUpdateCompletionService _contentLockUpdateCompletionService;

    public EventNewsController(IContentLockUpdateCompletionService contentLockUpdateCompletionService)
    {
        _contentLockUpdateCompletionService = contentLockUpdateCompletionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginationResult<EventNewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByFilters([FromQuery] EventNewsFilterDto filter)
    {
        return HandleResult(await Mediator.Send(new GetEventNewsByFiltersQuery(filter)));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginationResult<EventNewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchEventNews([FromQuery] SearchEventNewsDto searchEventNewsDto)
    {
        return HandleResult(await Mediator.Send(new SearchEventNewsQuery(searchEventNewsDto)));
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

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEventNews(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteEventNewsCommand(id)));
    }

    [HttpPut("{id:long}")]
    [RequireContentLock(ContentLockConstants.EventNewsResourceType, "id")]
    [ProducesResponseType(typeof(EventNewsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status423Locked)]
    public async Task<IActionResult> UpdateEventNews(long id, [FromBody] UpdateEventNewsDto updateEventNewsDto)
    {
        var result = await Mediator.Send(new UpdateEventNewsCommand(id, updateEventNewsDto));
        if (result.IsSuccess)
        {
            await _contentLockUpdateCompletionService.CompleteAsync(
                new()
                {
                    ResourceType = ContentLockConstants.EventNewsResourceType,
                    ResourceId = id,
                },
                HttpContext.GetContentLockSessionOrDefault());
        }

        return HandleResult(result);
    }

    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderEventNews([FromBody] ReorderEventNewsDto reorderEventNewsDto)
    {
        return HandleResult(await Mediator.Send(new ReorderEventNewsCommand(reorderEventNewsDto)));
    }
}
