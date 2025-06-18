using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.TeamMembers.Update;
using VictoryCenter.BLL.Commands.TeamMembers.Delete;
using VictoryCenter.BLL.DTOs.TeamMembers;

namespace VictoryCenter.WebAPI.Controllers.TeamMember;

public class TeamMemberController : BaseApiController
{
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTeamMember(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberCommand(id)));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTeamMember([FromBody] UpdateTeamMemberDto updateTeamMemberDto)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamMemberCommand(updateTeamMemberDto)));
    }
}
