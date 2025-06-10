using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.TeamMember.DeleteTeamMember;

namespace VictoryCenter.WebAPI.Controllers.TeamMember;

public class TeamMemberController : BaseApiController
{
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTeamMember(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberCommand(id)));
    }

}
