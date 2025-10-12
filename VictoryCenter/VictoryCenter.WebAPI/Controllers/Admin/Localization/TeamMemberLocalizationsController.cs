using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Create;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Delete;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.Admin.Localization.TeamMembers;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByLanguageId;
using VictoryCenter.BLL.Queries.Admin.Localization.TeamMembers.GetByTeamMemberId;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin.Localization;

public class TeamMemberLocalizationsController : AuthorizedApiController
{
    [HttpGet("member/{id:long}")]
    public async Task<IActionResult> GetByTeamMemberId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByTeamMemberIdQuery(id)));
    }

    [HttpGet("lang/{id:long}")]
    public async Task<IActionResult> GetByLanguageId(long id)
    {
        return HandleResult(await Mediator.Send(new GetByLanguageIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeamMemberLocalization([FromBody] CreateTeamMemberLocalizationDto createTeamMemberLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamMemberLocalizationCommand(createTeamMemberLocalizationDto)));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTeamMemberLocalization([FromBody] UpdateTeamMemberLocalizationDto updateTeamMemberLocalizationDto)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamMemberLocalizationCommand(updateTeamMemberLocalizationDto)));
    }

    [HttpDelete("{memberId:long}/{langId:long}")]
    public async Task<IActionResult> DeleteTeamMemberLocalization(
        [FromRoute(Name = "memberId")] long TeamMemberId,
        [FromRoute(Name = "langId")] long LanguageId)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberLocalizationCommand(TeamMemberId, LanguageId)));
    }
}
