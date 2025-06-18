using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.TeamMembers.Delete;

namespace VictoryCenter.WebAPI.Controllers.TeamMember;

public class TeamMemberController : BaseApiController
{
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTeamMember(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberCommand(id)));
    }
}
