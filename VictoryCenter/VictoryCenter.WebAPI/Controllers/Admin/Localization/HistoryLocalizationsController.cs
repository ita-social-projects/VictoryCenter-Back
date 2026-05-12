using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HistoryLocalizationsController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(HistorySectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHistoryLocalization([FromBody] CreateHistorySectionLocalizationDto createHistorySectionLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateHistoryLocalizationCommand(createHistorySectionLocalizationDto)));
    }

    [HttpPut("{entityId:long}/{languageId:long}")]
    [ProducesResponseType(typeof(HistorySectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHistoryLocalization(
        [FromBody] UpdateHistorySectionLocalizationDto updateHistorySectionLocalizationDto,
        [FromRoute(Name = "entityId")] long EntityId,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateHistoryLocalizationCommand(updateHistorySectionLocalizationDto, EntityId, LanguageId)));
    }
}
