using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.TeamMember;
using VictoryCenter.BLL.Queries.Pages.GetAllPages;
using VictoryCenter.BLL.Queries.TeamMembers;
using VictoryCenter.WebAPI.Controllers;

namespace VictoryCenter.Controllers.TeamMembers
{
    public class TeamMembersController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeamMemberDto>))]
        public async Task<IActionResult> GetTeamMembers([FromBody] TeamMembersFilterDto teamMembersFilterDto)
        {
            return HandleResult(await Mediator.Send(new GetTeamMembersByFiltersQuery(teamMembersFilterDto)));
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamMemberDto))]
        public async Task<IActionResult> GetTeamMemberById(long id)
        {
            return HandleResult(await Mediator.Send(new GetTeamMemberByIdQuery(id)));
        }
    }
}
