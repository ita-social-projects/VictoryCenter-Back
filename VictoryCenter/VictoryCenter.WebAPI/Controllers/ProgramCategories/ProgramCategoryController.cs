using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.ProgramCategories.Update;
using VictoryCenter.BLL.Commands.ProgramCategories.Create;
using VictoryCenter.BLL.Commands.ProgramCategories.Delete;
using VictoryCenter.BLL.DTOs.ProgramCategories;
using VictoryCenter.BLL.Queries.ProgramCategories;

namespace VictoryCenter.WebAPI.Controllers.ProgramCategories;

[Authorize]
public class ProgramCategoryController : BaseApiController
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
