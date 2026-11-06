using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.WebAPI.Controllers.Common;
using VictoryCenter.WebAPI.Filters;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class EventNewsLocalizationsController : AuthorizedApiController
{
    private readonly IContentLockUpdateCompletionService _contentLockUpdateCompletionService;

    public EventNewsLocalizationsController(IContentLockUpdateCompletionService contentLockUpdateCompletionService)
    {
        _contentLockUpdateCompletionService = contentLockUpdateCompletionService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdminEventNewsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEventNewsLocalization([FromBody] CreateAdminEventNewsLocalizationDto createAdminEventNewsLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateEventNewsLocalizationCommand(createAdminEventNewsLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [RequireContentLock(ContentLockConstants.EventNewsResourceType, "entityId")]
    [ProducesResponseType(typeof(AdminEventNewsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status423Locked)]
    public async Task<IActionResult> UpdateEventNewsLocalization(
        [FromBody] UpdateEventNewsLocalizationDto updateEventNewsLocalizationDto,
        [FromRoute(Name = "entityId")] long entityId,
        [FromRoute(Name = "languageId")] long languageId)
    {
        var result = await Mediator.Send(new UpdateEventNewsLocalizationCommand(updateEventNewsLocalizationDto, entityId, languageId));
        if (result.IsSuccess)
        {
            await _contentLockUpdateCompletionService.CompleteAsync(
                new()
                {
                    ResourceType = ContentLockConstants.EventNewsResourceType,
                    ResourceId = entityId,
                });
        }

        return HandleResult(result);
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteEventNewsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEventNewsLocalization(
        [FromRoute(Name = "entityId")] long entityId,
        [FromRoute(Name = "languageId")] long languageId)
    {
        return HandleResult(await Mediator.Send(new DeleteEventNewsLocalizationCommand(entityId, languageId)));
    }
}
