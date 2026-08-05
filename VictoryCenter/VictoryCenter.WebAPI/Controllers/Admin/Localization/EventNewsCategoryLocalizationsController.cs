using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;
using VictoryCenter.BLL.Queries.Admin.Localization.EventNewsCategories.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class EventNewsCategoryLocalizationsController : AuthorizedApiController
{
    [HttpGet("entityId/{entityId:long}")]
    [ProducesResponseType(typeof(List<AdminEventNewsCategoryLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEntityId(long entityId)
    {
        return HandleResult(await Mediator.Send(
            new GetEventNewsCategoryLocalizationsByEntityIdQuery(entityId)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdminEventNewsCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateEventNewsCategoryLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new CreateEventNewsCategoryLocalizationCommand(localization)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(AdminEventNewsCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long entityId,
        long languageId,
        [FromBody] UpdateEventNewsCategoryLocalizationDto localization)
    {
        return HandleResult(await Mediator.Send(
            new UpdateEventNewsCategoryLocalizationCommand(entityId, languageId, localization)));
    }

    [HttpDelete("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(DeleteEventNewsCategoryLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long entityId, long languageId)
    {
        return HandleResult(await Mediator.Send(
            new DeleteEventNewsCategoryLocalizationCommand(entityId, languageId)));
    }
}
