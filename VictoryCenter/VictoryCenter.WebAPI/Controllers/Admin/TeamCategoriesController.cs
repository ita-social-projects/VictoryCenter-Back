using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.TeamCategories;
using VictoryCenter.BLL.Queries.Admin.TeamCategories.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class TeamCategoriesController : AuthorizedApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TeamCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeamCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllTeamCategoriesQuery()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TeamCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeamCategory([FromBody] CreateTeamCategoryDto createTeamCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamCategoryCommand(createTeamCategoryDto)));
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(TeamCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTeamCategory([FromBody] UpdateTeamCategoryDto updateTeamCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamCategoryCommand(updateTeamCategoryDto, id)));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTeamCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamCategoryCommand(id)));
    }
}
