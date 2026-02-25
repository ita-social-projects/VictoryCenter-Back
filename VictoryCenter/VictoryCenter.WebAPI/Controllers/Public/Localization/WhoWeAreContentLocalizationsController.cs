using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;
using VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByEntityId;
using VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class WhoWeAreContentLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{id:long}")]
    [ProducesResponseType(typeof(List<WhoWeAreContentLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntityId(long id)
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreContentLocalizationByEntityIdQuery(id)));
    }

    [HttpGet("languageId/{id:long}")]
    [ProducesResponseType(typeof(List<WhoWeAreContentLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetWhoWeAreContentLocalizationByLanguageIdQuery(id)));
    }
}
