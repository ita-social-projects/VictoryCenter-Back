using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByTeamMemberId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Public.Localization;

public class TeamMemberLocalizationsController : BaseApiController
{
    [HttpGet("member/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByTeamMemberId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByTeamMemberIdQuery(id)));
    }

    [HttpGet("lang/{id:long}")]
    [ProducesResponseType(typeof(IEnumerable<TeamMemberLocalizationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByLanguageIdQuery(id)));
    }
}
