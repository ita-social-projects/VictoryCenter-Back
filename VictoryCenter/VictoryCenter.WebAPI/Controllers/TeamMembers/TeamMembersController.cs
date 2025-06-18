using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;
using VictoryCenter.BLL.Queries.TeamMembers.GetById;

namespace VictoryCenter.WebAPI.Controllers.TeamMembers;

public class TeamMembersController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeamMemberDto>))]
    public async Task<IActionResult> GetFilteredTeamMembers([FromQuery] TeamMembersFilterDto teamMembersFilterDto)
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
