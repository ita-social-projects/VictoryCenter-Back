using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Create;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.HippotherapyProgramCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.BLL.Queries.Admin.HippotherapyProgramCategories;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HippotherapyProgramCategoryController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProgramCategory([FromBody] CreateHippotherapyProgramCategoryDto programCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateHippotherapyProgramCategoryCommand(programCategoryDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgramCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteHippotherapyProgramCategoryCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgramCategory([FromBody] UpdateHippotherapyProgramCategoryDto updateProgramCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateHippotherapyProgramCategoryCommand(updateProgramCategoryDto, id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetProgramCategories()
    {
        return HandleResult(await Mediator.Send(new GetHippotherapyProgramCategoriesQuery()));
    }
}
