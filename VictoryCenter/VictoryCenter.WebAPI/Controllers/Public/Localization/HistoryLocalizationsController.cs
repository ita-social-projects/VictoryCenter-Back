using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.Queries.Admin.Localization.History.GetByEntityId;
using VictoryCenter.BLL.Queries.Admin.Localization.History.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class HistoryLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{entityId:long}")]
    [ProducesResponseType(typeof(IEnumerable<HistorySectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryLocalizationByEntityId(long entityId)
    {
        return HandleResult(await Mediator.Send(new GetHistoryLocalizationByEntityIdQuery(entityId)));
    }

    [HttpGet("languageId/{languageId:long}")]
    [ProducesResponseType(typeof(IEnumerable<HistorySectionLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryLocalizationByLanguageId(long languageId)
    {
        return HandleResult(await Mediator.Send(new GetHistoryLocalizationsByLanguageIdQuery(languageId)));
    }
}
