using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Queries.TeamMembers.GetPublished;

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
