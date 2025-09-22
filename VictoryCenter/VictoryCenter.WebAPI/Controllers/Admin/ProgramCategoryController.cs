using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.ProgramCategories.Create;
using VictoryCenter.BLL.Commands.Admin.ProgramCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.ProgramCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.ProgramCategories;
using VictoryCenter.BLL.Queries.Admin.ProgramCategories;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class ProgramCategoryController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProgramCategory([FromBody] CreateProgramCategoryDto programCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateProgramCategoryCommand(programCategoryDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgramCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteProgramCategoryCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgramCategory([FromBody] UpdateProgramCategoryDto updateProgramCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateProgramCategoryCommand(updateProgramCategoryDto, id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetProgramCategories()
    {
        return HandleResult(await Mediator.Send(new GetProgramCategoriesQuery()));
    }
}
