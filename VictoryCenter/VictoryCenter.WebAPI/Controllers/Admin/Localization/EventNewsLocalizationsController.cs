using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class EventNewsLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(EventNewsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEventNewsLocalization([FromBody] CreateEventNewsLocalizationDto createEventNewsLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateEventNewsLocalizationCommand(createEventNewsLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(EventNewsLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEventNewsLocalization(
        [FromBody] UpdateEventNewsLocalizationDto updateEventNewsLocalizationDto,
        [FromRoute(Name = "entityId")] long entityId,
        [FromRoute(Name = "languageId")] long languageId)
    {
        return HandleResult(await Mediator.Send(new UpdateEventNewsLocalizationCommand(updateEventNewsLocalizationDto, entityId, languageId)));
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
