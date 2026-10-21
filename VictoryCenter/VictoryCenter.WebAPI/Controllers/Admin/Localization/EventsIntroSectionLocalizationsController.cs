using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventsIntroSection.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;
using VictoryCenter.BLL.Queries.Admin.Localization.EventsIntroSection.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class EventsIntroSectionLocalizationsController : AuthorizedApiController
{
    [HttpGet("{entityId:long}")]
    [ProducesResponseType(typeof(List<EventsIntroSectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventsIntroSectionLocalizations(
        [FromRoute] long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetEventsIntroSectionLocalizationByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventsIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateEventsIntroSectionLocalization(
        [FromBody] CreateEventsIntroSectionLocalizationDto createDto)
    {
        return HandleResult(await Mediator.Send(
            new CreateEventsIntroSectionLocalizationCommand(createDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(EventsIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEventsIntroSectionLocalization(
        [FromBody] UpdateEventsIntroSectionLocalizationDto updateDto,
        [FromRoute(Name = "entityId")] long entityId,
        [FromRoute(Name = "languageId")] long languageId)
    {
        return HandleResult(await Mediator.Send(
            new UpdateEventsIntroSectionLocalizationCommand(updateDto, entityId, languageId)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteEventsIntroSectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEventsIntroSectionLocalization(
        [FromRoute(Name = "entityId")] long entityId,
        [FromRoute(Name = "languageId")] long languageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteEventsIntroSectionLocalizationCommand(entityId, languageId)));
    }
}
