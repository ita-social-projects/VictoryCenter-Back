using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.TeamMembers.CreateTeamMember;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.WebAPI.Controllers;

namespace VictoryCenter.Controllers.TeamMembers;

[ApiController]
[Route("api/team-members")]
public class TeamMembersController : BaseApiController
{

    [HttpPost]
    public async Task<IActionResult> CreateTeamMember([FromBody] CreateTeamMemberDto createTeamMemberDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamMemberCommand(createTeamMemberDto)));
    }
    
}