using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Public.TeamPage;
using VictoryCenter.BLL.Queries.Public.TeamPage.GetPublished;

namespace VictoryCenter.WebAPI.Controllers.Public;

public class TeamController : BaseApiController
{
    [HttpGet("published")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryWithPublishedTeamMembersDto>))]
    public async Task<IActionResult> GetPublishedTeamMembers()
    {
        return HandleResult(await Mediator.Send(new GetPublishedTeamMembersQuery()));
    }
}
