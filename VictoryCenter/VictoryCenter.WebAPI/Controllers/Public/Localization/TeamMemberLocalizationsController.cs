using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByEntityId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class TeamMemberLocalizationsController : BaseApiController
{
    [HttpGet("entityId/{id:long}")]
    [ProducesResponseType(typeof(List<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEntityId(long id)
    {
        return HandleResult(await Mediator.Send(new GetTeamMemberLocalizationByEntityIdQuery(id)));
    }

    [HttpGet("languageId/{id:long}")]
    [ProducesResponseType(typeof(List<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetTeamMemberLocalizationByLanguageIdQuery(id)));
    }
}
