using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Create;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Delete;
using VictoryCenter.BLL.Commands.Admin.HypotherapyProgramCategories.Update;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.BLL.Queries.Admin.HypotherapyProgramCategories;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class HypotherapyProgramCategoryController : AuthorizedApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProgramCategory([FromBody] CreateHypotherapyProgramCategoryDto programCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateHypotherapyProgramCategoryCommand(programCategoryDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteProgramCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteHypotherapyProgramCategoryCommand(id)));
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> UpdateProgramCategory([FromBody] UpdateHypotherapyProgramCategoryDto updateProgramCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateHypotherapyProgramCategoryCommand(updateProgramCategoryDto, id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetProgramCategories()
    {
        return HandleResult(await Mediator.Send(new GetHypotherapyProgramCategoriesQuery()));
    }
}
