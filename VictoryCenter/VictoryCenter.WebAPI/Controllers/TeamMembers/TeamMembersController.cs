using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.TeamMembers.Create;
using VictoryCenter.BLL.Commands.TeamMembers.Delete;
using VictoryCenter.BLL.Commands.TeamMembers.Reorder;
using VictoryCenter.BLL.Commands.TeamMembers.Update;
using VictoryCenter.BLL.DTOs.TeamMembers;
using VictoryCenter.BLL.Queries.TeamMembers.GetByFilters;
using VictoryCenter.BLL.Queries.TeamMembers.GetById;
using VictoryCenter.BLL.Queries.TeamMembers.Search;

namespace VictoryCenter.WebAPI.Controllers.TeamMembers;

[Authorize]
public class TeamMembersController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeamMemberDto>))]
    public async Task<IActionResult> GetFilteredTeamMembers([FromQuery] TeamMembersFilterDto teamMembersFilterDto)
    {
        return HandleResult(await Mediator.Send(new GetTeamMembersByFiltersQuery(teamMembersFilterDto)));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeamMemberDto>))]
    public async Task<IActionResult> SearchTeamMembers([FromQuery(Name = "fullname")] string fullName)
    {
        return HandleResult(await Mediator.Send(new SearchTeamMemberQuery(new SearchTeamMemberDto { FullName = fullName })));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamMemberDto))]
    public async Task<IActionResult> GetTeamMemberById(long id)
    {
        return HandleResult(await Mediator.Send(new GetTeamMemberByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeamMember([FromBody] CreateTeamMemberDto createTeamMemberDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamMemberCommand(createTeamMemberDto)));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteTeamMember(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamMemberCommand(id)));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateTeamMember([FromBody] UpdateTeamMemberDto updateTeamMemberDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamMemberCommand(updateTeamMemberDto, id)));
    }

    [HttpPut("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReorderTeamMembers([FromBody] ReorderTeamMembersDto dto)
    {
        return HandleResult(await Mediator.Send(new ReorderTeamMembersCommand(dto)));
    }
}
