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
    public async Task<IActionResult> GetCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllTeamCategoriesQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateTeamCategoryDto createCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateTeamCategoryCommand(createCategoryDto)));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateTeamCategoryDto updateCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateTeamCategoryCommand(updateCategoryDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteTeamCategoryCommand(id)));
    }
}
