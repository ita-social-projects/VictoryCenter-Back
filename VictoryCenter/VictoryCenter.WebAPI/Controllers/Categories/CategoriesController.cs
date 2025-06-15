using Microsoft.AspNetCore.Mvc;
using VictoryCenter.BLL.Commands.Categories.Create;
using VictoryCenter.BLL.Commands.Categories.Delete;
using VictoryCenter.BLL.Commands.Categories.Update;
using VictoryCenter.BLL.DTOs.Categories;
using VictoryCenter.BLL.Queries.Categories.GetAll;

namespace VictoryCenter.WebAPI.Controllers.Categories;

public class CategoriesController : BaseApiController
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

    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto updateCategoryDto)
    {
        return HandleResult(await Mediator.Send(new UpdateCategoryCommand(updateCategoryDto)));
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        return HandleResult(await Mediator.Send(new DeleteCategoryCommand(id)));
    }
}
