using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Admin.Categories.Create;
using VictoryCenter.BLL.Commands.Admin.Categories.Delete;
using VictoryCenter.BLL.Commands.Admin.Categories.Update;
using VictoryCenter.BLL.DTOs.Admin.Categories;
using VictoryCenter.BLL.Queries.Admin.Categories.GetAll;
using VictoryCenter.WebAPI.Controllers.Common;

namespace VictoryCenter.WebAPI.Controllers.Admin;

public class CategoriesController : AuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        return HandleResult(await Mediator.Send(new GetAllCategoriesQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        return HandleResult(await Mediator.Send(new CreateCategoryCommand(createCategoryDto)));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategoryDto, long id)
    {
        return HandleResult(await Mediator.Send(new UpdateCategoryCommand(updateCategoryDto, id)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteCategoryCommand(id)));
    }
}
