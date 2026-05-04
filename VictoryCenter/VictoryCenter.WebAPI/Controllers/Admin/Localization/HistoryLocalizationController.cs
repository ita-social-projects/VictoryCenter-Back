using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.History.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class HistoryLocalizationController : AuthorizedApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(HistorySectionLocalizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateHistoryLocalization([FromBody] CreateHistorySectionLocalizationDto createHistorySectionLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateHistoryLocalizationCommand(createHistorySectionLocalizationDto)));
    }
}
