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
    [ProducesResponseType(typeof(List<HistorySectionLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHistoryLocalization([FromBody] List<CreateHistorySectionLocalizationDto> createHistorySectionLocalizationDtos)
    {
        return HandleResult(await Mediator.Send(new CreateHistoryLocalizationCommand(createHistorySectionLocalizationDtos)));
    }

    [HttpPut("{languageId:long}")]
    [ProducesResponseType(typeof(List<HistorySectionLocalizationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHistoryLocalization(
        [FromBody] List<UpdateHistorySectionLocalizationDto> updateHistorySectionLocalizationDtos,
        [FromRoute(Name = "languageId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new UpdateHistoryLocalizationCommand(updateHistorySectionLocalizationDtos, LanguageId)));
    }
}
