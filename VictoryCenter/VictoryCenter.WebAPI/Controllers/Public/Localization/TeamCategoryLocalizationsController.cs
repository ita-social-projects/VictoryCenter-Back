using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamCategories;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByEntityId;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamCategories.GetByLanguageId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class TeamCategoryLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{id:long}")]
    [ProducesResponseType(typeof(List<TeamCategoryLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntityId(long id)
    {
        return HandleResult(await Mediator.Send(new GetTeamCategoryLocalizationByEntityIdQuery(id)));
    }

    [HttpGet("languageId/{id:long}")]
    [ProducesResponseType(typeof(List<TeamCategoryLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetTeamCategoryLocalizationByLanguageIdQuery(id)));
    }
}
